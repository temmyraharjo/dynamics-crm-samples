using System;
using System.Text.RegularExpressions;
using System.Xml;

namespace Insurgo.Custom.Api.Extensions
{
    public static class StringExtensions
    {
        public static string GetTemplateContent(this string text)
        {
            var valid = text.Contains("xml") && text.Contains("xsl:template match");
            if (!valid) return text;

            var document = new XmlDocument();
            document.LoadXml(text);

            var templates = document.GetElementsByTagName("xsl:template");
            return templates.Count > 0 ? templates[0].InnerText: "";
        }

        public static string StripHtml(this string text)
        {
            return Regex.Replace(text, "<.*?>", String.Empty);
        }
    }
}
