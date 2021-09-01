using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Niam.XRM.Framework.Interfaces.Plugin;
using Niam.XRM.Framework.Plugin;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Insurgo.Custom.Api.Business
{
    public class GetFileInAttribute : OperationBase<Entity>
    {
        public const string InputEntityLogicalName = "EntityLogicalName";
        public const string InputEntityGuid = "EntityGuid";
        public const string InputFileAttributeName = "FileAttributeName";

        public const string OutputFileName = "FileName";
        public const string OutputFileContent = "FileContent";

        public GetFileInAttribute(ITransactionContext<Entity> context) : base(context)
        {
        }

        protected override void HandleExecute()
        {
            var fileInformation = GetFileContinuationToken();
            if (string.IsNullOrEmpty(fileInformation.FileContinuationToken)) return;

            var req = new DownloadBlockRequest { FileContinuationToken = fileInformation.FileContinuationToken, BlockLength = fileInformation.FileSizeInBytes };
            var result = (DownloadBlockResponse)Service.Execute(req);

            Context.PluginExecutionContext.OutputParameters[OutputFileName] = fileInformation.FileName;
            Context.PluginExecutionContext.OutputParameters[OutputFileContent] = Convert.ToBase64String(result.Data);
        }

        private InitializeFileBlocksDownloadResponse GetFileContinuationToken()
        {
            var entityLogicalName = Context.PluginExecutionContext.InputParameters[InputEntityLogicalName].ToString();
            var entityId = new Guid(Context.PluginExecutionContext.InputParameters[InputEntityGuid].ToString());
            var attributeName = Context.PluginExecutionContext.InputParameters[InputFileAttributeName].ToString();

            var fileAttributes = GetFileAttributes(attributeName).ToArray();

            var data = Service.Retrieve(entityLogicalName, entityId, new ColumnSet(true));

            if (!fileAttributes.Any(expectedAttribute => data.Contains(expectedAttribute)))
            {
                return new InitializeFileBlocksDownloadResponse();
            }

            var initializeFile = new InitializeFileBlocksDownloadRequest
            {
                FileAttributeName = attributeName,
                Target = new EntityReference(entityLogicalName, entityId)
            };

            return (InitializeFileBlocksDownloadResponse)Service.Execute(initializeFile);
        }

        private IEnumerable<string> GetFileAttributes(string attributeName)
        {
            yield return attributeName; // For File
            yield return $"{attributeName}id"; // For Image
        }
    }
}
