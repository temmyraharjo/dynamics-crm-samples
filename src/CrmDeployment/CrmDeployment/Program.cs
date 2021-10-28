using CommandLine;
using CrmDeployment.Business;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Tooling.Connector;
using System;

namespace CrmDeployment
{
    class Program
    {
        static void Main(string[] args)
        {
            Parser.Default.ParseArguments<CommandLineOptions>(args)
                .WithParsed(option => Run(option));
            Console.Read();
        }

        static void Run(CommandLineOptions option)
        {
            var client = new CrmServiceClient(option.ConnectionString);

            Console.WriteLine($"Connected to {client.ConnectedOrgFriendlyName}");

            switch (option.Type)
            {
                case "Update-WebResource":
                    {
                        new UpsertWebResources(client, option.Directory).Execute();

                        var publishAll = new PublishAllXmlRequest();
                        client.Execute(publishAll);

                        Console.WriteLine("Publish All finished");
                        break;
                    }

                case "Update-Plugin":
                    new UpdatePlugin(client, option.FilePath).Execute();
                    break;
                case "Update-Plugins":
                    new UpdatePlugins(client, option.Directory).Execute();
                    break;
                case "Export-Solutions":
                    {
                        var publishAll = new PublishAllXmlRequest();
                        client.Execute(publishAll);

                        Console.WriteLine("Publish All finished");

                        new SolutionsExporter(client, option.Directory, option.IsManaged.GetValueOrDefault()).Execute();
                        break;
                    }
                case "Import-Solutions":
                    {
                        new SolutionsImporter(client, option.Directory, option.IsToday.GetValueOrDefault()).Execute();
                        break;
                    }
            }
        }
    }
}