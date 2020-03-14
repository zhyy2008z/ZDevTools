using System.Diagnostics;

namespace ZDevTools.Rebooter
{
    public static class RebootController
    {
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
