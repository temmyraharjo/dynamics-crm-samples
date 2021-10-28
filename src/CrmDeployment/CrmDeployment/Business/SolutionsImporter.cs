using Microsoft.Xrm.Sdk;
using Niam.XRM.Framework;
using System;
using System.IO;
using System.Linq;

namespace CrmDeployment.Business
{
    public class SolutionsImporter
    {
        private readonly IOrganizationService _service;
        private readonly string _directory;
        private readonly bool _isToday;

        public SolutionsImporter(IOrganizationService service, string directory, bool isToday)
        {
            _service = service;
            _directory = directory;
            _isToday = isToday;
        }

        public void Execute()
        {
            try
            {
                if (!Directory.Exists(_directory))
                {
                    Console.WriteLine($"Create directory {_directory}..");
                    Directory.CreateDirectory(_directory);
                }

                var files = Directory.GetFiles(_directory, "*.zip", SearchOption.AllDirectories);
                foreach (var file in files)
                {
                    var fileInfo = new FileInfo(file);
                    if (_isToday && fileInfo.LastWriteTime.ToShortDateString() != DateTime.Now.ToShortDateString())
                    {
                        Console.WriteLine($"File {file} last modified date is {fileInfo.LastWriteTime.ToShortDateString()}.");
                        continue;
                    }

                    var fileBytes = File.ReadAllBytes(file);
                    var importRequest = new Microsoft.Crm.Sdk.Messages.ImportSolutionRequest
                    {
                        CustomizationFile = fileBytes
                    };
                    var start = DateTime.Now;
                    var fileName = Path.GetFileName(file);
                    Console.WriteLine($"Import solution {fileName} at {start}..");

                    _service.Execute<Microsoft.Crm.Sdk.Messages.ImportSolutionResponse>(importRequest);
                    var end = DateTime.Now;

                    Console.WriteLine($"Import solution {fileName} start: {start} end: {end} total minutes: {(end - start).TotalMinutes}..");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                throw;
            }
        }
    }
}
