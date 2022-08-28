using System.IO;
using Insurgo.Custom.Api.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Insurgo.Custom.Api.Tests.Extensions
{
    [TestClass]
    public class StringExtensionsTests
    {
        [TestMethod]
        public void StringExtensions_GetTemplateContent_ShouldValid()
        {
            var xml = File.ReadAllText(Directory.GetCurrentDirectory() + "//Extensions//SampleTemplateSubject.xml");

            var result = xml.GetTemplateContent();
            Assert.IsFalse(string.IsNullOrEmpty(result));
        }
    }
}
