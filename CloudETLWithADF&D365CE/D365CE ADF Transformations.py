# Databricks notebook source
# MAGIC %md
# MAGIC # D365CE ADF Transfomation Example
# MAGIC ##Goal
# MAGIC 
# MAGIC Read data with Apache Spark using parameters, supplied from Azure Data Factory, based on 3 different inputs:  
# MAGIC * **Staging_Account.csv**: Sample Account entity data from Dynamics CRM 2013, extracted from SQL Server 2008
# MAGIC * **Staging_Lead.csv**: Sample Lead entity data from Dynamics CRM 2013, extracted from SQL Server 2008
# MAGIC * **Staging_Contact.csv**: Sample Contact entity data from Dynamics CRM 2013, extracted from SQL Server 2008
# MAGIC 
# MAGIC Depending on the input, apply the following transformations and output as new .csv files:
# MAGIC 
# MAGIC ###Staging_Account.csv
# MAGIC 
# MAGIC Modify the **Account Name** by appending the following values to the end of the field, based on the number of employees for each record:
# MAGIC 
# MAGIC * ** < 50**: Append **- SMB**
# MAGIC * ** Between 51 and 250**: Append **- SME**
# MAGIC * ** > 251**: Append **- Enterprise**
# MAGIC 
# MAGIC Unique Identifiers for each team record to be supplied via the Data Factory as parameters
# MAGIC 
# MAGIC ###Staging_Lead.csv
# MAGIC 
# MAGIC Filter data to only return Leads with a Status of **Open**
# MAGIC 
# MAGIC ###Staging_Contact.csv
# MAGIC 
# MAGIC Append the following fields/values to indicate the contact preference:
# MAGIC * **Contact Method**: Email
# MAGIC * **Email**: Allow
# MAGIC * **Follow Email**: Allow
# MAGIC * **Bulk Email**: Allow
# MAGIC * **Phone**: Do Not Allow
# MAGIC * **Fax**: Do Not Allow
# MAGIC * **Mail**: Do Not Allow
# MAGIC 
# MAGIC ###Notes
# MAGIC 
# MAGIC Notebook has been adapted from the example described in the following article/blob storage location:
# MAGIC 
# MAGIC https://docs.microsoft.com/en-us/azure/data-factory/solution-template-databricks-notebook  
# MAGIC https://adflabstaging1.blob.core.windows.net/share/Transformations.html

# COMMAND ----------

# MAGIC %md
# MAGIC ##### Define Widgets / Parameters
# MAGIC Creating widgets for leveraging parameters to be passed from Azure Data Factory

# COMMAND ----------

dbutils.widgets.text("filename", "","") 
dbutils.widgets.get("filename")

# COMMAND ----------

# MAGIC %md
# MAGIC ##### Mount Storage as DBFS
# MAGIC **[ACTION REQUIRED]** Please update the **storageName** and the **accesskey** parameters, based on the Storage Account Name and API Key from your generated Storage Account resource. 

# COMMAND ----------

#Supply storageName and accessKey values, setting the container URL based on the supplied file name.
storageName = "mystorageaccountname"
accessKey = "myaccesskey"
#Mount the appropriate Blob Container, based on the name of the file beign processed.
fn = getArgument("filename")
if "Staging_Account" in fn:
  container_source = "wasbs://crmsinkdata-account@"+storageName+".blob.core.windows.net/"
elif "Staging_Lead" in fn:
  container_source = "wasbs://crmsinkdata-lead@"+storageName+".blob.core.windows.net/"
elif "Staging_Contact" in fn:
  container_source = "wasbs://crmsinkdata-contact@"+storageName+".blob.core.windows.net/"
else:
  raise Exception('Invalid file name {} supplied by Data Factory'.format(fn))
try:
  dbutils.fs.mount(source = container_source, mount_point = "/mnt/adfdata", extra_configs = {"fs.azure.account.key."+storageName+".blob.core.windows.net": accessKey})
except Exception as e:
  #The error message has a long stack trace.  This code tries to print just the relevent line indicating what failed.
  import re
  result = re.findall(r"^\s*Caused by:\s*\S+:\s*(.*)$", e.message, flags=re.MULTILINE)
  if result:
    print(result[-1]) # Print only the relevant error message
  else:
    print(e) # Otherwise print the whole stack trace.

# COMMAND ----------

# MAGIC %md
# MAGIC Create a **DataFrame** object from the CSV file and display as part of the job (for debugging)

# COMMAND ----------

# Create DataFrame

inputFile = "dbfs:/mnt/adfdata/"+getArgument("filename")
initialDF = (spark.read           # The DataFrameReader
  .option("header", "true")       # Use first line of all files as header
  .option("inferSchema", "true")  # Automatically infer data types
  .csv(inputFile)                 # Creates a DataFrame from CSV after reading in the file
)

display(initialDF)

# COMMAND ----------

# MAGIC %md
# MAGIC Perform the appropraite transformations to the Data Frame, based on the name of the supplied field

# COMMAND ----------

from pyspark.sql.functions import col, when, lit, concat
if "Staging_Account" in fn:
  #Locate the columns that require updating, based on number of employees, and perform the actual update of the Account Name
  finalDF = initialDF.withColumn("name", when(initialDF["numberofemployees"] <= 50, concat(initialDF["name"], lit(" - SMB"))).otherwise(when((initialDF["numberofemployees"] >= 51) & (initialDF["numberofemployees"] <= 250), concat(initialDF["name"], lit(" - SME"))).otherwise(concat(initialDF["name"], lit(" - Enterprise")))))
elif "Staging_Lead" in fn:
  #Filter the CSV data to only return Active Status Leads
  finalDF = initialDF.filter(initialDF.statecode == 0).sort(initialDF.leadid)
elif "Staging_Contact" in fn:
  #Add additional columns to the CSV data for contact preferences
  pcmc = 2
  allow = 0
  doNotAllow = 1
  finalDF = initialDF.withColumn("preferredcontactmethodcode", lit(pcmc)) #Email
  finalDF = finalDF.withColumn("donotemail", lit(allow)) #Allow
  finalDF = finalDF.withColumn("donotbulkemail", lit(allow)) #Allow
  finalDF = finalDF.withColumn("donotphone", lit(doNotAllow)) #Do Not Allow
  finalDF = finalDF.withColumn("donotfax", lit(doNotAllow)) #Do Not Allow
  finalDF = finalDF.withColumn("donotpostalmail", lit(doNotAllow)) #Do Not Allow
#Throw an error if an incorrect file name is supplied
else:
  raise Exception('Invalid file name {} supplied by Data Factory'.format(fn))


# COMMAND ----------

# MAGIC %md
# MAGIC Write the **output** to **CSV**, overwriting the existing CSV in the process. Code is adapted from the following example supplied [here](https://medium.com/@zifah/how-to-write-data-from-an-azure-databricks-notebook-to-an-azure-blob-storage-container-283bfdf23893).

# COMMAND ----------

#Removing Extension from filename
import os
file = os.path.splitext(getArgument("filename"))[0]
print(file)

output_file_path = "dbfs:/mnt/adfdata"

# write the dataframe as a single file to blob storage
finalDF.coalesce(1).write.mode("overwrite").option("header", "true").csv(output_file_path) #for csv, coalesce(1) only for demo, 

# Get the name of the wrangled-data CSV file that was just saved to Azure blob storage (it starts with 'part-')
# Also define the list of other output files that require cleanup.
files = dbutils.fs.ls(output_file_path)
output_file = [x for x in files if x.name.startswith("part-")]
delete_files = [x for x in files if x.name.startswith("_SUCCESS") or x.name.startswith("_committed_") or x.name.startswith("_started_")]

# Move the wrangled-data CSV file from a sub-folder (wrangled_data_folder) to the root of the blob container
# While simultaneously changing the file name
dbutils.fs.mv(output_file[0].path, output_file_path+"/"+file+"_Transformed.csv")

#Remove unnecessary, leftover files.
for file in delete_files:
  dbutils.fs.rm(file.path, True)

# COMMAND ----------

# MAGIC %md
# MAGIC Dismount the drive that has been mounted explicitely, to prevent errors on cluster for subsequent jobs

# COMMAND ----------

dbutils.fs.unmount("/mnt/adfdata")

# COMMAND ----------

# MAGIC %md-sandbox
# MAGIC **End**