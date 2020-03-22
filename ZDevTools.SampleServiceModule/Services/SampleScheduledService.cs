using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZDevTools.ServiceCore;
using System.Threading;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ServiceStack.Redis;


namespace ZDevTools.SampleServiceModule.Services
{
    [ServiceOrder(0)]
    public class SampleScheduledService : ScheduledServiceBase, IServiceRevokable
    {
        public SampleScheduledService(ILogger<SampleScheduledService> logger, IServiceProvider serviceProvider) : base(logger, serviceProvider)
        {

        }
        public override string DisplayName => "计划服务样例服务";

        bool cancelled;
        public void Cancel()
        {
            cancelled = true;
        }

        public override void ServiceCore()
        {
            cancelled = false;
            for (int i = 0; i < 3; i++)
            {
                Logger.LogInformation("我在执行！");
                Thread.Sleep(1000);
                if (cancelled)
                {
                    Logger.LogInformation("我被取消执行了！");
                    return;
                }
            }
            Logger.LogInformation("我执行完成了！");
            Logger.LogDebug("****调试信息****");
            Logger.LogWarning("*****警告信息*****");
        }
    }
}
