using System;
using System.Globalization;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xrm.Sdk;
using System.ServiceModel;
using Microsoft.Xrm.Sdk.Query;

namespace D365CE.CustomPricingTalk.Start
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

                    //TODO: Insert custom pricing logic here to be applied for Quote Line records.
                    //This should ideally mirror any functionality specified earlier.
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
    }
}
