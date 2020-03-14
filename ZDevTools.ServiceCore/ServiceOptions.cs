using System;
using System.Collections.Generic;
using System.Text;

namespace ZDevTools.ServiceCore
{
    public class ServiceOptions
    {
        public string RedisServer { get; set; }

        public WindowsServiceLogLevel WindowsServiceLogLevel { get; set; }
    }
}
