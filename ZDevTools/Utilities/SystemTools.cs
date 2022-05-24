using System;
using System.IO;
using System.Runtime.InteropServices;

namespace ZDevTools.Utilities
{
    /// <summary>
    /// 系统工具
    /// </summary>
    public static class SystemTools
    {
        /// <summary>
        /// 设置系统时间
        /// </summary>
        public static bool SetSystemTime(DateTime dateTime)
        {
            SYSTEMTIME systemTime = new SYSTEMTIME();
            systemTime.FromDateTime(dateTime);
            switch (dateTime.Kind)
            {
                case DateTimeKind.Utc:
                    return NativeMethods.SetSystemTime(ref systemTime);
                default:
                    return NativeMethods.SetLocalTime(ref systemTime);
            }
        }

        /// <summary>
        /// 获取当前应用工作集大小（单位：MB）
        /// </summary>
        public static double GetWorkingSet() => (double)Environment.WorkingSet / 1024 / 1024;

        /// <summary>
        /// 保持系统活跃状态
        /// </summary>
        public static void KeepSystemAlive() => NativeMethods.SetThreadExecutionState(ES_Flags.ES_SYSTEM_REQUIRED);

        /// <summary>
        /// 将软件当前状态写入指定路径下Minidump文件
        /// </summary>
        /// <param name="fileName">文件路径</param>
        public static bool WriteMinidump(string fileName)
        {
            return WriteMinidump(fileName, (uint)MiniDumpTypes.MiniDumpWithFullMemory);
        }

        /// <summary>
        /// 将软件当前状态写入指定路径下Minidump文件
        /// </summary>
        /// <param name="fileName">文件路径</param>
        /// <param name="dumpTypeFlags">minidump文件类型标记</param>
        /// <returns>Minidump文件是否创建成功</returns>
        public static bool WriteMinidump(string fileName, uint dumpTypeFlags)
        {
            using (var fs = new FileStream(fileName, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                MiniDumpExceptionInformation info;
                info.ThreadId = NativeMethods.GetCurrentThreadId();
                info.ClientPointers = false;
                info.ExceptioonPointers = Marshal.GetExceptionPointers();
                return NativeMethods.MiniDumpWriteDump(
                  NativeMethods.GetCurrentProcess(),
                  NativeMethods.GetCurrentProcessId(),
                  fs.SafeFileHandle.DangerousGetHandle(),
                  dumpTypeFlags,
                  ref info,
                  IntPtr.Zero,
                  IntPtr.Zero);
            }
        }
    }
}
