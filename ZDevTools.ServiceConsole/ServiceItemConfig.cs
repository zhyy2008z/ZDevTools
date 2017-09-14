using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace ZDevTools.ServiceConsole
{
    public class ServiceItemConfig
    {
        [JsonIgnore]
        public string ServiceName { get; set; }

        public bool OneKeyStart { get; set; }
    }
}
