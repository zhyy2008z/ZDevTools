using System;
using System.Diagnostics;

namespace ZDevTools.Utilities
{
    /// <summary>
    /// 当前应用Cpu占用监视器
    /// </summary>
    public class CpuUsageMonitor
    {
        readonly Process CurrentProcess = Process.GetCurrentProcess();
        readonly int ProcessorCount = Environment.ProcessorCount;

        /// <summary>
        /// 初始化Cpu占用监视器实例
        /// </summary>
        public CpuUsageMonitor()
        {
            _lastReadTime = DateTime.Now;
            _lastCpuTotalProcessorTime = CurrentProcess.TotalProcessorTime;
        }

        DateTime _lastReadTime;
        TimeSpan _lastCpuTotalProcessorTime;

        /// <summary>
        /// 获取从最后一次读取时至本次读取期间CPU的使用情况（比例值，取值范围[0,1]）
        /// </summary>
        /// <returns></returns>
        public double GetCpuUsage()
        {
            var readTime = DateTime.Now;
            var cpuTotalProcessorTime = CurrentProcess.TotalProcessorTime;

            var ret = (cpuTotalProcessorTime - _lastCpuTotalProcessorTime).TotalSeconds / ProcessorCount / (readTime - _lastReadTime).TotalSeconds;

            _lastReadTime = readTime;
            _lastCpuTotalProcessorTime = cpuTotalProcessorTime;

            return ret;
        }
    }
}
