using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prism.Mvvm;

namespace ZDevTools.ServiceConsole.Models
{
    public class ModuleInfo : BindableBase
    {
        string _name;
        public string Name { get { return _name; } set { SetProperty(ref _name, value); } }

        string _version;
        public string Version { get { return _version; } set { SetProperty(ref _version, value); } }

    }
}
