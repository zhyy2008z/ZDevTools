using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZDevTools.ServiceCore;
using System.Threading;

namespace ZDevTools.ServiceSample.Services
{
    [ExportService(0)]
    public class SampleScheduledService : ScheduledServiceBase, IServiceRevokable
    {
        static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(SampleScheduledService));

        protected override log4net.ILog Log => log;

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
                LogInfo("我在执行！");
                Thread.Sleep(1000);
                if (cancelled)
                {
                    LogInfo("我被取消执行了！");
                    return;
                }
            }
            LogInfo("我执行完成了！");
            LogDebug("****调试信息****");
            LogWarn("*****警告信息*****");
        }
    }
}
