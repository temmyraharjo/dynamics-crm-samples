using System;
using Insurgo.Custom.Entities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Niam.XRM.Framework;
using Niam.XRM.Framework.TestHelper;

namespace Insurgo.Custom.Api.Tests.Business
{
    [TestClass]
    public class GetEmailContentTests
    {
        [TestMethod]
        public void GetEmailContentTests_ShouldValid()
        {
            var systemUser = new SystemUser { Id = Guid.NewGuid() }
                .Set(e => e.FirstName, "User")
                .Set(e => e.LastName, "001");

            var account = new Account { Id = Guid.NewGuid() }
                .Set(e => e.Name, "Account 001")
                .Set(e => e.CreatedBy, systemUser.ToEntityReference());

            var contact = new Contact { Id = Guid.NewGuid() }
                .Set(e => e.FirstName, "FirstName")
                .Set(e => e.LastName, "LastName")
                .Set(e => e.Address1_Line1, "Address 1 Line 1")
                .Set(e => e.ParentCustomerId, account.ToEntityReference())
                .Set(e => e.Telephone1, "0123456789")
                .Set(e => e.BirthDate, new DateTime(1990, 01, 17))
                .Set(e => e.Address1_AddressTypeCode, Contact.Options.Address1_AddressTypeCode.Primary);

            var emailTemplate = new Template { Id = Guid.NewGuid() }
                .Set(e => e.Title, "Template 001")
                .Set(e => e.Subject, "Subject {firstname} {lastname}")
                .Set(e => e.Body, "Hi {account1.name} {address1_line1} {telephone1} {birthdate:dd-MM-yyyy} {systemuser1.firstname} {systemuser1.lastname}");

            var fetchXml = $@"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
                              <entity name='contact'>
                                <attribute name='firstname' />
                                <attribute name='lastname' />
                                <attribute name='address1_line1' />
                                <attribute name='telephone1' />
                                <attribute name='contactid' />
                                <attribute name='birthdate' />
                                <attribute name='parentcustomerid' />
                                <filter type='and'>
                                  <condition attribute='contactid' operator='eq' value='{contact.Id}' />
                                </filter>
                                <link-entity name='account' from='accountid' to='parentcustomerid'>
                                  <attribute name='createdby' />
                                  <attribute name='name' />
                                  <link-entity name='systemuser' from='systemuserid' to='createdby'>
                                    <attribute name='firstname' />
                                    <attribute name='lastname' />
                                  </link-entity>
                                </link-entity>
                              </entity>
                            </fetch>";


            var testContext = new TestEvent(systemUser, account, contact, emailTemplate);
            testContext.PluginExecutionContext.InputParameters[Api.Business.GetEmailContent.FetchXmlParameter] =
                fetchXml;
            testContext.PluginExecutionContext.InputParameters[Api.Business.GetEmailContent.TemplateIdParameter] =
                emailTemplate.Id;

            testContext.CreateEventCommand<Api.Business.GetEmailContent>(null);

            var result = testContext.PluginExecutionContext
                .OutputParameters[Api.Business.GetEmailContent.OutputParameter].ToString()
                .FromJson<Api.Business.GetEmailContent.GetEmailContentResult>();

            Assert.AreEqual("Subject FirstName LastName", result.Subject);
            Assert.AreEqual("Hi Account 001 Address 1 Line 1 0123456789 17-01-1990 User 001", result.Body);
        }

        [TestMethod]
        public void GetEmailContentTests_Childs_ShouldValid()
        {
            var account = new Account { Id = Guid.NewGuid() }
                .Set(e => e.Name, "Account 001");

            var contact1 = new Contact { Id = Guid.NewGuid() }
                .Set(e => e.FirstName, "Contact")
                .Set(e => e.LastName, "One")
                .Set(e => e.ParentCustomerId, account.ToEntityReference());

            var contact2 = new Contact { Id = Guid.NewGuid() }
                .Set(e => e.FirstName, "Contact")
                .Set(e => e.LastName, "Two")
                .Set(e => e.ParentCustomerId, account.ToEntityReference());

            var emailTemplate = new Template { Id = Guid.NewGuid() }
                .Set(e => e.Title, "Template 001")
                .Set(e => e.Subject, "Subject {name}")
                .Set(e => e.Body, "<list>{ab.firstname} {ab.lastname}</list>");

            var fetchXml = $@"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='true'>
                              <entity name='account'>
                                <attribute name='name' />
                                <attribute name='primarycontactid' />
                                <attribute name='telephone1' />
                                <attribute name='accountid' />
                                <order attribute='name' descending='false' />
                                <filter type='and'>
                                  <condition attribute='accountid' operator='eq' value='{account.Id}' />
                                </filter>
                                <link-entity name='contact' from='parentcustomerid' to='accountid' link-type='inner' alias='ab'>
                                  <attribute name='firstname' />
                                  <attribute name='lastname' />
                                </link-entity>
                              </entity>
                            </fetch>";


            var testContext = new TestEvent( account, contact1, contact2, emailTemplate);
            testContext.PluginExecutionContext.InputParameters[Api.Business.GetEmailContent.FetchXmlParameter] =
                fetchXml;
            testContext.PluginExecutionContext.InputParameters[Api.Business.GetEmailContent.TemplateIdParameter] =
                emailTemplate.Id;

            testContext.CreateEventCommand<Api.Business.GetEmailContent>(null);

            var result = testContext.PluginExecutionContext
                .OutputParameters[Api.Business.GetEmailContent.OutputParameter].ToString()
                .FromJson<Api.Business.GetEmailContent.GetEmailContentResult>();

            Assert.AreEqual("Subject Account 001", result.Subject);
            Assert.AreEqual("Contact One Contact Two", result.Body);
        }
    }
}
