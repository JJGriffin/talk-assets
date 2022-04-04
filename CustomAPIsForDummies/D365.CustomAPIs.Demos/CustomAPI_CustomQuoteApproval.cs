using System;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Query;

namespace D365.CustomAPIs.Demos
{
    public class CustomAPI_CustomQuoteApproval : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            //Obtain the execution context from the service provider.

            IPluginExecutionContext context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));

            //Extract the tracing service for use in debugging sandboxed plug-ins
            ITracingService tracer = (ITracingService)serviceProvider.GetService(typeof(ITracingService));
            tracer.Trace("Tracing implemented successfully!");

            if (context.MessageName.Equals("duff_AccountsApproval") && context.Stage.Equals(30))
            {
                try
                {
                    Guid quoteID = (Guid)context.InputParameters["QuoteID"];
                    bool? isApproved = (bool)context.InputParameters["IsApproved"];
                    string reason = (string)context.InputParameters["Reason"];

                    //Only proceed if we have all the expected input parameters

                    if (quoteID != null && isApproved != null && reason != null)
                    {
                        tracer.Trace("Quote ID = {0}", quoteID.ToString());
                        tracer.Trace("Approved = {0}", isApproved.ToString());
                        tracer.Trace("Reason = {0}", reason);

                        //Get a reference to the Organization service.

                        IOrganizationServiceFactory factory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
                        IOrganizationService service = factory.CreateOrganizationService(context.UserId);

                        //Get the Quote record we want - Quote Number, Quote ID, Opportunity ID, Total Amount, Name, Owner

                        Entity quote = service.Retrieve("quote", quoteID, new ColumnSet("quotenumber", "opportunityid", "totalamount", "name", "ownerid"));
                        tracer.Trace("Got Quote with ID {0}", quoteID);

                        //We execute everything in a transaction, so we can rollback cleanly on failure

                        ExecuteTransactionRequest transactionRequest = new ExecuteTransactionRequest()
                        {
                            Requests = new OrganizationRequestCollection(),
                            ReturnResponses = true
                        };
                        ExecuteTransactionResponse transactionResponse;

                        //Determine whether it's an approval or rejection

                        if (isApproved == true)
                        {
                            //Add first request to update Quote with approval details

                            quote["duff_accountteamnotes"] = reason;

                            UpdateRequest quoteUpdateRequest = new UpdateRequest()
                            {
                                Target = quote
                            };

                            transactionRequest.Requests.Add(quoteUpdateRequest);

                            //Build the WinQuote and ConvertQuoteToSalesOrder requests to get the quote converted successfully

                            Entity qc = new Entity("quoteclose");
                            qc["subject"] = string.Format("Quote Won (Won) - {0}", quote.GetAttributeValue<string>("quotenumber"));
                            qc["quoteid"] = new EntityReference("quote", quoteID);
                            qc["quotenumber"] = quote.GetAttributeValue<string>("quotenumber");
                            qc["statuscode"] = new OptionSetValue(2);

                            WinQuoteRequest winQuoteRequest = new WinQuoteRequest()
                            {
                                QuoteClose = qc,
                                Status = new OptionSetValue(4)
                            };

                            transactionRequest.Requests.Add(winQuoteRequest);

                            ConvertQuoteToSalesOrderRequest convertQuoteRequest = new ConvertQuoteToSalesOrderRequest()
                            {
                                QuoteId = quoteID,
                                ColumnSet = new ColumnSet("salesorderid") //Returns Order ID only, which is all we need
                            };

                            transactionRequest.Requests.Add(convertQuoteRequest);

                            //If an Opportunity is linked, then we need to build and add a WinOpportunity request

                            if (quote.GetAttributeValue<EntityReference>("opportunityid") != null)
                            {
                                tracer.Trace("Opportunity with ID {0} linked to Quote, closing this as well...", quote.GetAttributeValue<EntityReference>("opportunityid").Id.ToString());
                                Entity oc = new Entity("opportunityclose");
                                oc["opportunityid"] = quote.GetAttributeValue<EntityReference>("opportunityid");
                                oc["actualrevenue"] = quote.GetAttributeValue<Money>("totalamount");
                                oc["actualend"] = DateTime.UtcNow;
                                oc["description"] = string.Format("Automatically closed as won on approval of Quote {0}", quote.GetAttributeValue<string>("name"));

                                WinOpportunityRequest winOppRequest = new WinOpportunityRequest
                                {
                                    OpportunityClose = oc,
                                    Status = new OptionSetValue(-1)
                                };

                                transactionRequest.Requests.Add(winOppRequest);
                            }
                            else
                            {
                                tracer.Trace("No Opportunity linked to Quote, continuing...");
                            }

                            //Execute the complete transaction.
                            transactionResponse = (ExecuteTransactionResponse)service.Execute(transactionRequest);
                            tracer.Trace("Quote approved successfully!");

                            //Get the newly created Order details

                            Entity order = (Entity)transactionResponse.Responses[2].Results["Entity"];

                            //Return the Order ID as an output parameter
                            context.OutputParameters["EntityID"] = new Guid(order.Id.ToString());
                        }
                        else
                        {
                            //For rejection, we update the Quote Status to kick off the email notification via Flow
                            //We close the Quote via an update and perform a revision in this case, as that's technically what we should be doing because simply updating the Status / Status Reason returns an error
                            tracer.Trace("Quote with ID {0} has been rejected, creating Quote revision...", quoteID.ToString());

                            Entity updQuote = new Entity("quote", quoteID);

                            updQuote["duff_accountteamnotes"] = reason;
                            updQuote["statecode"] = new OptionSetValue(3);
                            updQuote["statuscode"] = new OptionSetValue(6);

                            UpdateRequest quoteUpdateRequest = new UpdateRequest()
                            {
                                Target = updQuote
                            };

                            transactionRequest.Requests.Add(quoteUpdateRequest);

                            ReviseQuoteRequest reviseQuoteRequest = new ReviseQuoteRequest()
                            {
                                QuoteId = quoteID,
                                ColumnSet = new ColumnSet("quoteid")
                            };

                            //Execute the complete transaction.
                            transactionRequest.Requests.Add(reviseQuoteRequest);

                            //Execute the complete transaction.
                            transactionResponse = (ExecuteTransactionResponse)service.Execute(transactionRequest);

                            //Get and then update the revised quote with the rejection reason
                            tracer.Trace("Transaction completed successfully!");
                            Entity revisedQuote = (Entity)transactionResponse.Responses[1].Results["Entity"];
                            tracer.Trace("Quote revision with ID {0} generated successfully, adding rejection notes...", revisedQuote.Id.ToString());
                            revisedQuote["duff_accountteamnotes"] = reason;
                            service.Update(revisedQuote);
                            tracer.Trace("Revised Quote with ID {0} updated successfully!", revisedQuote.Id.ToString());

                            //Return the Quote ID as an output parameter
                            context.OutputParameters["EntityID"] = new Guid(revisedQuote.Id.ToString());
                        }
                    }
                    else
                    {
                        tracer.Trace("Required input parameters are missing. Cancelling plug-in execution");
                    }
                }
                catch (Exception ex)
                {
                    tracer.Trace("CustomAPI_AccountsApproval: {0}", ex.ToString());
                    throw new InvalidPluginExecutionException(ex.Message, ex);
                }
            }
            else
            {
                tracer.Trace("CustomAPI_AccountsApproval plug-in is not associated with the expected message or is not registered for the main operation.");
            }
        }
    }
}
