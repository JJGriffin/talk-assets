using Microsoft.Xrm.Tooling.Connector;
using System.Configuration;

namespace ScheduleAPIConsoleDemo
{
    internal class Program
    {
        static void Main(string[] args)
        {
            //Initialise values from config to connect to D365
            //TODO: Add in a local.config file with an App Setting defined for your own environment.
            //Ensure this is always copied to the build output folder
            string url = ConfigurationManager.AppSettings.Get("d365URL");
            string conn = $@"
                AuthType=OAuth;
                url={url};
                LoginPrompt=Auto;
                ClientId=51f81489-12ee-4a9e-aaae-a2591f45987d;
                RedirectUri=app://58145B91-0C36-4500-8554-080854F2AC97";

            using (var svc = new CrmServiceClient(conn))
            {
                Demos.Execute(svc);
            }
        }
    }
}