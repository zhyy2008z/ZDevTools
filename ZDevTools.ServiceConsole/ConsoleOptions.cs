using System;
using System.Collections.Generic;
using System.Text;

namespace ZDevTools.ServiceConsole
{
    public class ConsoleOptions
    {
        public string ServiceConsoleTitle { get; set; } = "服务控制台名称（您可以自定义控制台要显示的名称）";

        public int ServiceWaitTimeout { get; set; } = 600;

        public string AutoUpdateUri { get; set; }

        public int AutoUpdateInterval { get; set; } = 1800;
    }
}
