using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using log4net;
using ZDevTools.ServiceCore;

namespace ZDevTools.ServiceSample.Services
{
    [ExportService(2)]
    public class SampleHostedService : HostedServiceBase
    {
        static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(SampleHostedService));

        public override string DisplayName => "承载服务样例服务";

        protected override ILog Log => log;

        protected override int ServiceCore(CancellationToken cancelationToken)
        {
            LogInfo("我在运行");

            return 3000;
        }
    }
}
