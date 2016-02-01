using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZDevTools.ServiceCore
{
    public abstract class ScheduledServiceBase : ServiceBase, IScheduledService
    {
        static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(ScheduledServiceBase));
        void logError(string message, Exception exception) => log.Error($"【{ServiceName}】{message}", exception);

        protected ExecutionExtraInfo ExecutionExtraInfo { get; set; }

        public void Run()
        {
            try
            {
                ServiceCore();

                if (ExecutionExtraInfo != null)
                    ReportStatus(ExecutionExtraInfo);
                else
                    ReportStatus();
            }
#if !DEBUG
            catch (Exception ex)
            {
                logError($"执行出错，错误：{ex.Message}", ex);
                ReportError(ex);
            }
#endif
            finally { }
        }

        public abstract void ServiceCore();
    }
}
