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

        /// <summary>
        /// 当服务执行失败时发生
        /// </summary>
        public event EventHandler Faulted;

        /// <summary>
        /// 开始服务
        /// </summary>
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

        /// <summary>
        /// 需要重复执行的服务核心代码
        /// </summary>
        /// <param name="cancelationToken">是否取消，状态获取令牌</param>
        /// <returns>返回要让服务休息的毫秒数，小于等于0就不休息</returns>
        protected abstract int ServiceCore(CancellationToken cancelationToken);

        /// <summary>
        /// 服务初始化操作
        /// </summary>
        protected virtual void ServiceInitialize() { }

        void job()
        {
            using (source)
            using (manualResetEvent)
            {
                try
                {
                    ReportStatus("状态：正在运行");
                    while (true)
                    {
                        if (source.Token.IsCancellationRequested)
                            break;

                        int millisecondsTimeout = ServiceCore(source.Token);

                        //让服务歇一会儿
                        if (millisecondsTimeout > 0)
                            manualResetEvent.WaitOne(millisecondsTimeout);
                    }
                    ReportStatus("状态：停止运行");
                }
                catch (OperationCanceledException) { ReportStatus("状态：停止运行"); } //捕获掉取消操作异常，当作停止运行来处理
#if !DEBUG
                catch (Exception ex)
                {
                    logError($"承载服务失败：{ex.Message}", ex);
                    ReportError("状态：已停止，承载服务失败", ex);
                    if (Faulted != null)
                        Faulted(this, EventArgs.Empty);
                }
#endif
                finally { }
            }
            source = null;
            manualResetEvent = null;
        }

        /// <summary>
        /// 停止服务
        /// </summary>
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
