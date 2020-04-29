using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServiceStack.Redis;
using System.Diagnostics;
using System.IO;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.DependencyInjection;
using System.Text.Json;

namespace ZDevTools.ServiceCore
{
    /// <summary>
    /// 服务基础抽象类
    /// </summary>
    public abstract class ServiceBase : IServiceBase
    {
        /// <summary>
        /// 日志对象
        /// </summary>
        protected readonly ILogger Logger;
        /// <summary>
        /// 服务提供者
        /// </summary>
        protected readonly IServiceProvider ServiceProvider;

        /// <summary>
        /// 服务基类构造函数
        /// </summary>
        protected ServiceBase(ILogger logger, IServiceProvider serviceProvider)
        {
            this.ServiceProvider = serviceProvider;
            this.ServiceName = this.GetType().Name; //设置服务名称
            this.Logger = logger;
            this.RedisManagerPool = serviceProvider.GetService<RedisManagerPool>();
        }

        /// <summary>
        /// 服务报告文件夹
        /// </summary>
        const string ServiceReportsFolder = "reports";

        /// <summary>
        /// 日志存放位置
        /// </summary>
        const string LogsFolder = "logs";

        /// <summary>
        /// RedisManagerPool
        /// </summary>
        public RedisManagerPool RedisManagerPool { get; }

        /// <summary>
        /// 服务显示名称
        /// </summary>
        public abstract string DisplayName { get; }

        /// <summary>
        /// 服务內部名称（其实就是类名）
        /// </summary>
        public string ServiceName { get; }

        /// <summary>
        /// 扔出较为严重的错误，该错误发生后任务彻底终止
        /// </summary>
        /// <param name="message">错误消息</param>
        [DebuggerNonUserCode]
        public void ThrowError(string message)
        {
            throw new ServiceErrorException(message);
        }

        /// <summary>
        /// 扔出较为严重的错误，该错误发生后任务彻底终止
        /// </summary>
        /// <param name="message">错误消息</param>
        /// <param name="innerException">內部异常</param>
        [DebuggerNonUserCode]
        public void ThrowError(Exception innerException, string message)
        {
            throw new ServiceErrorException(message, innerException);
        }

        /// <summary>
        /// 获取日志文件夹路径（确保日志文件夹存在）
        /// </summary>
        /// <returns></returns>
        public static string GetLogsFolder()
        {
            string logsFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, LogsFolder);
            if (!Directory.Exists(logsFolder))
                Directory.CreateDirectory(logsFolder);
            return logsFolder;
        }

        /// <summary>
        /// 获取报告文件夹路径（确保报告文件夹存在）
        /// </summary>
        /// <returns></returns>
        public static string GetReportsFolder()
        {
            string logsFolder = GetLogsFolder();

            string reportsFolder = Path.Combine(logsFolder, ServiceReportsFolder);
            if (!Directory.Exists(reportsFolder))
                Directory.CreateDirectory(reportsFolder);

            return reportsFolder;
        }

        #region 已导出
        void reportStatus(ServiceReport serviceReport)
        {
            try
            {
                serviceReport.ServiceName = DisplayName;
                serviceReport.UpdateTime = DateTime.Now;

                writeReport(serviceReport);

                if (RedisManagerPool != null)
                    using (var client = RedisManagerPool.GetClient())
                    {
                        client.SetEntryInHash(RedisKeys.ServiceReports, ServiceName, JsonSerializer.Serialize(serviceReport));
                        client.PublishMessage(RedisKeys.ServiceReports, ServiceName);
                    }
            }
            catch (Exception ex) { Logger.LogWarning(ex, "状态汇报出错，错误：" + ex.Message); }
        }

        FileStream _reportStream;
        StreamWriter _reportStreamWriter;
        static readonly object ReportLocker = new object();
        /// <summary>
        /// 写入报告（该方法允许多线程调用）
        /// </summary>
        /// <param name="serviceReport">服务报告</param>
        void writeReport(ServiceReport serviceReport)
        {
            lock (ReportLocker)
            {
                if (_reportStream == null)
                {
                    string reportsFolder = GetReportsFolder();

                    string reportFullName = Path.Combine(reportsFolder, ServiceName + ".log");

                    bool fileExists = File.Exists(reportFullName);

                    _reportStream = File.Open(reportFullName, FileMode.Append, FileAccess.Write, FileShare.Read);
                    _reportStreamWriter = new StreamWriter(_reportStream);

                    if (!fileExists) //write log header
                        _reportStreamWriter.WriteLine("时间\t\t\t错误\t消息\t\t\t消息组");
                }

                _reportStreamWriter.WriteLine($"{serviceReport.UpdateTime}\t{(serviceReport.HasError ? "[有]" : "[无]")}\t{serviceReport.Message}\t{(serviceReport.MessageArray == null ? null : string.Join("、", serviceReport.MessageArray))}");

                _reportStreamWriter.Flush();
            }
        }

        /// <summary>
        /// 报告服务状态
        /// </summary>
        /// <param name="message"></param>
        public void ReportStatus(string message)
        {
            ServiceReport report = new ServiceReport();

            report.Message = message;

            reportStatus(report);
        }

        /// <summary>
        /// 报告服务执行状态及额外信息
        /// </summary>
        /// <param name="executionExtraInfo"></param>
        public void ReportStatus(ExecutionExtraInfo executionExtraInfo)
        {
            ServiceReport report = new ServiceReport();

            if (executionExtraInfo != null)
            {
                report.Message = executionExtraInfo.WarnMessage;
                report.MessageArray = executionExtraInfo.WarnMessageArray;
            }

            reportStatus(report);
        }

        /// <summary>
        /// 报告服务错误
        /// </summary>
        /// <param name="message"></param>
        public void ReportError(string message)
        {
            ServiceReport report = new ServiceReport();

            report.HasError = true;
            report.Message = message;

            reportStatus(report);
        }

        /// <summary>
        /// 报告服务错误
        /// </summary>
        /// <param name="message"></param>
        /// <param name="exception"></param>
        public void ReportError(Exception exception, string message)
        {
            ServiceReport report = new ServiceReport();

            report.HasError = true;
            report.Message = message;
            report.MessageArray = new List<string>() { exception.Message };

            reportStatus(report);
        }
        #endregion
    }

}
