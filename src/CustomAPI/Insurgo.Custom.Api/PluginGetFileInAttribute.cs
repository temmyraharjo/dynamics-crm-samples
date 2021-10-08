using Insurgo.Custom.Api.Business;
using Microsoft.Xrm.Sdk;
using Niam.XRM.Framework.Interfaces.Plugin;
using Niam.XRM.Framework.Plugin;
using System;

namespace Insurgo.Custom.Api
{
    public class PluginGetFileInAttribute : PluginBase, IPlugin
    {
        public PluginGetFileInAttribute(string unsecure, string secure) : base(unsecure, secure)
        {
        }

        protected override void ExecuteCrmPlugin(IPluginContext<Entity> context)
        {
            new GetFileInAttribute(context).Execute();
        }
    }
}