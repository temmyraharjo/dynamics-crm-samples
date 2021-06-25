using Insurgo.Custom.Api.Business;
using Insurgo.Custom.Entities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Xrm.Sdk;
using Niam.XRM.Framework;
using Niam.XRM.Framework.TestHelper;
using System;

namespace Insurgo.Custom.Api.Tests.Business
{
    [TestClass]
    public class GetTeamsEmailTests
    {
        [TestMethod]
        public void GetTeamsEmail_PrimaryEmail()
        {
            var team = new Team { Id = Guid.NewGuid() }
                .Set(e => e.Name, "TEAM1")
                .Set(e => e.TeamType, Team.Options.TeamType.Owner);

            var user1 = new SystemUser { Id = Guid.NewGuid() }
                .Set(e => e.InternalEMailAddress, "user1@me.com")
                .Set(e => e.IsDisabled, false);

            var teamMembership1 = new TeamMembership { Id = Guid.NewGuid() }
                .Set(e => e.TeamId, team.Id)
                .Set(e => e.SystemUserId, user1.Id);

            var user2 = new SystemUser { Id = Guid.NewGuid() }
                .Set(e => e.InternalEMailAddress, "user2@me.com")
                .Set(e => e.IsDisabled, false);

            var teamMembership2 = new TeamMembership { Id = Guid.NewGuid() }
                .Set(e => e.TeamId, team.Id)
                .Set(e => e.SystemUserId, user2.Id);

            var testEvent = new TestEvent(team, user1, user2, teamMembership1, teamMembership2);

            testEvent.PluginExecutionContext.InputParameters[GetTeamsEmail.IsEmailKey] = true;
            testEvent.PluginExecutionContext.InputParameters[GetTeamsEmail.TeamNameKey] = "TEAM1";

            testEvent.CreateEventCommand<GetTeamsEmail>(new Entity());

            var result = testEvent.PluginExecutionContext.OutputParameters[GetTeamsEmail.OutputParameter] as string[];
            Assert.IsNotNull(result);
            Assert.AreEqual("user1@me.com", result[0]);
            Assert.AreEqual("user2@me.com", result[1]);
        }

        [TestMethod]
        public void GetTeamsEmail_Guids()
        {
            var team = new Team { Id = Guid.NewGuid() }
                .Set(e => e.Name, "TEAM1")
                .Set(e => e.TeamType, Team.Options.TeamType.Owner);

            var user1 = new SystemUser { Id = Guid.NewGuid() }
                .Set(e => e.InternalEMailAddress, "user1@me.com")
                .Set(e => e.IsDisabled, false);

            var teamMembership1 = new TeamMembership { Id = Guid.NewGuid() }
                .Set(e => e.TeamId, team.Id)
                .Set(e => e.SystemUserId, user1.Id);

            var user2 = new SystemUser { Id = Guid.NewGuid() }
                .Set(e => e.InternalEMailAddress, "user2@me.com")
                .Set(e => e.IsDisabled, false);

            var teamMembership2 = new TeamMembership { Id = Guid.NewGuid() }
                .Set(e => e.TeamId, team.Id)
                .Set(e => e.SystemUserId, user2.Id);

            var testEvent = new TestEvent(team, user1, user2, teamMembership1, teamMembership2);

            testEvent.PluginExecutionContext.InputParameters[GetTeamsEmail.IsEmailKey] = false;
            testEvent.PluginExecutionContext.InputParameters[GetTeamsEmail.TeamNameKey] = "TEAM1";

            testEvent.CreateEventCommand<GetTeamsEmail>(new Entity());

            var result = testEvent.PluginExecutionContext.OutputParameters[GetTeamsEmail.OutputParameter] as string[];
            Assert.IsNotNull(result);
            Assert.AreEqual(user1.Id.ToString(), result[0]);
            Assert.AreEqual(user2.Id.ToString(), result[1]);
        }
    }
}
