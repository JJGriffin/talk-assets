using System;
using System.Globalization;
using System.ServiceModel;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

namespace D365CE.CustomPricingTalk
{
    public class PostOpCalculatePriceCustomPricingTalk : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            //Obtain the execution context from the service provider.
            IPluginExecutionContext context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));

            //Extract the tracing service for use in debugging sandboxed plug-ins

            ITracingService tracing = (ITracingService)serviceProvider.GetService(typeof(ITracingService));
            tracing.Trace("Tracing implemented successfully!");

            //Check the parent context, break if it contains CustomPrice shared variable - this prevents infinite loop

            if (context.ParentContext != null
                && context.ParentContext.ParentContext != null
                && context.ParentContext.ParentContext.ParentContext != null
                && context.ParentContext.ParentContext.ParentContext.SharedVariables.ContainsKey("CustomPrice")
                && (bool)context.ParentContext.ParentContext.ParentContext.SharedVariables["CustomPrice"])
                return;

            // The InputParameters collection contains all the data passed in the message request.            
            if (context.InputParameters.Contains("Target") 
                && context.InputParameters["Target"] is EntityReference)
            {
                // Obtain the target entity from the input parmameters.
                EntityReference entity = (EntityReference)context.InputParameters["Target"];

                // Verify that the target entity represents an appropriate entity.                
                if (CheckIfNotValidEntity(entity))
                    return;

                try
                {
                    //Add shared variable, used earlier to check for infinite loops
                    context.SharedVariables.Add("CustomPrice", true);
                    context.ParentContext.SharedVariables.Add("CustomPrice", true);

                    //Get a reference to the organization service - used later
                    IOrganizationServiceFactory serviceFactory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
                    IOrganizationService service = serviceFactory.CreateOrganizationService(context.UserId);

                    switch(entity.LogicalName)
                    {
                        case "opportunityproduct":
                        case "invoicedetail":
                        case "quotedetail":
                        case "salesorderdetail":
                            tracing.Trace("Calculating " + entity.LogicalName + " as a line item calculation.");
                            CalculateLineItem(entity, service, tracing);
                            return;
                        case "opportunity":
                        case "quote":
                        case "salesorder":
                        case "invoice":
                            tracing.Trace("Calculating " + entity.LogicalName + " as a sales document calculation.");
                            CalculateSalesDocument(entity, service, tracing);
                            return;
                        default:
                            throw new InvalidPluginExecutionException("Entity with name " + entity.LogicalName + " is not valid for calculating price information.");
                    }
                }

                catch (FaultException<OrganizationServiceFault> ex)
                {
                    tracing.Trace("CalculatePrice: {0}", ex.ToString());
                    throw new InvalidPluginExecutionException("An error occurred in the Calculate Price plug-in.", ex);
                }

                catch (Exception ex)
                {
                    tracing.Trace("CalculatePrice: {0}", ex.ToString());
                    throw;
                }
            }

            //Currently appears to be some bug/issue calculating Quote Line Extended Amounts when generating from an Opportunity
            //The code that follows deals with this issue, via an additiona plug-in step on Create of a Quote Line item.

            else if (context.InputParameters.Contains("Target") 
                     && context.InputParameters["Target"] is Entity)
            {
                // Obtain the target entity from the input parmameters.
                Entity entity = (Entity)context.InputParameters["Target"];

                if (entity.LogicalName == "quotedetail")
                {
                    //Add shared variable, used earlier to check for infinite loops
                    context.SharedVariables.Add("CustomPrice", true);
                    context.ParentContext.SharedVariables.Add("CustomPrice", true);

                    //Get a reference to the organization service - used later
                    IOrganizationServiceFactory serviceFactory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
                    IOrganizationService service = serviceFactory.CreateOrganizationService(context.UserId);

                    tracing.Trace("Calculating " + entity.LogicalName + " as a line item calculation, which has just been created.");
                    CalculateLineItem(entity, service, tracing);
                }

            }
        }
        /// <summary>
        /// Checks to see if the entity logical name is valid for use for the CalculatePrice message and, if so, returns false; else, true.
        /// </summary>
        /// <param name="entity">The entity to check</param>
        private static bool CheckIfNotValidEntity(EntityReference entity)
        {
            switch (entity.LogicalName)
            {
                case "opportunity":
                case "quote":
                case "salesorder":
                case "invoice":
                case "opportunityproduct":
                case "invoicedetail":
                case "quotedetail":
                case "salesorderdetail":
                    return false;

                default:
                    return true;
            }
        }
        /// <summary>
        /// Performs all appropriate calculations for a line item record, according to custom business logic.
        /// </summary>
        /// <param name="lineItem">The line item record to calculate.</param>
        /// <param name="service">A reference to the IOrganizationService. Required to perform record retrievals.</param>
        /// <param name="tracing">A reference to the ITracingService. Required for debugging.</param>
        private static void CalculateLineItem(EntityReference lineItem, IOrganizationService service, ITracingService tracing)
        {
            //Retrieve all required fields, including custom fields, from the Product record
            //For this example, we return all fields; this is NOT recommended for Production environments
            tracing.Trace("Retrieving " + lineItem.LogicalName + " record...");
            Entity e1 = new Entity();
            e1 = service.Retrieve(lineItem.LogicalName, lineItem.Id, new ColumnSet(true));
            tracing.Trace("Retrieved " + lineItem.LogicalName + " record with ID " + e1.Id.ToString());

            //If Order Line or Invoice Line, we need to check the related Order/Invoice to determine whether pricing is locked.
            //If locked, then no calculation takes place, to prevent errors.

            bool pricingLocked = CheckIfPricingLocked(e1, service, tracing);
            if (pricingLocked == false)
            {

                //If Existing Product, then we need to check to ensure it is not being sold underneath the value specified in the price list

                bool isWriteIn = e1.GetAttributeValue<bool>("isproductoverridden");

                if (isWriteIn == false)
                {
                    tracing.Trace("Existing product record detected, checking Price List cost price value...");
                    bool isUnderCostPrice = CheckLineItemCostPrice(e1, service, tracing);
                    if (isUnderCostPrice == true)
                    {
                        throw new InvalidPluginExecutionException("You are attempting to sell this product record below its listed cost price value. Please update the record to ensure that it is equal or higher than its price list value.");
                    }
                }

                //Obtain the correct Discount and Freight Amounts for the product record.
                //As Opportunity entity does not have Ship To Address values, we default to £25 instead.

                Money da = CalculateLineItemDiscount(e1, service, tracing);
                Money fa = new Money(25);

                if(e1.LogicalName != "opportunityproduct")
                {
                    fa = CalculateLineItemFreight(e1, service, tracing);
                }

                //Set all required amounts on the line item record.
                //First, obtain the values needed from the product record.

                decimal ppu = e1.GetAttributeValue<Money>("priceperunit")?.Value ?? 0;
                decimal q = e1.GetAttributeValue<decimal>("quantity");
                decimal t = e1.GetAttributeValue<Money>("tax")?.Value ?? 0;

                //The, it's time to calculate!

                //Amount = Price Per Unit * Quantity
                decimal a = ppu * q;
                e1["baseamount"] = new Money(a);
                tracing.Trace("Amount = " + a.ToString());
                //Manual Discount
                e1["manualdiscountamount"] = da;
                tracing.Trace("Discount Amount = " + da.ToString());
                //Freight Amount
                e1["jjg_freightamount"] = fa;
                tracing.Trace("Freight Amount = " + fa.ToString());
                //Extended Amount = (Amount - Manual Discount) + Freight Amount + Tax
                decimal ea = (a - da.Value) + fa.Value + t;
                e1["extendedamount"] = new Money(ea);
                tracing.Trace("Extended Amount = " + ea.ToString());

                //Actually update the record

                service.Update(e1);
                tracing.Trace(e1.LogicalName + " updated successfully!");

            }

        }
        /// <summary>
        /// Performs all appropriate calculations for a line item record, according to custom business logic.
        /// </summary>
        /// <param name="lineItem">The line item record to calculate.</param>
        /// <param name="service">A reference to the IOrganizationService. Required to perform record retrievals.</param>
        /// <param name="tracing">A reference to the ITracingService. Required for debugging.</param>
        private static void CalculateLineItem(Entity lineItem, IOrganizationService service, ITracingService tracing)
        {
            //Retrieve all required fields, including custom fields, from the Product record
            //For this example, we return all fields; this is NOT recommended for Production environments
            tracing.Trace("Retrieving " + lineItem.LogicalName + " record...");
            Entity e1 = new Entity();
            e1 = service.Retrieve(lineItem.LogicalName, lineItem.Id, new ColumnSet(true));
            tracing.Trace("Retrieved " + lineItem.LogicalName + " record with ID " + e1.Id.ToString());

            //If Order Line or Invoice Line, we need to check the related Order/Invoice to determine whether pricing is locked.
            //If locked, then no calculation takes place, to prevent errors.

            bool pricingLocked = CheckIfPricingLocked(e1, service, tracing);
            if (pricingLocked == false)
            {

                //If Existing Product, then we need to check to ensure it is not being sold underneath the value specified in the price list

                bool isWriteIn = e1.GetAttributeValue<bool>("isproductoverridden");

                if (isWriteIn == false)
                {
                    tracing.Trace("Existing product record detected, checking Price List cost price value...");
                    bool isUnderCostPrice = CheckLineItemCostPrice(e1, service, tracing);
                    if (isUnderCostPrice == true)
                    {
                        throw new InvalidPluginExecutionException("You are attempting to sell this product record below its listed cost price value. Please update the record to ensure that it is equal or higher than its price list value.");
                    }
                }

                //Obtain the correct Discount and Freight Amounts for the product record.
                //As Opportunity entity does not have Ship To Address values, we default to £25 instead.

                Money da = CalculateLineItemDiscount(e1, service, tracing);
                Money fa = new Money(25);

                if (e1.LogicalName != "opportunityproduct")
                {
                    fa = CalculateLineItemFreight(e1, service, tracing);
                }

                //Set all required amounts on the line item record.
                //First, obtain the values needed from the product record.

                decimal ppu = e1.GetAttributeValue<Money>("priceperunit")?.Value ?? 0;
                decimal q = e1.GetAttributeValue<decimal>("quantity");
                decimal t = e1.GetAttributeValue<Money>("tax")?.Value ?? 0;

                //The, it's time to calculate!

                //Amount = Price Per Unit * Quantity
                decimal a = ppu * q;
                e1["baseamount"] = new Money(a);
                tracing.Trace("Amount = " + a.ToString());
                //Manual Discount
                e1["manualdiscountamount"] = da;
                tracing.Trace("Discount Amount = " + da.ToString());
                //Freight Amount
                e1["jjg_freightamount"] = fa;
                tracing.Trace("Freight Amount = " + fa.ToString());
                //Extended Amount = (Amount - Manual Discount) + Freight Amount + Tax
                decimal ea = (a - da.Value) + fa.Value + t;
                e1["extendedamount"] = new Money(ea);
                tracing.Trace("Extended Amount = " + ea.ToString());

                //Actually update the record

                service.Update(e1);
                tracing.Trace(e1.LogicalName + " updated successfully!");

            }
        }
        /// <summary>
        /// Performs all appropriate calculations for a line item record, according to custom business logic.
        /// </summary>
        /// <param name="lineItem">The line item record to calculate.</param>
        /// <param name="service">A reference to the IOrganizationService. Required to perform record retrievals.</param>
        /// <param name="tracing">A reference to the ITracingService. Required for debugging.</param>
        private static void CalculateSalesDocument(EntityReference salesDoc, IOrganizationService service, ITracingService tracing)
        {
            //Only calculate record if it is in an Active state

            Entity e = service.Retrieve(salesDoc.LogicalName, salesDoc.Id, new ColumnSet("statecode"));
            OptionSetValue statecode = (OptionSetValue)e["statecode"];
            if (statecode.Value == 0)
            {

                //Assign correct entity name for related Product records

                string pe = GetProductEntityName(salesDoc.LogicalName);
                string peID = salesDoc.LogicalName + "id";

                //Build the query to return all line items for the related Sales Document

                QueryExpression query = new QueryExpression(pe);
                query.ColumnSet.AddColumns("baseamount", "manualdiscountamount", "jjg_discountpercent", "jjg_freightamount", "tax", "extendedamount");
                query.Criteria.AddCondition(peID, ConditionOperator.Equal, salesDoc.Id);
                EntityCollection ec = service.RetrieveMultiple(query);

                //Iterate through and total up values for each line item
                //Also create list to store discount percent values (used later)

                decimal da = 0;
                decimal d = 0;
                decimal fa = 0;
                decimal t = 0;
                decimal ta = 0;

                List<List<int>> l = new List<List<int>>();

                for (int i = 0; i < ec.Entities.Count; i++)
                {
                    Money totalAmount = ec.Entities[i].GetAttributeValue<Money>("baseamount");
                    Money totalDiscount = ec.Entities[i].GetAttributeValue<Money>("manualdiscountamount");
                    Money totalFreightAmount = ec.Entities[i].GetAttributeValue<Money>("jjg_freightamount");
                    Money totalTax = ec.Entities[i].GetAttributeValue<Money>("tax");
                    Money totalExtendedAmount = ec.Entities[i].GetAttributeValue<Money>("extendedamount");

                    //If no value in the returned fields, default to 0

                    da += totalAmount?.Value ?? 0;
                    d += totalDiscount?.Value ?? 0;
                    fa += totalFreightAmount?.Value ?? 0;
                    t += totalTax?.Value ?? 0;
                    ta += totalExtendedAmount?.Value ?? 0;

                    l.Add(new List<int> { ec.Entities[i].GetAttributeValue<int>("jjg_discountpercent")});
                }

                //For discounts, we figure out an average (arithmetic mean) percentage discount, based on each line item returned.
                //We only calculate this if there are related line item records, otherwise, set to 0.

                double dp = 0;

                if (ec.Entities.Count != 0)
                {
                    dp = Math.Round(l.Average(inner => inner[0]), 2, MidpointRounding.AwayFromZero);
                }

                //Update entity with required values

                e["totallineitemamount"] = new Money(da);
                tracing.Trace("Total Detail Amount = " + da.ToString());
                e["discountpercentage"] = dp;
                tracing.Trace("Invoice Discount (%) = " + dp.ToString());
                e["discountamount"] = new Money(d);
                tracing.Trace("Invoice Discount Amount = " + d.ToString());
                //Pre-Freight Amount = Detail Amount - Discount
                decimal pfa = da - d;
                e["totalamountlessfreight"] = new Money(pfa);
                tracing.Trace("Total Pre-Freight Amount = " + pfa.ToString());
                e["freightamount"] = new Money(fa);
                tracing.Trace("Total Freight Amount = " + fa.ToString());
                e["totaltax"] = new Money(t);
                tracing.Trace("Total Tax = " + t.ToString());
                e["totalamount"] = new Money(ta);
                tracing.Trace("Total Amount = " + ta.ToString());

                //Actually update the sales document

                service.Update(e);
                tracing.Trace(e.LogicalName + " updated successfully!");

            }
        }
        /// <summary>
        /// Calculates the appropriate discount amount to apply to a line item record, and returns this as a monetary value.
        /// </summary>
        /// <param name="lineItem">The line item record to calculate the discount for</param>
        /// <param name="service">A reference to the IOrganizationService. Required to perform record retrievals.</param>
        /// <param name="tracing">A reference to the ITracingService. Required for debugging.</param>
        private static Money CalculateLineItemDiscount(Entity lineItem, IOrganizationService service, ITracingService tracing)
        {
            tracing.Trace("Calculating discount for line item record...");

            //Initialise the values needed
            Money da = new Money(0);
            int? dp = lineItem.GetAttributeValue<int?>("jjg_discountpercent");
            Money ba = lineItem.GetAttributeValue<Money>("baseamount");

            //Determine if a discount percentage exists on the Line item - have to make integer nullable to check this correctly
            if (dp != null && ba != null)
            {
                //If so, then apply the decrease accordingly
                //Have to convert Discount Percentage to a decimal, so it can be used in calculations.
                decimal dpDec = new decimal((int)dp);
                //Then, we can obtain the discounted Base Amount value
                da = new Money(ba.Value * dpDec / 100);
                tracing.Trace("Discount Amount = " + da.Value.ToString());
            }
            //Otherwise, we need to check the parent sales entity instead and use whatever is listed there
            else if (dp == null && ba != null)
            {
                //Use helper methods to retrieve the entity and lookup names we need and obtain ID for this record.
                string peName = GetProductEntityName(lineItem.LogicalName);
                string peIDName = GetProductEntityIDName(lineItem.LogicalName);
                EntityReference peID = lineItem.GetAttributeValue<EntityReference>(peIDName);
                //Do a NULL check, if NULL, then we throw an error - as we want to prevent orphan line item records.
                if (peID != null)
                {
                    Entity pe = service.Retrieve(peName, peID.Id, new ColumnSet("jjg_discountpercent"));
                    dp = pe.GetAttributeValue<int?>("jjg_discountpercent");
                    if (dp != null)
                    {
                        //Once retrieved, we then apply our calculation.
                        //Have to convert Discount Percentage to a decimal, so it can be used in calculations.
                        decimal dpDec = new decimal((int)dp);
                        da = new Money(ba.Value * dpDec / 100);
                        tracing.Trace("Discount Amount = " + da.Value.ToString());
                    }
                    else
                    {
                        tracing.Trace("WARNING: Could not retrieve a discount percentage value from line item's parent record. Continuing...");
                    }
                }
                else
                {
                    throw new InvalidPluginExecutionException("Could not calculate line item record as it is not associated to a parent sales record. Please associate a parent record to this line item and try saving again.");
                }
            }
            //If all else fails, we just return a 0
            else
            {
                tracing.Trace("WARNING: Could not retrieve a discount percentage value from line item record. Continuing...");
            }
            return da;
        }
        /// <summary>
        /// Calculates the appropriate freight amount to apply to a line item record, and returns this as a monetary value.
        /// </summary>
        /// <param name="lineItem">The line item record to calculate the discount for</param>
        /// <param name="service">A reference to the IOrganizationService. Required to perform record retrievals.</param>
        /// <param name="tracing">A reference to the ITracingService. Required for debugging.</param>
        private static Money CalculateLineItemFreight(Entity lineItem, IOrganizationService service, ITracingService tracing)
        {
            tracing.Trace("Calculating freight amount for line item record...");

            //Initialise the values needed
            //25 is the default amount we want to return if we cannot get the correct value
            Money fa = new Money(25);

            //Retrieve parent sales document record and its associated City value
            //Use helper methods to retrieve the entity and lookup names we need and obtain ID for this record.
            string peName = GetProductEntityName(lineItem.LogicalName);
            string peIDName = GetProductEntityIDName(lineItem.LogicalName);
            EntityReference peID = lineItem.GetAttributeValue<EntityReference>(peIDName);
            Entity psd = new Entity();
            psd = service.Retrieve(peName, peID.Id, new ColumnSet("shipto_city"));

            //Get the city value - if NULL, then we default to Freight amount of £25

            string city = psd.GetAttributeValue<string>("shipto_city");

            if (city != null)
            {
                //Query the Freight Amount entity, using the City name field.
                //Can use QueryByAttribute here, due to simple nature of the query

                QueryByAttribute qba = new QueryByAttribute("jjg_freightamount");
                qba.ColumnSet = new ColumnSet("jjg_freightamountvalue");
                qba.Attributes.AddRange("jjg_city");
                qba.Values.AddRange(city);

                EntityCollection faEntity = service.RetrieveMultiple(qba);

                //Get the Freight Amount value
                //Always grab the top record only - the instance in question should have duplicate detection rules in place, to prevent potential conflicts

                if (faEntity.TotalRecordCount == 1)
                {
                    fa = faEntity[0].GetAttributeValue<Money>("jjg_freightamountvalue");
                }

                else
                {
                    tracing.Trace("Could not obtain a Freight Amount Value, defaulting to £25");
                }
            }
            else
            {
                tracing.Trace("No Ship To City value supplied for " + psd.LogicalName + " record, defaulting to £25");
            }

            return fa;
        }
        /// <summary>
        /// Returns the corresponding child Sales entity line entity logical name and vice-versa. For example, passing "opportunityproduct" to this method would return "opportunity"
        /// </summary>
        /// <param name="entity">The logical name of the entity to evaluate</param>
        private static string GetProductEntityName(string entity)
        {
            string pe = "";

            switch (entity)
            {
                //Parent -> Line Item

                case "opportunity":
                    pe = "opportunityproduct";
                    return pe;
                case "quote":
                    pe = "quotedetail";
                    return pe;
                case "salesorder":
                    pe = "salesorderdetail";
                    return pe;
                case "invoice":
                    pe = "invoicedetail";
                    return pe;

                //Line Item -> Parent

                case "opportunityproduct":
                    pe = "opportunity";
                    return pe;
                case "quotedetail":
                    pe = "quote";
                    return pe;
                case "salesorderdetail":
                    pe = "salesorder";
                    return pe;
                case "invoicedetail":
                    pe = "invoice";
                    return pe;

                default:
                    pe = "";
                    return pe;
            }
        }
        /// <summary>
        /// Returns the corresponding parent Sales entity line parent lookup, logical field name. For example, passing "opportunityproduct" to this method would return "opportunityid"
        /// </summary>
        /// <param name="entity">The logical name of the entity to evaluate</param>
        private static string GetProductEntityIDName(string entity)
        {
            string pe = "";

            switch (entity)
            {
                case "opportunityproduct":
                    pe = "opportunityid";
                    return pe;
                case "quotedetail":
                    pe = "quoteid";
                    return pe;
                case "salesorderdetail":
                    pe = "salesorderid";
                    return pe;
                case "invoicedetail":
                    pe = "invoiceid";
                    return pe;
                default:
                    pe = "";
                    return pe;
            }
        }
        /// <summary>
        /// Retrieve cost price details from the Product line item record supplied and determine return true if it is being sold below cost price; else, false.
        /// </summary>
        /// <param name="lineItem">The line item record to evaluate.</param>
        /// <param name="service">A reference to the IOrganizationService. Required to perform record retrievals.</param>
        /// <param name="tracing">A reference to the ITracingService. Required for debugging.</param>
        private static bool CheckLineItemCostPrice(Entity lineItem, IOrganizationService service, ITracingService tracing)
        {
            bool underCostPrice = false;

            //Retrieve parent sales document record and its associated Price List ID value
            //Use helper methods to retrieve the entity and lookup names we need and obtain ID for this record.
            string peName = GetProductEntityName(lineItem.LogicalName);
            string peIDName = GetProductEntityIDName(lineItem.LogicalName);
            EntityReference peID = lineItem.GetAttributeValue<EntityReference>(peIDName);
            Entity psd = new Entity();
            psd = service.Retrieve(peName, peID.Id, new ColumnSet("pricelevelid"));

            //Get the Price List ID value - if NULL, then we throw an error, as this should be there

            EntityReference pl = psd.GetAttributeValue<EntityReference>("pricelevelid");

            if (pl != null)
            {
                //Query the Price List Item entity, using the Price List ID and Product ID field from the line item.
                //Have to use QueryExpression here, as we are filtering on multiple fields.

                QueryExpression qe = new QueryExpression("productpricelevel");
                qe.ColumnSet.AddColumn("amount");

                ConditionExpression condition1 = new ConditionExpression("pricelevelid", ConditionOperator.Equal, pl.Id);
                ConditionExpression condition2 = new ConditionExpression("productid", ConditionOperator.Equal, lineItem.GetAttributeValue<EntityReference>("productid").Id);

                FilterExpression filter1 = new FilterExpression();
                filter1.Conditions.Add(condition1);

                FilterExpression filter2 = new FilterExpression();
                filter2.Conditions.Add(condition2);

                qe.Criteria.AddFilter(filter1);
                qe.Criteria.AddFilter(filter2);

                qe.PageInfo.ReturnTotalRecordCount = true;

                EntityCollection pliEntity = service.RetrieveMultiple(qe);

                //Get the Amount value from the Price List Item record
                //Always grab the top record only - only 1 record should ever be present, due to application limits.

                if (pliEntity.TotalRecordCount == 1)
                {
                    //Get the appropriate values from the line item and price list item record and perform the comparison.
                    Money a = pliEntity[0].GetAttributeValue<Money>("amount");
                    Money liUP = lineItem.GetAttributeValue<Money>("priceperunit");
                    tracing.Trace("Price List Amount = " + a.Value.ToString() + ", Line Item Price Per Unit = " + liUP.Value.ToString());

                    if (liUP.Value < a.Value)
                    {
                        tracing.Trace("Product is being sold under cost!");
                        underCostPrice = true;
                    }
                    else
                    {
                        tracing.Trace("Product is being sold at or over cost!");
                    }
                }

                //Throw an error if nothing is there - having come this far, having nothing here indicates something has gone wrong.

                else
                {
                    throw new InvalidPluginExecutionException("A problem occurred when retrieving an original amount for an existing line item record.");

                }
            }

            else
            {
                throw new InvalidPluginExecutionException("Could not locate a price list ID when attempting to calculate whether this line item is being sold under cost.");
            }

            return underCostPrice;
        }
        /// <summary>
        /// Determines whether prices are locked for an Order Product or Invoice Product record and, if so, returns true; else, false.
        /// </summary>
        /// <param name="entity">The entity to evaluate. Must equal either salesorderdetail or invoicedetail</param>
        /// <param name="service">A reference to the IOrganizationService. Required to perform record retrievals.</param>
        /// <param name="tracing">A reference to the ITracingService. Required for debugging.</param>
        private static bool CheckIfPricingLocked(Entity entity, IOrganizationService service, ITracingService tracing)
        {
            tracing.Trace("Determining whether prices are locked or not...");
            bool pricingLocked = false;
            if (entity.LogicalName == "salesorderdetail" || entity.LogicalName == "invoicedetail")
            {
                tracing.Trace(string.Concat("Entity is ", entity.LogicalName, ", checking whether prices are locked..."));

                if (entity.LogicalName == "salesorderdetail")
                {
                    Entity e2 = new Entity();
                    e2 = service.Retrieve("salesorder", entity.GetAttributeValue<EntityReference>("salesorderid").Id, new ColumnSet("ispricelocked"));
                    if (e2.GetAttributeValue<Boolean>("ispricelocked") == true)
                    {
                        tracing.Trace(string.Concat("salesorder record with ID ", e2.Id.ToString(), " has its pricing locked."));
                        pricingLocked = true;
                    }
                    else
                    {
                        tracing.Trace(string.Concat("salesorder record with ID ", e2.Id.ToString(), " does NOT have its pricing locked."));
                    }
                }
                else if (entity.LogicalName == "invoicedetail")
                {
                    Entity e2 = new Entity();
                    e2 = service.Retrieve("invoice", entity.GetAttributeValue<EntityReference>("invoiceid").Id, new ColumnSet("ispricelocked"));
                    if (e2.GetAttributeValue<Boolean>("ispricelocked") == true)
                    {
                        tracing.Trace(string.Concat("invoice record with ID ", e2.Id.ToString(), " has its pricing locked."));
                        pricingLocked = true;
                    }
                    else
                    {
                        tracing.Trace(string.Concat("invoice record with ID ", e2.Id.ToString(), " does NOT have its pricing locked."));
                    }
                }
            }
            else
            {
                tracing.Trace("Entity is opportunityproduct or quotedetail, pricing will not be locked.");
            }
            return pricingLocked;
        }
    }
}
