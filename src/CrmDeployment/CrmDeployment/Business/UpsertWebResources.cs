using System;
using System.IO;
using System.Linq;
using Entities;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Niam.XRM.Framework;
using Niam.XRM.Framework.Data;

namespace CrmDeployment.Business
{
    public class UpsertWebResources
    {
        private readonly IOrganizationService _service;
        private readonly string _directory;

        public UpsertWebResources(IOrganizationService service, string directory)
        {
            _service = service;
            _directory = directory;
        }

        public void Execute()
        {
            var files = Directory.GetFiles(_directory).Where(e => e.ToLower().EndsWith(".js"))
                .Select(e => new { FilePath = e, Name = Path.GetFileName(e) }).ToArray();
            if (!files.Any()) return;

            var webResources = GetWebResources(files.Select(e => (object) e.Name).ToArray());

            foreach (var file in files)
            {
                var bytes = File.ReadAllBytes(file.FilePath);
                var base64String = Convert.ToBase64String(bytes);
                var webResource = webResources.FirstOrDefault(wb => wb.Get(e => e.Name) == file.Name);

                if (webResource == null)
                {
                    webResource = new WebResource().Set(e => e.Name, file.Name).Set(e => e.DisplayName, file.Name)
                        .Set(e => e.WebResourceType, WebResource.Options.WebResourceType.ScriptJScript)
                        .Set(e => e.Content, base64String);

                    _service.Create(webResource);
                    Console.WriteLine($"Success Created {file.Name}..");
                    continue;
                }

                var update = new WebResource { Id = webResource.Id }.Set(e => e.Content, base64String);
                _service.Update(update);
                Console.WriteLine($"Success Updated {file.Name}..");
            }
        }

        private WebResource[] GetWebResources(object[] fileNames)
        {
            var query = new QueryExpression(WebResource.EntityLogicalName)
            {
                ColumnSet = new ColumnSet<WebResource>(e => e.Name)
            };
            query.Criteria.AddCondition<WebResource>(e => e.Name, ConditionOperator.In,
                fileNames);

            return _service.RetrieveMultiple(query).Entities.Select(e => e.ToEntity<WebResource>()).ToArray();
        }
    }
}
