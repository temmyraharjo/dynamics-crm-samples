using Insurgo.Custom.Entities;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Niam.XRM.Framework;
using Niam.XRM.Framework.Data;
using Niam.XRM.Framework.Interfaces.Plugin;
using Niam.XRM.Framework.Plugin;
using System.Linq;

namespace Insurgo.Custom.Api.Business
{
    public class GetTeamsEmail : OperationBase
    {
        public const string IsEmailKey = "IsEmail";
        public const string TeamNameKey = "TeamName";

        public const string OutputParameter = "Result";

        public GetTeamsEmail(ITransactionContext<Entity> context) : base(context)
        {
        }

        protected override void HandleExecute()
        {
            var teamName = Context.PluginExecutionContext.InputParameters[TeamNameKey].ToString();
            var isEmail = bool.Parse(Context.PluginExecutionContext.InputParameters[IsEmailKey].ToString());

            if (string.IsNullOrEmpty(teamName))
            {
                throw new InvalidPluginExecutionException($"{TeamNameKey} is empty.");
            }

            var users = GetUsers(teamName);

            var result = users.Select(user => isEmail ? user.Get(e => e.InternalEMailAddress) : user.Id.ToString())
                .ToArray();
            Context.PluginExecutionContext.OutputParameters[OutputParameter] = result;
        }

        private SystemUser[] GetUsers(string teamName)
        {
            var query = new QueryExpression(SystemUser.EntityLogicalName)
            {
                ColumnSet = new ColumnSet<SystemUser>(e => e.Id, e => e.InternalEMailAddress)
            };
            query.Criteria.AddCondition<SystemUser>(e => e.IsDisabled, ConditionOperator.Equal, false);
            query.Criteria.AddCondition<SystemUser>(e => e.InternalEMailAddress, ConditionOperator.NotNull);

            var tmLink = query.AddLink<SystemUser, TeamMembership>(e => e.Id, e => e.SystemUserId);
            tmLink.Columns = new ColumnSet(false);
            tmLink.EntityAlias = "tm";

            var teamLink = tmLink.AddLink<TeamMembership, Team>(e => e.TeamId, e => e.Id);
            teamLink.Columns = new ColumnSet(false);
            teamLink.LinkCriteria.AddCondition<Team>(e => e.Name, ConditionOperator.Equal, teamName);

            var result = Service.RetrieveMultiple(query);

            return result.Entities?.Select(e => e.ToEntity<SystemUser>()).ToArray();
        }
    }
}
