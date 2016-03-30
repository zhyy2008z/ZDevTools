using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZDevTools.ServiceConsole
{
    public class AppConfig
    {
        public Dictionary<string, ServiceItemConfig> OneKeyStart { get; set; }

        public Dictionary<string, string> ServicesConfig { get; set; }
    }
}
