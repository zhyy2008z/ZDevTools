using System.Diagnostics;

namespace ZDevTools.Rebooter
{
    /// <summary>
    /// 重启控制器
    /// </summary>
    public static class RebootController
    {
        /// <summary>
        /// 运行指定程序
        /// </summary>
        public static void RunIt(string exePath)
        {
#if NETFRAMEWORK
            Process.Start(typeof(RebootController).Assembly.Location, "\"" + exePath + "\"" + " " + Process.GetCurrentProcess().Id);
#elif NETCOREAPP
            Process.Start("dotnet", "\"" + typeof(RebootController).Assembly.Location + "\" \"" + exePath + "\" " + Process.GetCurrentProcess().Id);
#endif
        }
    }
}
