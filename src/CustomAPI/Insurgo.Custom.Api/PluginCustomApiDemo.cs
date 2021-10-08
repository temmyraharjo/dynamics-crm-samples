using Microsoft.Xrm.Sdk;
using Niam.XRM.Framework;
using Niam.XRM.Framework.Interfaces.Plugin;
using Niam.XRM.Framework.Plugin;

namespace Insurgo.Custom.Api
{
    public class PluginCustomApiDemo : PluginBase, IPlugin
    {
        public PluginCustomApiDemo(string unsecure, string secure) : base(unsecure, secure)
        {
        }

        protected override void ExecuteCrmPlugin(IPluginContext<Entity> context)
        {
            var param = context.PluginExecutionContext.InputParameters["name"].ToString();
            var document = new Entity("new_document");
            document.Set("new_documentnumber", "Create from Custom API: " + param);

            context.Service.Create(document);
        }
    }
}
