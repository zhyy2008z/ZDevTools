using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prism.Mvvm;
using System.Collections.ObjectModel;

namespace ZDevTools.ServiceConsole.ViewModels
{
    using Models;
    using Views;

    public class VersionWindowViewModel : WindowViewModelBase<VersionWindow>
    {
        ObservableCollection<ModuleInfo> _modules;
        public ObservableCollection<ModuleInfo> Modules { get { return _modules; } set { SetProperty(ref _modules, value); } }
    }
}
