﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServiceStack.Redis;
using System.Diagnostics;
using Newtonsoft.Json;

namespace ZDevTools.ServiceCore
{
    public abstract class ServiceBase : IServiceBase
    {
        static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(ServiceBase));
        void logInfo(string message) => log.Info($"【{ServiceName}】{message}");
        void logError(string message, Exception exception) => log.Error($"【{ServiceName}】{message}", exception);

        static RedisManagerPool redisManagerPool { get; } = string.IsNullOrEmpty(Properties.Settings.Default.RedisServer) ? null : new RedisManagerPool(Properties.Settings.Default.RedisServer);

        public abstract string ServiceName { get; }

        protected abstract log4net.ILog Log { get; }

        /// <summary>
        /// 保存Hash对象
        /// </summary>
        public void SaveHash(string hashId, Dictionary<string, string> dic)
        {
            if (redisManagerPool == null)
                return;

            using (var client = redisManagerPool.GetClient())
            {
                using (var trans = client.CreateTransaction())
                {
                    trans.QueueCommand(rc => rc.Remove(hashId));
                    trans.QueueCommand(rc => rc.SetRangeInHash(hashId, dic));
                    trans.Commit();
                }
            }

            logInfo($"生成条目{dic.Count}条，已保存到hashid为{hashId}的哈希集合中");
        }

        /// <summary>
        /// 保存单值字符串
        /// </summary>
        public void SaveValue(string key, string value)
        {
            if (redisManagerPool == null)
                return;

            using (var client = redisManagerPool.GetClient())
            {
                client.SetValue(key, value);
            }

            logInfo($"生成条目{key}");
        }

        /// <summary>
        /// 记录提示性信息
        /// </summary>
        /// <param name="message"></param>
        public void LogInfo(string message)
        {
            Log.Info($"【{ServiceName}】{message}");
        }

        /// <summary>
        /// 记录调试信息
        /// </summary>
        /// <param name="message"></param>
        public void LogDebug(string message)
        {
            Log.Debug($"【{ServiceName}】{message}");
        }

        /// <summary>
        /// 记录调试信息
        /// </summary>
        /// <param name="message"></param>
        public void LogDebug(string message, Exception exception)
        {
            Log.Debug($"【{ServiceName}】{message}", exception);
        }

        /// <summary>
        /// 记录一般性警告错误
        /// </summary>
        /// <param name="message"></param>
        public void LogWarn(string message)
        {
            Log.Warn($"【{ServiceName}】{message}");
        }

        /// <summary>
        /// 记录一般性警告错误
        /// </summary>
        /// <param name="message"></param>
        /// <param name="exception"></param>
        public void LogWarn(string message, Exception exception)
        {
            Log.Warn($"【{ServiceName}】{message}", exception);
        }

        /// <summary>
        /// 扔出较为严重的错误，该错误发生后任务彻底终止
        /// </summary>
        /// <param name="message"></param>
        [DebuggerNonUserCode]
        public void ThrowError(string message)
        {
            throw new Exception($"【{ServiceName}】{message}");
        }

        /// <summary>
        /// 扔出较为严重的错误，该错误发生后任务彻底终止
        /// </summary>
        /// <param name="message"></param>
        /// <param name="innerException"></param>
        [DebuggerNonUserCode]
        public void ThrowError(string message, Exception innerException)
        {
            throw new Exception($"【{ServiceName}】{message}", innerException);
        }

        /// <summary>
        /// 提供一个标准的时间格式化方法
        /// </summary>
        /// <param name="dateTime">要被格式化的日期时间实例</param>
        /// <returns></returns>
        public static string FormatDateTime(DateTime dateTime) => $"{dateTime:yyyy-MM-dd HH:mm:ss}";

        void reportStatus(ServiceReport serviceReport)
        {
            if (redisManagerPool == null)
                return;

            try
            {
                serviceReport.ServiceName = ServiceName;
                serviceReport.UpdateTime = DateTime.Now;
                using (var client = redisManagerPool.GetClient())
                {
                    client.SetEntryInHash(RedisKeys.ServiceReports, this.GetType().Name, JsonConvert.SerializeObject(serviceReport));
                    client.PublishMessage(RedisKeys.ServiceReports, this.GetType().Name);
                }
            }
            catch (Exception ex) { logError("状态汇报出错，错误：" + ex.Message, ex); }
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

        public void ReportError(Exception exception)
        {
            ServiceReport report = new ServiceReport();

            report.HasError = true;
            report.Message = exception.Message;

            reportStatus(report);
        }

        public void ReportError(string message)
        {
            ServiceReport report = new ServiceReport();

            report.HasError = true;
            report.Message = message;

            reportStatus(report);
        }

        public void ReportStatus()
        {
            reportStatus(new ServiceReport());
        }
    }

}