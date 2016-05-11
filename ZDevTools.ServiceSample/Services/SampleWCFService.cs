using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using log4net;
using ZDevTools.ServiceCore;

namespace ZDevTools.ServiceSample.Services
{
    [ExportService(3)]
    public class SampleWCFService : WCFServiceBase, ISampleWCFService
    {

        static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(SampleWCFService));

        public override string DisplayName => "WCF服务样例服务";

        protected override ILog Log => log;

        public string TestApi()
        {
            return "测试消息";
        }
    }
}
