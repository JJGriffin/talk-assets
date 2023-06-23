using Microsoft.Xrm.Sdk;
using System;

namespace D365.CustomAPI.ProjOpsBudgetDemos
{
    public class ExecuteBudgetCustomAPIs : PluginBase
    {
        public ExecuteBudgetCustomAPIs(string unsecureConfiguration, string secureConfiguration)
            : base(typeof(ExecuteBudgetCustomAPIs))
        {}

        // Entry point for custom business logic execution
        protected override void ExecuteDataversePlugin(ILocalPluginContext localPluginContext)
        {
            if (localPluginContext == null)
            {
                throw new ArgumentNullException(nameof(localPluginContext));
            }

            var context = localPluginContext.PluginExecutionContext;

            //Get a reference to the Organization service.

            IOrganizationServiceFactory factory = (IOrganizationServiceFactory)localPluginContext.ServiceProvider.GetService(typeof(IOrganizationServiceFactory));
            IOrganizationService service = factory.CreateOrganizationService(context.UserId);

            //Declare and get the Custom API Request Parameter values

            string message = (string)context.InputParameters["Message"];
            localPluginContext.Trace($"Message: {message}");
            Guid projectID = (Guid)context.InputParameters["ProjectID"];
            localPluginContext.Trace($"Project ID: {projectID}");
            int targetState = (int)context.InputParameters["TargetState"];
            localPluginContext.Trace($"Target State: {targetState}");

            try
            {
                //The value of message will tell us what Custom API we need to call
                switch (message)
                {
                    case "msdyn_UpdateBudgetStatusForProject":
                        localPluginContext.Trace("Executing msdyn_UpdateBudgetStatusForProject Custom API...");
                        ExecuteUpdateBudgetStatus(projectID, targetState, service, localPluginContext);
                        break;
                    default:
                        throw new InvalidPluginExecutionException($"Message of type {message} is not compatible with this custom API.");
                }
            }
            catch (Exception ex)
            {
                localPluginContext.Trace("ExecuteBudgetCustomAPIs: {0}", ex.ToString());
                throw new InvalidPluginExecutionException(ex.Message, ex);
            }
        }

        private void ExecuteUpdateBudgetStatus(Guid projectID, int targetState, IOrganizationService service, ILocalPluginContext localPluginContext)
        {
            //Declare the Request Parameters required by the msdyn_UpdateBudgetStatusForProject Custom API
            ParameterCollection parameters = new ParameterCollection
            {
                { "msdyn_projectId", projectID },
                { "msdyn_targetState", targetState }
            };

            //Build the Request
            OrganizationRequest request = new OrganizationRequest()
            {
                RequestName = "msdyn_UpdateBudgetStatusForProject",
                Parameters = parameters
            };

            //Actually send the request
            localPluginContext.Trace("Sending msdyn_UpdateBudgetStatusForProject request...");
            service.Execute(request);
            localPluginContext.Trace("msdyn_UpdateBudgetStatusForProject request completed successfully!");
        }
    }
}