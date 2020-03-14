using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using ZDevTools.ServiceConsole.Views;
using ZDevTools.ServiceConsole.Models;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace ZDevTools.ServiceConsole.ViewModels
{
    public class OneKeyStartConfigViewModel : ReactiveObject
    {

        [Reactive]
        public ObservableCollection<ServiceItemModel> Configs { get; set; }

    }
}
