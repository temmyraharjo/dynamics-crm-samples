using System;
using System.Collections.Generic;
using System.Linq;
using Insurgo.Custom.Api.Extensions;
using Insurgo.Custom.Entities;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Niam.XRM.Framework;
using Niam.XRM.Framework.Data;
using Niam.XRM.Framework.Interfaces.Plugin;
using Niam.XRM.Framework.Plugin;

namespace Insurgo.Custom.Api.Business
{
    public class GetEmailContent : OperationBase
    {
        public const string FetchXmlParameter = "FetchXmlParameter";
        public const string TemplateIdParameter = "TemplateIdParameter";

        public const string OutputParameter = "OutputParameter";

        public class GetEmailContentResult
        {
            public string Subject { get; set; }
            public string Body { get; set; }
        }

        public GetEmailContent(ITransactionContext<Entity> context) : base(context)
        {
        }

        protected override void HandleExecute()
        {
            var fetchXml = Context.PluginExecutionContext.InputParameters[FetchXmlParameter].ToString();
            if (string.IsNullOrEmpty(fetchXml)) throw new ArgumentNullException(nameof(FetchXmlParameter));

            var templateId = Context.PluginExecutionContext.InputParameters[TemplateIdParameter].ToString();
            if (string.IsNullOrEmpty(templateId)) throw new ArgumentNullException(nameof(TemplateIdParameter));

            var data = Service.RetrieveMultiple(new FetchExpression(fetchXml));
            var currentTemplate = Service.Retrieve(Template.EntityLogicalName, new Guid(templateId),
                new ColumnSet<Template>(e => e.Subject, e => e.Body))
                .ToEntity<Template>();

            var mainEntity = data.Entities.FirstOrDefault();
            if (mainEntity == null)
            {
                Context.PluginExecutionContext.OutputParameters[OutputParameter] = new GetEmailContentResult().ToJson();
                return;
            }

            var subject = ReplaceText(currentTemplate.Get(e => e.Subject).GetTemplateContent(), 
                mainEntity);

            var bodyText = ProcessList(currentTemplate.Get(e => e.Body).GetTemplateContent(), 
                data.Entities.ToArray());
            var body = ReplaceText(bodyText, mainEntity);

            Context.PluginExecutionContext.OutputParameters[OutputParameter] = new GetEmailContentResult
            {
                Body = body,
                Subject = subject.StripHtml()
            }.ToJson();
        }

        private static string ProcessList(string text, Entity[] entities)
        {
            var currentIndex = text.IndexOf("<list>", StringComparison.Ordinal);
            var lastIndex = text.IndexOf("</list>", StringComparison.Ordinal);

            var valid = currentIndex > -1 && lastIndex > -1;
            if (!valid) return text;

            currentIndex += 6;
            var originalText = text.Substring(currentIndex, lastIndex - currentIndex);
            var result = new List<string>();
            foreach (var entity in entities)
            {
                var tempText = string.Copy(originalText);
                result.Add(ReplaceText(tempText, entity));
            }

            return text.Replace(originalText, string.Join(" ", result))
                .Replace("<list>", "")
                .Replace("</list>", "");
        }

        private static string ReplaceText(string text, Entity source)
        {
            foreach (var attribute in source.Attributes)
            {
                var findText = "{" + attribute.Key;
                var index = text.IndexOf(findText, StringComparison.Ordinal);
                if (index == -1) continue;

                var closeIndex = FindCloseIndex(text, index + findText.Length);
                if (closeIndex == -1) continue;

                var key = text.Substring(index, closeIndex - index);
                var formatString = GetFormatString(key);
                var value = source.FormattedValues.Contains(key) ? source.FormattedValues[key] :
                            (attribute.Value is AliasedValue aliasedValue ? ToString(aliasedValue.Value, formatString) : ToString(attribute.Value, formatString));

                text = text.Replace(key, value);
            }

            return text;
        }

        private static string ToString(object value, string format)
        {
            return string.IsNullOrEmpty(format) ? value.ToString() : string.Format("{0:" + format + "}", value);
        }

        private static string GetFormatString(string key)
        {
            var list = key.Replace("}", "").Split(':').ToArray();
            return list.Length > 1 ? list.LastOrDefault() : string.Empty;
        }

        private static int FindCloseIndex(string text, int index)
        {
            for (var i = index; i < text.Length; i++)
            {
                var current = text[i];
                if (current == '}') return i + 1;
            }

            return -1;
        }
    }
}
