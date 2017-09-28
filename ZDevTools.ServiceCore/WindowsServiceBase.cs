using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using ServiceStack.Redis;

namespace ZDevTools.ServiceCore
{
    /// <summary>
    /// Windows服务基类
    /// </summary>
    public abstract class WindowsServiceBase : System.ServiceProcess.ServiceBase, IHostedService
    {
        /// <summary>
        /// Windows服务日志文件夹
        /// </summary>
        public const string WindowsServiceLogsFolder = "wslogs";

        /// <summary>
        /// 初始化一个服务
        /// </summary>
        public WindowsServiceBase()
        {
            this.ServiceName = this.GetType().Name; //设置服务名称
        }

        /// <summary>
        /// 记录错误
        /// </summary>
        void logError(string message) => writeLog(message, "ERROR", null);

        /// <summary>
        /// 记录错误
        /// </summary>
        void logError(string message, Exception exception) => writeLog(message, "ERROR", exception);


        FileStream _logStream;
        StreamWriter _logStreamWriter;
        static readonly object LogLocker = new object();

        /// <summary>
        /// 写入日志（该方法允许多线程调用）
        /// </summary>
        /// <param name="message"></param>
        /// <param name="level"></param>
        /// <param name="exception"></param>
        void writeLog(string message, string level, Exception exception)
        {
            if (_disposed) //记录日志功能失效
                return;

            lock (LogLocker)
            {
                if (_logStream == null)
                {
                    string saveFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, WindowsServiceLogsFolder);
                    if (!Directory.Exists(saveFolder))
                        Directory.CreateDirectory(saveFolder);

                    string logFullName = Path.Combine(saveFolder, ServiceName + ".log");

                    bool fileExists = File.Exists(logFullName);

                    _logStream = File.Open(logFullName, FileMode.Append, FileAccess.Write, FileShare.Read);
                    _logStreamWriter = new StreamWriter(_logStream);

                    if (!fileExists) //write log header
                        _logStreamWriter.WriteLine("时间\t\t\t线程\t级别\t消息");
                }

                _logStreamWriter.WriteLine($"{FormatDateTime(DateTime.Now)}\t[{System.Threading.Thread.CurrentThread.ManagedThreadId}]\t{level} - 【{DisplayName}】{message}");

                if (exception != null)
                    _logStreamWriter.WriteLine(exception);

                _logStreamWriter.Flush();
            }
        }

        bool _disposed;
        /// <summary>
        /// 重写服务释放方法
        /// </summary>
        /// <param name="disposing"></param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_logStream != null)
                    lock (LogLocker)
                    {
                        _logStreamWriter?.Dispose();
                        _logStream?.Dispose();
                    }

                if (_reportStream != null)
                    lock (ReportLocker)
                    {
                        _reportStreamWriter?.Dispose();
                        _reportStream?.Dispose();
                    }
            }
            _disposed = true;
            base.Dispose(disposing);
        }

        /// <summary>
        /// 服务显示名称
        /// </summary>
        public abstract string DisplayName { get; }

        /// <summary>
        /// 服务失败（针对该事件，Windows服务永不会被监听，因此没必要触发它）
        /// </summary>
        public event EventHandler<ErrorEventArgs> Faulted;

        /// <summary>
        /// 异步启动
        /// </summary>
        public Task StartAsync()
        {
            return Task.Run(() =>
            {
                var serviceController = System.ServiceProcess.ServiceController.GetServices().Where(sc => sc.ServiceName == ServiceName).SingleOrDefault();
                serviceController.Start();
                serviceController.WaitForStatus(System.ServiceProcess.ServiceControllerStatus.Running, new TimeSpan(0, 0, 30));
            });
        }

        /// <summary>
        /// 异步停止
        /// </summary>
        public Task StopAsync()
        {
            return Task.Run(() =>
            {
                var serviceController = System.ServiceProcess.ServiceController.GetServices().Where(sc => sc.ServiceName == ServiceName).SingleOrDefault();
                serviceController.Stop();
                serviceController.WaitForStatus(System.ServiceProcess.ServiceControllerStatus.Stopped, new TimeSpan(0, 0, 30));
            });
        }

        /// <summary>
        /// 记录提示性信息
        /// </summary>
        public void LogInfo(string message)
        {
            writeLog(message, "INFO", null);
        }

        /// <summary>
        /// 记录一般性警告错误
        /// </summary>
        public void LogWarn(string message)
        {
            writeLog(message, "WARN", null);
        }

        /// <summary>
        /// 记录一般性警告错误
        /// </summary>
        public void LogWarn(string message, Exception exception)
        {
            writeLog(message, "WARN", exception);
        }

        /// <summary>
        /// 被密封，由框架管理
        /// </summary>
        /// <param name="args"></param>
        protected sealed override void OnStart(string[] args)
        {
            try
            {
                DoWork(args);
                ReportStatus("状态：正在运行");
            }
            catch (Exception ex)
            {
                logError("无法启动服务", ex); //由于错误无法传达到壳程序里，故在此处记录错误
                throw;//扔出错误，错误会出现在事件管理器里
            }
        }


        /// <summary>
        /// 重写该方法以启动你的业务逻辑
        /// </summary>
        /// <param name="args">启动参数</param>
        protected abstract void DoWork(string[] args);


        /// <summary>
        /// 被密封，由框架管理
        /// </summary>
        protected sealed override void OnStop()
        {
            if (!_errorStop)
            {
                try
                {
                    CleanUp();
                    ReportStatus("状态：停止运行");
                }
                catch (Exception ex)
                {
                    logError("无法停止服务", ex);//由于错误无法传达到壳程序里，故在此处记录错误
                    throw;//扔出错误，错误会出现在事件管理器里
                }
            }
        }

        /// <summary>
        /// 体面地结束你的任务
        /// </summary>
        protected abstract void CleanUp();


        //服务不支持暂停与恢复
        /// <summary>
        /// 被密封，由框架管理
        /// </summary>
        protected sealed override void OnContinue() { }
        /// <summary>
        /// 被密封，由框架管理
        /// </summary>
        protected sealed override void OnPause() { }


        bool _errorStop;
        /// <summary>
        /// 发生严重错误，报告错误并停止运行服务，请不要在<see cref="DoWork(string[])"/>或<see cref="CleanUp()"/>执行时调用此方法，系统会自动捕获这两个方法所Throw出的异常，并做相应记录。
        /// </summary>
        public void ReportErrorAndStop(string message, Exception exception)
        {
            //填充参数
            if (string.IsNullOrEmpty(message) && exception != null)
                message = exception.Message;

            //报告错误
            if (exception != null)
            {
                logError(message, exception);
                ReportError($"状态：已停止，Windows服务失败【{message}】", exception);
            }
            else
            {
                logError(message);
                ReportError($"状态：已停止，Windows服务失败【{message}】");
            }

            //发出停止命令
            _errorStop = true;
            Stop();
        }

        #region 导入ServiceBase通用成员
        /// <summary>
        /// RedisManagerPool
        /// </summary>
        public static RedisManagerPool RedisManagerPool { get { return ServiceBase.RedisManagerPool; } }

        /// <summary>
        /// 提供格式化日期时间的统一方案
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public static string FormatDateTime(DateTime dateTime)
        {
            return ServiceBase.FormatDateTime(dateTime);
        }

        /// <summary>
        /// 服务报告存放位置
        /// </summary>
        public const string ServiceReportsFolder = ServiceBase.ServiceReportsFolder;
        #endregion

        #region 导入ServiceBase实例方法
        /// <summary>
        /// 保存Hash对象
        /// </summary>
        public void SaveHash(string hashId, Dictionary<string, string> dic)
        {
            if (RedisManagerPool == null)
                return;

            using (var client = RedisManagerPool.GetClient())
            {
                using (var trans = client.CreateTransaction())
                {
                    trans.QueueCommand(rc => rc.Remove(hashId));
                    trans.QueueCommand(rc => rc.SetRangeInHash(hashId, dic));
                    trans.Commit();
                }
            }

            LogInfo($"生成条目{dic.Count}条，已保存到hashid为{hashId}的哈希集合中");
        }

        /// <summary>
        /// 保存单值字符串
        /// </summary>
        public void SaveValue(string key, string value)
        {
            if (RedisManagerPool == null)
                return;

            using (var client = RedisManagerPool.GetClient())
            {
                client.SetValue(key, value);
            }

            LogInfo($"生成条目{key}");
        }

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
                        client.SetEntryInHash(RedisKeys.ServiceReports, ServiceName, JsonConvert.SerializeObject(serviceReport));
                        client.PublishMessage(RedisKeys.ServiceReports, ServiceName);
                    }
            }
            catch (Exception ex) { LogWarn("状态汇报出错，错误：" + ex.Message, ex); }
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
            if (_disposed) //记录日志功能失效
                return;

            lock (ReportLocker)
            {
                if (_reportStream == null)
                {
                    string saveFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, ServiceReportsFolder);
                    if (!Directory.Exists(saveFolder))
                        Directory.CreateDirectory(saveFolder);

                    string reportFullName = Path.Combine(saveFolder, ServiceName + ".log");

                    bool fileExists = File.Exists(reportFullName);

                    _reportStream = File.Open(reportFullName, FileMode.Append, FileAccess.Write, FileShare.Read);
                    _reportStreamWriter = new StreamWriter(_reportStream);

                    if (!fileExists) //write log header
                        _reportStreamWriter.WriteLine("时间\t\t\t错误\t消息\t\t\t消息组");
                }

                _reportStreamWriter.WriteLine($"{FormatDateTime(serviceReport.UpdateTime)}\t{(serviceReport.HasError ? "[有]" : "[无]")}\t{serviceReport.Message}\t{(serviceReport.MessageArray == null ? null : string.Join("、", serviceReport.MessageArray))}");

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
        public void ReportError(string message, Exception exception)
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
