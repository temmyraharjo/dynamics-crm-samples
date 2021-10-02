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
            if (!args.Any())
            {
                Console.WriteLine("No arguments received..");
                return;
            }

            var jsonString = File.ReadAllText($"{AppDomain.CurrentDomain.BaseDirectory}/config.json");
            var config = JsonConvert.DeserializeObject<ConfigModel>(jsonString);

            var client = new CrmServiceClient(config.ConnectionString);

            Console.WriteLine($"Connected to {client.ConnectedOrgFriendlyName}");

            if (args[0] == ("WebResource"))
            {
                new UpsertWebResources(client, config).Execute();

                var publishAll = new PublishAllXmlRequest();
                client.Execute(publishAll);

                Console.WriteLine("Publish All finished");
            }
            else if (args[0] == "Plugin")
            {
                var filePath = string.Join(" ", args.Skip(1)).Trim();
                new UpdatePlugin(client, filePath).Execute();
            }
            else if (args[0] == "Plugins")
            {
                var directory = string.Join(" ", args.Skip(1)).Trim();
                new UpdatePlugins(client, directory).Execute();
            }
            else if (args[0] == "Solutions")
            {
                var publishAll = new PublishAllXmlRequest();
                client.Execute(publishAll);

                Console.WriteLine("Publish All finished");

                var isManaged = bool.Parse(args[1]);
                var directoryText = string.Join("", args.Skip(2))
                    .Trim();


                new SolutionsExporter(client, directoryText, isManaged).Execute();
            }

            Console.WriteLine("Done..");
        }
    }
}
