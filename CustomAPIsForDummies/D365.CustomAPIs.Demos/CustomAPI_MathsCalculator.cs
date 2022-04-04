using System;
using Microsoft.Xrm.Sdk;
using System.Linq;

namespace D365.CustomAPIs.Demos
{
    public class CustomAPI_MathsCalculator : IPlugin
    {
        public string[] supportedOperations = new string[] {"plus", "minus", "divide", "multiply"};
        public void Execute(IServiceProvider serviceProvider)
        {
            //Obtain the execution context from the service provider.

            IPluginExecutionContext context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));

            //Extract the tracing service for use in debugging sandboxed plug-ins
            ITracingService tracer = (ITracingService)serviceProvider.GetService(typeof(ITracingService));
            tracer.Trace("Tracing implemented successfully!");

            if (context.MessageName.Equals("duff_MathsCalculator") && context.Stage.Equals(30))
            {
                try
                {
                    //All InputParameters are set to mandatory
                    string operation = (string)context.InputParameters["Operator"];
                    decimal firstValue = (decimal)context.InputParameters["FirstValue"];
                    decimal secondValue = (decimal)context.InputParameters["SecondValue"];

                    tracer.Trace($"Operator = {operation}");
                    tracer.Trace($"First Value = {firstValue}");
                    tracer.Trace($"Second Value = {secondValue}");
                    
                    //Check that we have a valid operator
                    if (supportedOperations.Contains(operation))
                    {
                        tracer.Trace("Operator is valid, continuing...");
                           
                        decimal result = 0;
                        
                        switch (operation)
                        {
                            case "plus":
                                result = decimal.Add(firstValue, secondValue);
                                break;
                            case "minus":
                                result = decimal.Subtract(firstValue, secondValue);
                                break;
                            case "divide":
                                if (secondValue == 0)
                                    throw new Exception("You sneaky Pete! Dividing by zero is not allowed.");
                                result = decimal.Divide(firstValue, secondValue);
                                break;
                            case "multiply":
                                result = decimal.Multiply(firstValue, secondValue);
                                break;
                            default:
                                //Should never be here. So best to error if so.
                                throw new Exception("Unreachable code detected, glavin!");
                        }
                        
                        //Return the result as an Output Parameter
                        tracer.Trace($"Result = {result}");
                        context.OutputParameters["Result"] = result;
                    }
                    else
                    {
                        tracer.Trace($"Invalid operator supplied to the API. Expected +, -, / or *, actually got {operation}");
                        throw new Exception($"Looks like the operator {operation} isn't supported - D'oh!");
                    }         
                }
                catch (Exception ex)
                {
                    tracer.Trace("CustomAPI_MathsCalculator: {0}", ex.ToString());
                    throw new InvalidPluginExecutionException(ex.Message, ex);
                }
            }
            else
            {
                tracer.Trace("CustomAPI_MathsCalculator plug-in is not associated with the expected message or is not registered for the main operation.");
            }
        }
    }
}