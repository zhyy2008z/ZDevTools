using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ZDevTools.ServiceCore;
using ZDevTools.SampleServiceModule.Services;

namespace ZDevTools.SampleServiceModule
{
    public class ServiceModule : IServiceModule
    {
        public void ConfigureServices(HostBuilderContext hostBuilderContext, IServiceCollection serviceCollection)
        {
            serviceCollection.AddSingleton<IServiceBase, SampleGRpcService>();
            serviceCollection.AddSingleton<IServiceBase, SampleHostedService>();
            serviceCollection.AddSingleton<IServiceBase, SampleScheduledService>();
            serviceCollection.AddSingleton<IServiceBase, SampleWindowsService>();

            serviceCollection.AddTransient<SampleGRpcCommunication>();
        }
    }
}
