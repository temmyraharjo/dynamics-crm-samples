using Insurgo.Custom.Api.Business;
using Microsoft.Xrm.Sdk;
using Niam.XRM.Framework.Interfaces.Plugin;
using Niam.XRM.Framework.Plugin;

namespace Insurgo.Custom.Api
{
    public class PluginGetTeamsEmail : PluginBase, IPlugin
    {
        public PluginGetTeamsEmail(string unsecure, string secure) : base(unsecure, secure)
        {
        }

        protected override void ExecuteCrmPlugin(IPluginContext<Entity> context)
        {
            new GetTeamsEmail(context).Execute();
        }
    }
}