using Entities;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Niam.XRM.Framework;
using System;
using System.IO;
using System.Linq;

namespace CrmDeployment.Business
{
    public class UpdatePlugin
    {
        private readonly IOrganizationService _service;
        private readonly string _filePath;

        public UpdatePlugin(IOrganizationService service, string filePath)
        {
            _service = service;
            _filePath = filePath;
        }

        public void Execute()
        {
            var fileName = Path.GetFileNameWithoutExtension(_filePath);
            var result = GetPluginAssembly(fileName);

            if (result.Id == Guid.Empty)
            {
                Console.WriteLine($"Assembly {fileName} not found!");
                return;
            }

            var bytes = File.ReadAllBytes(_filePath);
            var base64String = Convert.ToBase64String(bytes);
            var update = new PluginAssembly { Id = result.Id }.Set(e => e.Content, base64String);

            _service.Update(update);
            Console.WriteLine($"Success update {fileName} assembly..");
        }

        private PluginAssembly GetPluginAssembly(string fileName)
        {
            var query = new QueryExpression(PluginAssembly.EntityLogicalName)
            {
                ColumnSet = new ColumnSet(false),
                TopCount = 1
            };
            query.Criteria.AddCondition<Entities.PluginAssembly>(e => e.Name, ConditionOperator.Equal, fileName);
            return
             (_service.RetrieveMultiple(query).Entities.FirstOrDefault() ??
              new Entity { LogicalName = PluginAssembly.EntityLogicalName }).ToEntity<PluginAssembly>();
        }
    }
}
