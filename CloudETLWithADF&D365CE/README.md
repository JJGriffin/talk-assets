# Cloud ETL with Azure Data Factory & Microsoft Dataverse #

## Abstract ##

In today's fast-moving cloud world, it can be difficult sometimes to evaluate new solutions on the marketplace to satisfy Extract, Transform and Load (ETL) scenarios or data integration needs involving Microsoft Dataverse. Typically, we may find ourselves returning to "safe" options such as SQL Server Integration Services (SSIS) or to look at investing in potentially expensive third-party solutions, such as Tibco or KingswaySoft. With version 2 of Azure Data Factory (ADF), developers now have a product available with sufficient feature parity to SSIS. In addition, ADF offers a range of additional options, such as the ability to manage your solution within Azure DevOps or seamlessly integrate with Azure functions to execute custom code. All of this, and more, can be implemented for little or no cost.

In this deep-dive session, we'll look at how Azure Data Factory can be used to successfully migrate an on-premise instance of Dynamics CRM 2013 to a Microsoft Dataverse instance. As part of this, attendees will become familiar with the process involved in deploying their first Azure Data Factory Pipeline, from start to finish. No previous experience with Azure Data Factory is required, although attendees should have some familiarity with CDS entities and how to deploy resources into Azure.

## What's Here ##

* **Azure.CloudETLD365CEADF** - Visual Studio Azure Template project, that contains all resources needed to run the sample.
* **D365CEADFDEMO_MSCRM.zip** - ZIP file that contains the SQL Server backup (.bak) of the CRM 2013 organization demoed in the talk, containing all sample data.
* **Cloud ETL with Azure Data Factory & Microsoft Dataverse.pdf** - PDF version of the talks PowerPoint presentation.

## Using the Samples ##

#### Pre-Requisites ####

To work with the samples, you should ideally have:

* A valid Azure subscription
* An on-premise instance of Dynamics CRM 2013, running SQL Server 2008 R2 (other versions may work, but the included databases derives from these versions)
* A Microsoft Dataverse environment
* [Visual Studio 2017 or later with the Azure development and ASP.NET workloads installed](https://docs.microsoft.com/en-us/dotnet/azure/dotnet-tools?view=azure-dotnet&tabs=windows)
* Some familiarity with working with [Azure Template Projects in Visual Studio](https://docs.microsoft.com/en-us/azure/azure-resource-manager/vs-azure-tools-resource-groups-deployment-projects-create-deploy) is advised.

### Instructions ###

1. Clone the repository to your local machine and open the **Azure.CloudETLD365CEADF** project.
2. On the machine that is running your CRM 2013 install, follow the instructions in [this article](https://www.cobalt.net/2016/11/14/how-to-back-up-and-restore-a-database-in-a-crm-organization/) to restore the **D365CEADFDEMO_MSCRM** into your CRM 2013 instance. Once deployed successfully, navigate to the database and reset the password for the **adf-ir** login by running the following script (modify the password to something else, if you like):

```sql

ALTER LOGIN [adf-ir] WITH PASSWORD = N'MyNewPassword'

```
3. Review the values within the **azuredeploy.parameters.json** file and modify to suit your environment/requirements. If you have changed the password to the service account in step 2, then ensure that this updated password is reflected for the **onpremsql_connectionString** variable. For guidance on what values should be used here, you can refer to the parameter **description** values within the **azuredeploy.json** file. Save the **azuredeploy.parameters.json** file once ready.
4. Open the **Azure.CloudETLD365CEADF** project in Visual Studio. Build the project and verify there are no errors.
5. Right click the project and select **Deploy** -> **New...**
6. Within the **Deploy to Resource Group** Dialog Box, select the Azure Subscription that all resources will be deployed to. Then, select (or create) a Resource Group. Press **Deploy** to begin the deployment. This will take several minutes to complete.
7. Once deployed, navigate to the newly created Data Factory resource and select the **Author and Monitor** button. This will load the data factory workspace and, once clicking the **Author** button on the left-hand pane, you should be able to view several resources defined within the factory.
8. Select the **Connections** button at the bottom of the page and then the **Integration Runtimes** tab. Click the pencil icon next to the **CRM2013SQLOnPremise** runtime. Make a note of the **Key 1** value.
9. Download the [Azure Data Factory Integration Runtime](https://www.microsoft.com/en-us/download/details.aspx?id=39717) onto the same machine that has the SQL Server 2008 R2 instance. Follow the instructions in [this article](https://docs.microsoft.com/en-us/azure/data-factory/create-self-hosted-integration-runtime) to install this component and, when prompted, enter the **Key 1** value from earlier.
10. On Azure, navigate to the Azure Key Vault resource created earlier and navigate to the **Secrets** tab. You should see 4 Secret values listed, 1 of which - **SAConnectionString** - will require updating using the **New Version** button. You can obtain the new values for these as follows:
	* **SAConnectionString**: Navigate to the **Access Keys** tab on the storage account resource and copy the **key1 Connection String** value. You should also make a note of the **Key** value, which will be used in later steps.
11. Remaining on the Azure Key Vault resource, navigate to the **Access policies** tab and click on the **+ Add New** button. Under the **Select principal** option, type in the name of your Azure Data Factory resource created in step 6 and select it. Then, under **Secret permissions**, tick the **Get** option, press **OK** and then **Save**. This will grant the Azure Data Factory resources privilege to retrieve all secret values from the Key Vault. This can be confirmed by going back to Azure Data Factory and selecting the **Test Connection** option under each **Linked Service**.
12. With everything configured successfully, navigate to the **CRM2013_D365CE_DataMigration** pipeline and click on **Debug** to execute it. Pipeline execution will take approximately 15-20 minutes to complete.

## Disclaimer ##

The examples include in this repository are provided "as-is" with no warranty expressed or implied. Please feel free to raise an issue if you encounter any problems, and I will happily take a look, but I can't offer any guarantee of resolution.