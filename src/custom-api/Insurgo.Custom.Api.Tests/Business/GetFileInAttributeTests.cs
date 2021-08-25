using Microsoft.Crm.Sdk.Messages;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Xrm.Sdk;
using Niam.XRM.Framework.Interfaces.Plugin;
using NSubstitute;
using System;
using System.Text;

namespace Insurgo.Custom.Api.Tests.Business
{
    [TestClass]
    public class GetFileInAttributeTests
    {
        [TestMethod]
        public void GetFileInAttribute_NoFileFound_ReturnEmptyResponse()
        {
            var transactionContext = Substitute.For<ITransactionContext<Entity>>();

            var pluginContext = Substitute.For<IPluginExecutionContext>();
            var parameterCollection = new ParameterCollection
            {
                [Api.Business.GetFileInAttribute.InputEntityLogicalName] = "account",
                [Api.Business.GetFileInAttribute.InputEntityGuid] = Guid.NewGuid(),
                [Api.Business.GetFileInAttribute.InputFileAttributeName] = "new_file"
            };
            pluginContext.InputParameters.Returns(parameterCollection);

            transactionContext.PluginExecutionContext.Returns(pluginContext);

            var organizationService = Substitute.For<IOrganizationService>();
            organizationService.Execute(Arg.Any<InitializeFileBlocksDownloadRequest>()).Returns(x =>
            {
                throw new Exception("No file attachment found for attribute: new_file EntityId: 2e19aa2f-5505-ec11-b6e6-00224816ca54.");
            });
            transactionContext.Service.Returns(organizationService);

            new Api.Business.GetFileInAttribute(transactionContext).Execute();

            Assert.IsNull(pluginContext.OutputParameters);
        }


        [TestMethod]
        public void GetFileInAttribute_NoFileFound_ReturnResponse()
        {
            var transactionContext = Substitute.For<ITransactionContext<Entity>>();

            var pluginContext = Substitute.For<IPluginExecutionContext>();
            var parameterCollection = new ParameterCollection
            {
                [Api.Business.GetFileInAttribute.InputEntityLogicalName] = "account",
                [Api.Business.GetFileInAttribute.InputEntityGuid] = Guid.NewGuid(),
                [Api.Business.GetFileInAttribute.InputFileAttributeName] = "new_file"
            };
            pluginContext.InputParameters.Returns(parameterCollection);
            pluginContext.OutputParameters.Returns(new ParameterCollection());

            transactionContext.PluginExecutionContext.Returns(pluginContext);

            var organizationService = Substitute.For<IOrganizationService>();
            organizationService.Execute(Arg.Any<InitializeFileBlocksDownloadRequest>()).Returns(x =>
            {
                return new InitializeFileBlocksDownloadResponse
                {
                    ["FileContinuationToken"] = "token",
                    ["FileName"] = "test.txt",
                    ["FileSizeInBytes"] = 10l
                };
            });

            organizationService.Execute(Arg.Any<DownloadBlockRequest>()).Returns(x =>
            {
                return new DownloadBlockResponse
                {
                    ["Data"] = Encoding.ASCII.GetBytes("Hello world")
                };
            });
            transactionContext.Service.Returns(organizationService);

            new Api.Business.GetFileInAttribute(transactionContext).Execute();

            Assert.IsNotNull(pluginContext.OutputParameters);
            Assert.AreEqual("test.txt", pluginContext.OutputParameters[Api.Business.GetFileInAttribute.OutputFileName]);
            Assert.IsNotNull(pluginContext.OutputParameters[Api.Business.GetFileInAttribute.OutputFileContent]);
        }
    }
}
