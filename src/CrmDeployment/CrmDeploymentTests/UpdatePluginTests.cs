using CrmDeployment.Business;
using Entities;
using FakeXrmEasy;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Xrm.Sdk;
using Niam.XRM.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace CrmDeploymentTests
{
    [TestClass]
    public class UpdatePluginTests
    {
        [TestMethod]
        public void Can_update_plugin_assembly()
        {
            var context = new XrmFakedContext();
            var pluginAssembly = new PluginAssembly() { Id = Guid.NewGuid() }.Set(e => e.Name, "Sample").ToEntity<Entity>();

            context.Initialize(new List<Entity> { pluginAssembly });

            var wrapper = new DatabaseWrapper(context.GetOrganizationService());

            new UpdatePlugin(wrapper, Directory.GetCurrentDirectory() + "//Sample.js").Execute();

            Assert.IsTrue(wrapper.UpdatedEntities.Any());
            Assert.AreEqual(pluginAssembly.Id, wrapper.UpdatedEntities[0].Id);
        }
    }
}
