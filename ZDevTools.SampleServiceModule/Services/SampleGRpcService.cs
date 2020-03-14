using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZDevTools.ServiceCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ServiceStack.Redis;

namespace ZDevTools.SampleServiceModule.Services
{


    [ServiceOrder(3)]
    public class SampleGRpcService : GRpcServiceBase
    {
        public SampleGRpcService(ILogger<SampleGRpcService> logger, IServiceProvider serviceProvider, SampleGRpcCommunication sampleGRpcCommunication)
            : base(SampleService.BindService(sampleGRpcCommunication), 8886, logger, serviceProvider)
        {

        }

        public override string DisplayName => "WCF服务样例服务";
    }
}
