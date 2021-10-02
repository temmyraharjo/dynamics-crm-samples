using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Niam.XRM.Framework;
using Niam.XRM.Framework.Data;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace CrmDeployment.Business
{
    public class SolutionsExporter
    {
        private readonly IOrganizationService _service;
        private readonly string _directory;
        private readonly bool _isManaged;

        public SolutionsExporter(IOrganizationService service, string directory, bool isManaged)
        {
            _service = service;
            _directory = directory;
            _isManaged = isManaged;
        }

        public void Execute()
        {
            var solutions = GetSolutions();

            var solutionDirectory = _directory + "\\Export Solutions";
            if (!Directory.Exists(solutionDirectory))
            {
                Console.WriteLine($"Create directory {solutionDirectory}..");
                Directory.CreateDirectory(solutionDirectory);
            }

            foreach (var solution in solutions)
            {
                var solutionName = solution.Get(e => e.UniqueName);
                var exportRequest = new Microsoft.Crm.Sdk.Messages.ExportSolutionRequest
                {
                    SolutionName = solutionName,
                    Managed = _isManaged
                };
                var start = DateTime.Now;
                Console.WriteLine($"Run export solution {solutionName} at {start}..");

                var result = _service.Execute<Microsoft.Crm.Sdk.Messages.ExportSolutionResponse>(exportRequest);
                var end = DateTime.Now;

                Console.WriteLine($"Run export solution {solutionName} start: {start} end: {end} total minutes: {(end - start).TotalMinutes}..");

                var filePath = solutionDirectory + "\\" + solutionName + (_isManaged ? "_Managed" : "_Unmanaged") + ".zip";
                File.WriteAllBytes(filePath, result.ExportSolutionFile);
                Console.WriteLine($"Save solution at {filePath}");
            }
        }

        private Entities.Solution[] GetSolutions()
        {
            var descriptionLike = DateTime.Now.ToString("dd-MM-yyyy");
            Console.WriteLine($"Find CRM Solutions with description like {descriptionLike}");
            var query = new QueryExpression(Entities.Solution.EntityLogicalName)
            {
                ColumnSet = new ColumnSet<Entities.Solution>(e => e.Id, e => e.UniqueName),
                NoLock = true
            };
            query.Criteria.AddCondition<Entities.Solution>(e => e.Description, ConditionOperator.Like, $"%{descriptionLike}%");

            var result = _service.RetrieveMultiple(query);

            return result.Entities?.Select(e => e.ToEntity<Entities.Solution>()).ToArray();
        }
    }
}
