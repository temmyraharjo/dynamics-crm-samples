using Entities;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Niam.XRM.Framework;
using Niam.XRM.Framework.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace CrmDeployment.Business
{
    public class UpdatePlugins
    {
        private readonly IOrganizationService _service;
        private readonly string _directory;

        public UpdatePlugins(IOrganizationService service, string directory)
        {
            _service = service;
            _directory = directory;
        }

        public void Execute()
        {
            var pluginInformations = GetPluginFilePaths()
                .Select(filePath =>
                    new { FilePath = filePath, PluginName = Path.GetFileName(filePath) })
                .ToArray();
            if (!pluginInformations.Any())
            {
                Console.WriteLine("No Plugin Assemblies found..");
                return;
            }

            var pluginAssemblies = GetPluginAssemblies(pluginInformations.Select(e => (object)e.PluginName).ToArray());

            foreach (var pluginAssembly in pluginAssemblies)
            {
                var pluginName = pluginAssembly.Get(e => e.Name);
                var information = pluginInformations.FirstOrDefault(e => e.PluginName == pluginName);
                if (information == null) continue;

                var bytes = File.ReadAllBytes(information.FilePath);
                var base64String = Convert.ToBase64String(bytes);
                var update = new PluginAssembly { Id = pluginAssembly.Id }.Set(e => e.Content, base64String);

                _service.Update(update);
                Console.WriteLine($"Success update {information.PluginName} assembly..");
            }

            Console.WriteLine($"Done update plugin assemblies..");
        }

        private IEnumerable<string> GetPluginFilePaths()
        {
            var files = Directory.GetFiles(_directory, "*.dll", SearchOption.AllDirectories);
            foreach (var file in files)
            {
                var filtered = Assembly.LoadFile(file).GetTypes()
                    .Where(t => t.IsClass)
                    .Where(t => !t.IsAbstract)
                    .Where(t => !t.IsGenericType)
                    .Where(t => typeof(IPlugin).IsAssignableFrom(t)).ToArray();

                if (!filtered.Any()) continue;

                Console.WriteLine($"Found Plugin {file}..");
                yield return file;
            }
        }

        private PluginAssembly[] GetPluginAssemblies(object[] pluginNames)
        {
            if (!pluginNames.Any()) return new PluginAssembly[] { };

            var query = new QueryExpression(PluginAssembly.EntityLogicalName)
            {
                ColumnSet = new ColumnSet<PluginAssembly>(e => e.Id, e => e.Name),
                TopCount = 1
            };
            query.Criteria.AddCondition<PluginAssembly>(e => e.Name, ConditionOperator.In, pluginNames);
            var result = _service.RetrieveMultiple(query);

            return result.Entities?.Select(e => e.ToEntity<PluginAssembly>()).ToArray();
        }
    }
}
