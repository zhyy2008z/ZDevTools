using System.Diagnostics;

namespace ZDevTools.Rebooter
{
    public static class RebootController
    {
        public static void RunIt(string exePath)
        {
            Process.Start(typeof(RebootController).Assembly.Location, "\"" + exePath + "\"" + " " + Process.GetCurrentProcess().Id);
        }
    }
}
