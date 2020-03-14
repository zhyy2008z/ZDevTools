using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace ZDevTools.ServiceCore
{
    public interface IServiceModule
    {
        void ConfigureServices(HostBuilderContext hostBuilderContext, IServiceCollection serviceCollection);

    }
}
