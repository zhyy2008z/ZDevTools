using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace ZDevTools.ServiceConsole.Models
{

    public class ModuleInfo : ReactiveObject
    {
        [Reactive]
        public string Name { get; set; }

        [Reactive]
        public string Version { get; set; }

    }
}
