using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using log4net;

namespace ZDevTools.ServiceCore
{
    /// <summary>
    /// 专为承载WCF服务创建的服务基类
    /// </summary>
    public abstract class WCFServiceBase : ServiceBase, IHostedService
    {
        ServiceHost serviceHost;

        public Task StartAsync()
        {
            return Task.Run(() =>
            {
                serviceHost = new ServiceHost(this.GetType());
                serviceHost.Open();
            });
        }

        public Task StopAsync()
        {
            return Task.Run(() =>
            {
                serviceHost.Close();
            });
        }
    }
}
