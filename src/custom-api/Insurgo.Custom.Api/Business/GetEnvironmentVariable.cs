using Insurgo.Custom.Entities;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Niam.XRM.Framework;
using Niam.XRM.Framework.Data;
using Niam.XRM.Framework.Interfaces.Plugin;
using Niam.XRM.Framework.Plugin;
using System.Collections.Generic;
using System.Linq;

namespace Insurgo.Custom.Api.Business
{
    public class GetEnvironmentVariable : OperationBase<Entity>
    {
        public const string InputParameter = "EnvironmentVariableName";
        public const string OutputParameter = "Result";

        public class JsonValueModel
        {
            public string Name { get; set; }
            public string Value { get; set; }
        }

        public GetEnvironmentVariable(ITransactionContext<Entity> context) : base(context)
        {
        }

        protected override void HandleExecute()
        {
            var environmentVariableName =
                Context.PluginExecutionContext.InputParameters.ContainsKey(InputParameter)
                    ? Context.PluginExecutionContext.InputParameters[InputParameter].ToString()
                    : "";

            if (string.IsNullOrEmpty(environmentVariableName))
            {
                throw new InvalidPluginExecutionException($"{InputParameter} is empty!");
            }

            var data = GetData(environmentVariableName);
            var result = GetModel(data).Where(e => !string.IsNullOrEmpty(e.Value)).ToArray();
            var row = result.LastOrDefault() ?? new JsonValueModel();

            if (string.IsNullOrEmpty(row.Value))
            {
                throw new InvalidPluginExecutionException("Value is null!");
            }

            Context.PluginExecutionContext.OutputParameters[OutputParameter] = row.Value;
        }

        private IEnumerable<JsonValueModel> GetModel(EnvironmentVariableDefinition[] data)
        {
            if (data.Any())
            {
                var parent = data.FirstOrDefault();
                yield return new JsonValueModel
                {
                    Name = parent.Get(e => e.SchemaName),
                    Value = parent.Get(e => e.DefaultValue)
                };
            }

            foreach (var datum in data)
            {
                var child = datum.GetAliasedEntity<EnvironmentVariableValue>("ev");
                yield return new JsonValueModel
                {
                    Name = child.Get(e => e.SchemaName),
                    Value = child.Get(e => e.Value)
                };
            }
        }

        private EnvironmentVariableDefinition[] GetData(string environmentVariableName)
        {
            var query = new QueryExpression(EnvironmentVariableDefinition.EntityLogicalName)
            {
                ColumnSet = new ColumnSet<EnvironmentVariableDefinition>(e => e.DefaultValue, e => e.SchemaName)
            };
            query.Criteria.AddCondition<EnvironmentVariableDefinition>(e => e.SchemaName, ConditionOperator.Equal, environmentVariableName);

            var childLink =
                query.AddLink<EnvironmentVariableDefinition, EnvironmentVariableValue>(e => e.Id,
                    e => e.EnvironmentVariableDefinitionId, JoinOperator.LeftOuter);
            childLink.EntityAlias = "ev";
            childLink.Columns = new ColumnSet<EnvironmentVariableValue>(e => e.SchemaName, e => e.Value);

            var result = Service.RetrieveMultiple(query);

            var data = result.Entities.Any()
                ? result.Entities.Select(e => e.ToEntity<EnvironmentVariableDefinition>()).ToArray()
                : new EnvironmentVariableDefinition[] { };

            return data;
        }
    }
}
