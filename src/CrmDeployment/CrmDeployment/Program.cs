using CrmDeployment.Business;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Tooling.Connector;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Linq;

namespace CrmDeployment
{
    class Program
    {
        static void Main(string[] args)
        {
            var jsonString = File.ReadAllText($"{AppDomain.CurrentDomain.BaseDirectory}/config.json");
            var config = JsonConvert.DeserializeObject<ConfigModel>(jsonString);

            var client = new CrmServiceClient(config.ConnectionString);

            Console.WriteLine($"Connected to {client.ConnectedOrgFriendlyName}");

            if (args.Any() && args.Contains("WebResource"))
            {
                new UpsertWebResources(client, config).Execute();

                var publishAll = new PublishAllXmlRequest();
                client.Execute(publishAll);

                Console.WriteLine("Publish All finished");
            }
            else if (args.Any() && args.Contains("Plugin"))
            {
                var filePath = string.Join(" ", args.Skip(1)).Trim();
                new UpdatePlugin(client, filePath).Execute();
            }

            Console.WriteLine("Done Deployment..");
        }
    }
}
