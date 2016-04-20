using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZDevTools.ServiceCore
{
    /// <summary>
    /// 计划服务基类
    /// </summary>
    public abstract class ScheduledServiceBase : ServiceBase, IScheduledService
    {
        static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(ScheduledServiceBase));
        void logError(string message, Exception exception) => log.Error($"【{DisplayName}】{message}", exception);

        /// <summary>
        /// 执行额外信息
        /// </summary>
        protected ExecutionExtraInfo ExecutionExtraInfo { get; set; }

        /// <summary>
        /// 执行本次服务
        /// </summary>
        /// <returns>服务是否执行成功</returns>
        public bool Run()
        {
            try
            {
                ServiceCore();

                if (ExecutionExtraInfo != null)
                    ReportStatus(ExecutionExtraInfo);
                else
                    ReportStatus("执行成功");

                return true;
            }
#if !DEBUG
            catch (Exception ex)
            {
                logError($"执行出错，错误：{ex.Message}", ex);
                ReportError("执行出错", ex);
                return false;
            }
#endif
            finally { }
        }

        /// <summary>
        /// 服务核心代码
        /// </summary>
        public abstract void ServiceCore();
    }
}
