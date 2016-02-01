using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZDevTools.ServiceMonitor
{
    public class ServiceReport
    {
        public string ServiceName { get; set; }

        public bool HasError { get; set; }

        public string Message { get; set; }

        public DateTime UpdateTime { get; set; }

        public string[] MessageArray { get; set; }
    }
}
