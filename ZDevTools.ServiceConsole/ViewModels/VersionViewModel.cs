using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using ZDevTools.ServiceConsole.Models;
using ZDevTools.ServiceConsole.Views;
using ReactiveUI.Fody.Helpers;
using ReactiveUI;

namespace ZDevTools.ServiceConsole.ViewModels
{
    public class VersionViewModel : ReactiveObject
    {
        [Reactive]
        public ObservableCollection<ModuleInfo> Modules { get; set; }
    }
}
