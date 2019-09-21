using System;
using System.Globalization;
using System.ServiceModel;
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
                && context.ParentContext.ParentContext != null
                && context.ParentContext.ParentContext.ParentContext.SharedVariables.ContainsKey("CustomPrice")
                && (bool)context.ParentContext.ParentContext.ParentContext.SharedVariables["CustomPrice"])
                return;

            // The InputParameters collection contains all the data passed in the message request.            
            if (context.InputParameters.Contains("Target") &&
                context.InputParameters["Target"] is EntityReference)
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
        }
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
                //Check to see if the product is being sold below cost price - if so, throw an error

                decimal cp;
                decimal sp;

                //Obtain the correct Discount and Freight Amounts for the product record

                Money da = CalculateLineItemDiscount(e1, service, tracing);
                Money fa = CalculateLineItemFreight(e1, service, tracing);

            }

        }
        private static void CalculateSalesDocument(EntityReference salesDoc, IOrganizationService service, ITracingService tracing)
        {

        }
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
                da = new Money(ba.Value - (ba.Value * dpDec / 100));
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
                        da = new Money(ba.Value - (ba.Value * dpDec / 100));
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

            string city = psd.GetAttributeValue<string>("shipto_city0");

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
        private static string GetProductEntityName(string entity)
        {
            string pe = "";

            //Return the corresponding child Sales entity line entity name and vice-versa.
            //For example, passing "opportunityproduct" to this method would return "opportunity"

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
        private static string GetProductEntityIDName(string entity)
        {
            string pe = "";

            //Return the corresponding parent Sales entity line parent lookup field name.
            //For example, passing "opportunityproduct" to this method would return "opportunityid"

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
