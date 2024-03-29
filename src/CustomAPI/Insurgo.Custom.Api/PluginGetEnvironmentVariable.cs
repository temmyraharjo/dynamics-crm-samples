﻿using Insurgo.Custom.Api.Business;
using Microsoft.Xrm.Sdk;
using Niam.XRM.Framework.Interfaces.Plugin;
using Niam.XRM.Framework.Plugin;
using System;

namespace Insurgo.Custom.Api
{
    public class PluginGetEnvironmentVariable : PluginBase, IPlugin
    {
        public PluginGetEnvironmentVariable(string unsecure, string secure) : base(unsecure, secure)
        {
        }

        protected override void ExecuteCrmPlugin(IPluginContext<Entity> context)
        {
            new GetEnvironmentVariable(context).Execute();
        }
    }
}
