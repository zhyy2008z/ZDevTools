using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace ZDevTools.ServiceConsole.Models
{
    public class ServiceItemModel : ReactiveObject
    {
        [Reactive]
        public string ServiceKey { get; set; }

        [Reactive]
        public string ServiceName { get; set; }

        [Reactive]
        public bool OneKeyStart { get; set; }
    }
}
