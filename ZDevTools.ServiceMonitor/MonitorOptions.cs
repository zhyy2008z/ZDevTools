using System;
using System.Collections.Generic;
using System.Text;

namespace ZDevTools.ServiceMonitor
{
    class MonitorOptions
    {
        public string RedisServer { get; set; } = "localhost:6379";

        public string ServiceMonitorTitle { get; set; } = "服务监视器名称（您可以自定义监视器要显示的名称）";

    }
}
