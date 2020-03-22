using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ZDevTools.ServiceCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ServiceStack.Redis;

namespace ZDevTools.SampleServiceModule.Services
{
    [ServiceOrder(2)]
    public class SampleHostedService : HostedServiceBase
    {
        public SampleHostedService(ILogger<SampleHostedService> logger, IServiceProvider serviceProvider) : base(logger, serviceProvider)
        {

        }
        public override string DisplayName => "承载服务样例服务";

        protected override int ServiceCore(CancellationToken cancelationToken)
        {
            Logger.LogInformation("我在运行");

            return 3000;
        }
    }
}
