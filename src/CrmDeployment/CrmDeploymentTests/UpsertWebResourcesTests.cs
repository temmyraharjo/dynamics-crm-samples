using CrmDeployment;
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
    public class UpsertWebResourcesTests
    {
        [TestMethod]
        public void Can_create_new_webresource()
        {
            var context = new XrmFakedContext();
            context.Initialize(new List<Entity>());

            var wrapper = new DatabaseWrapper(context.GetOrganizationService());
            var config = new ConfigModel { WebResourceFilePath = Directory.GetCurrentDirectory() };

            new UpsertWebResources(wrapper, config).Execute();

            Assert.IsTrue(wrapper.CreatedEntities.Any());
        }

        [TestMethod]
        public void Can_update_new_webresource()
        {
            var context = new XrmFakedContext();
            var webResource = new WebResource { Id = Guid.NewGuid() }.Set(e => e.Name, "Sample.js").ToEntity<Entity>();

            context.Initialize(new List<Entity> { webResource });

            var wrapper = new DatabaseWrapper(context.GetOrganizationService());
            var config = new ConfigModel { WebResourceFilePath = Directory.GetCurrentDirectory() };

            new UpsertWebResources(wrapper, config).Execute();

            Assert.IsTrue(wrapper.UpdatedEntities.Any());
            Assert.AreEqual(webResource.Id, wrapper.UpdatedEntities[0].Id);
        }
    }
}
