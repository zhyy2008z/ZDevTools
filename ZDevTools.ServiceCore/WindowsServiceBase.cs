using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace ZDevTools.ServiceCore
{
    /// <summary>
    /// Windows服务基类
    /// </summary>
    public abstract class WindowsServiceBase : System.ServiceProcess.ServiceBase, IHostedService
    {
        public const string WindowsServiceLogsFolder = "wslogs";


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
        StreamWriter _streamWriter;
        static readonly object Locker = new object();

        /// <summary>
        /// 写入日志（该方法允许多线程调用）
        /// </summary>
        /// <param name="message"></param>
        void writeLog(string message, string level, Exception exception)
        {
            if (_disposed) //记录日志功能失效
                return;

            lock (Locker)
            {
                if (_logStream == null)
                {
                    string saveFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, WindowsServiceLogsFolder);
                    if (!Directory.Exists(saveFolder))
                        Directory.CreateDirectory(saveFolder);

                    string logFullName = Path.Combine(saveFolder, ServiceName + ".log");

                    bool fileExists = File.Exists(logFullName);

                    _logStream = File.Open(logFullName, FileMode.Append, FileAccess.Write, FileShare.Read);
                    _streamWriter = new StreamWriter(_logStream);

                    if (!fileExists) //write log header
                        _streamWriter.WriteLine("时间\t\t\t线程\t级别\t消息");
                }

                _streamWriter.WriteLine($"{ServiceBase.FormatDateTime(DateTime.Now)}\t[{System.Threading.Thread.CurrentThread.ManagedThreadId}]\t{level} - 【{DisplayName}】{message}");

                if (exception != null)
                    _streamWriter.WriteLine(exception);

                _streamWriter.Flush();
            }
        }

        bool _disposed;
        protected override void Dispose(bool disposing)
        {
            if (disposing && _logStream != null)
            {
                lock (Locker)
                {
                    _streamWriter?.Dispose();
                    _logStream?.Dispose();
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
        /// 基类实现为记录服务正在运行状态，请将本基类方法在您的实现中作为最后一条调用语句
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
        protected sealed override void OnContinue() { }
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

        #region 已导入
        /// <summary>
        /// 保存Hash对象
        /// </summary>
        public void SaveHash(string hashId, Dictionary<string, string> dic)
        {
            if (ServiceBase.RedisManagerPool == null)
                return;

            using (var client = ServiceBase.RedisManagerPool.GetClient())
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
            if (ServiceBase.RedisManagerPool == null)
                return;

            using (var client = ServiceBase.RedisManagerPool.GetClient())
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

                saveReportHistory(serviceReport);

                if (ServiceBase.RedisManagerPool != null)
                    using (var client = ServiceBase.RedisManagerPool.GetClient())
                    {
                        client.SetEntryInHash(RedisKeys.ServiceReports, ServiceName, JsonConvert.SerializeObject(serviceReport));
                        client.PublishMessage(RedisKeys.ServiceReports, ServiceName);
                    }
            }
            catch (Exception ex) { LogWarn("状态汇报出错，错误：" + ex.Message, ex); }
        }

        private void saveReportHistory(ServiceReport serviceReport)
        {
            string saveFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, ServiceBase.ServiceReportsFolder);
            if (!Directory.Exists(saveFolder))
                Directory.CreateDirectory(saveFolder);

            string reportFullName = Path.Combine(saveFolder, ServiceName + ".log");

            if (!File.Exists(reportFullName))
            {
                File.WriteAllText(reportFullName, "时间\t\t\t错误\t消息\t\t\t消息组\r\n");
            }

            File.AppendAllText(reportFullName, $"{ServiceBase.FormatDateTime(serviceReport.UpdateTime)}\t{(serviceReport.HasError ? "[有]" : "[无]")}\t{serviceReport.Message}\t{(serviceReport.MessageArray == null ? null : string.Join("、", serviceReport.MessageArray))}\r\n");
        }

        public void ReportStatus(string message)
        {
            ServiceReport report = new ServiceReport();

            report.Message = message;

            reportStatus(report);
        }

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

        public void ReportError(string message)
        {
            ServiceReport report = new ServiceReport();

            report.HasError = true;
            report.Message = message;

            reportStatus(report);
        }

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
