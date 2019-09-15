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

                    //TODO: Insert custom pricing logic here
                    //As part of this, you can either: 
                    //Apply the same custom pricing logic to all sales entities (as this example will do)
                    //Use a switch statement to process custom logic for each entity individually e.g. for Opportunity:

                    //switch (entity.LogicalName)
                    //{
                    //case "opportunity":
                    //    DoSomethingHere();
                    //    return;
                    //}

                    //List of available entities as part of this:
                    //opportunity, opportunityproduct, quote, quotedetail, salesorder, salesorderdetail, invoice, invoicedetail

                    //For further details, please refer to the following articles:
                    //https://docs.microsoft.com/en-us/dynamics365/customer-engagement/developer/use-custom-pricing-products
                    //https://docs.microsoft.com/en-us/dynamics365/customer-engagement/developer/sample-calculate-price-plugin

                    


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
            return new Money(0);
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
    }
}
