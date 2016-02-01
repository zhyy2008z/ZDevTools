using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ZDevTools.ServiceCore
{
    /// <summary>
    /// 服务主机模式的服务基类，方便您快速完成主机模式服务的构建
    /// </summary>
    public abstract class HostedServiceBase : ServiceBase, IHostedService
    {
        static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(HostedServiceBase));
        void logError(string message, Exception exception) => log.Error($"【{ServiceName}】{message}", exception);

        Task jobTask;
        CancellationTokenSource source;
        ManualResetEvent manualResetEvent;

        public Task StartAsync()
        {
            source = new CancellationTokenSource();
            manualResetEvent = new ManualResetEvent(false);

            return Task.Run(() =>
            {
                ServiceInitialize();
                jobTask = Task.Run(new Action(job), source.Token);
            });
        }

        protected abstract void ServiceCore(CancellationToken token, ManualResetEvent waitHandle);

        protected virtual void ServiceInitialize() { }

        void job()
        {
            using (source)
            using (manualResetEvent)
            {
                try
                {
                    ReportStatus("状态：正在运行");
                    ServiceCore(source.Token, manualResetEvent);
                    ReportStatus("状态：停止运行");
                }
#if !DEBUG
                catch (Exception ex)
                {
                    logError($"运行出错，错误：{ex.Message}", ex);
                    ReportError(ex);
                }
#endif
                finally { }
            }
            source = null;
            manualResetEvent = null;
        }

        public Task StopAsync()
        {
            //取消请求
            if (source != null)
                source.Cancel();

            //立即唤醒可能正在沉睡的任务
            if (manualResetEvent != null)
                manualResetEvent.Set();

            return jobTask;
        }
    }
}
