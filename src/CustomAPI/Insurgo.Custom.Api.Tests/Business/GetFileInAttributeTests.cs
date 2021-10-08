using Microsoft.Crm.Sdk.Messages;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Niam.XRM.Framework;
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
            organizationService.Retrieve(Arg.Any<string>(), Arg.Any<Guid>(), 
                Arg.Any<ColumnSet>()).Returns(new Entity());

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

            organizationService.Retrieve(Arg.Any<string>(), Arg.Any<Guid>(),
               Arg.Any<ColumnSet>()).Returns(new Entity().Set("new_file", "FILE001"));

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
