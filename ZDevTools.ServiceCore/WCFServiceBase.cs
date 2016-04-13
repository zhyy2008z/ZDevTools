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
        static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(WCFServiceBase));

        void logError(string message) => log.Error($"【{ServiceName}】{message}");


        ServiceHost serviceHost;

        /// <summary>
        /// 当服务执行失败时发生
        /// </summary>
        public event EventHandler Faulted;

        /// <summary>
        /// 开始服务
        /// </summary>
        public Task StartAsync()
        {
            return Task.Run(() =>
            {
                serviceHost = new ServiceHost(this.GetType());
                serviceHost.Open();
                serviceHost.Faulted += serviceHost_Faulted;
                ReportStatus("状态：正在运行");
            });
        }

        private void serviceHost_Faulted(object sender, EventArgs e)
        {
            serviceHost.Close();
            logError("WCF服务失败");
            ReportError("状态：已停止，WCF服务失败");
            if (Faulted != null)
                Faulted(this, EventArgs.Empty);
        }

        /// <summary>
        /// 停止服务
        /// </summary>
        public Task StopAsync()
        {
            return Task.Run(() =>
            {
                serviceHost.Close();
                ReportStatus("状态：停止运行");
            });
        }
    }
}
