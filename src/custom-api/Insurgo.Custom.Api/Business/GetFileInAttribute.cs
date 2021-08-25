using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Niam.XRM.Framework.Interfaces.Plugin;
using Niam.XRM.Framework.Plugin;
using System;

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

            try
            {
                var initializeFile = new InitializeFileBlocksDownloadRequest
                {
                    FileAttributeName = "new_file",
                    Target = new EntityReference(entityLogicalName, entityId)
                };

                return (InitializeFileBlocksDownloadResponse)Service.Execute(initializeFile);
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("No file attachment found for attribute:")) return new InitializeFileBlocksDownloadResponse();
                throw;
            }
        }
    }
}
