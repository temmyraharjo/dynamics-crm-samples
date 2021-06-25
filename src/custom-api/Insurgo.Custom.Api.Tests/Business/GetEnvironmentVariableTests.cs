using Insurgo.Custom.Entities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Xrm.Sdk;
using Niam.XRM.Framework;
using Niam.XRM.Framework.TestHelper;
using System;

namespace Insurgo.Custom.Api.Tests.Business
{
    [TestClass]
    public class GetEnvironmentVariableTests
    {
        [TestMethod]
        public void GetEnvironmentVariable_EnvironmentVariableDefinition_ShouldValid()
        {
            var environmentVariable = new EnvironmentVariableDefinition { Id = Guid.NewGuid() }.
                Set(e => e.SchemaName, "TEST").
                Set(e => e.DefaultValue, "VALUE");

            var testContext = new TestEvent<Entity>(environmentVariable);
            testContext.PluginExecutionContext.InputParameters[Api.Business.GetEnvironmentVariable.InputParameter] = "TEST";

            testContext.CreateEventCommand<Api.Business.GetEnvironmentVariable>(null);

            Assert.AreEqual("VALUE", testContext.PluginExecutionContext.OutputParameters[Api.Business.GetEnvironmentVariable.OutputParameter]);
        }

        [TestMethod]
        public void GetEnvironmentVariable_EnvironmentVariableValue_ShouldValid()
        {
            var environmentVariable = new EnvironmentVariableDefinition {Id = Guid.NewGuid()}
                .Set(e => e.SchemaName, "TEST")
                .Set(e => e.DefaultValue, "VALUE");

            var environmentValue = new EnvironmentVariableValue { Id = Guid.NewGuid() }
                .Set(e => e.EnvironmentVariableDefinitionId, environmentVariable.ToEntityReference())
                .Set(e => e.SchemaName, "CHILD")
                .Set(e => e.Value, "CHILD-VALUE");

            var testContext = new TestEvent<Entity>(environmentVariable, environmentValue);
            testContext.PluginExecutionContext.InputParameters[Api.Business.GetEnvironmentVariable.InputParameter] = "TEST";

            testContext.CreateEventCommand<Api.Business.GetEnvironmentVariable>(null);

            Assert.AreEqual("CHILD-VALUE", testContext.PluginExecutionContext.OutputParameters[Api.Business.GetEnvironmentVariable.OutputParameter]);
        }
    }
}
