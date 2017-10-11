using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prism.Mvvm;
using System.Collections.ObjectModel;
using Prism.Commands;


namespace ZDevTools.ServiceConsole.ViewModels
{
    using Views;
    using Models;

    public class OneKeyStartConfigWindowViewModel : WindowViewModelBase<OneKeyStartConfigWindow>
    {
        public OneKeyStartConfigWindowViewModel()
        {
            OKCommand = new DelegateCommand(() =>
            {
                Window.DialogResult = true;
            });
        }

        ObservableCollection<ServiceItemModel> _configs;
        public ObservableCollection<ServiceItemModel> Configs { get { return _configs; } set { SetProperty(ref _configs, value); } }

        public DelegateCommand OKCommand { get; }
    }
}
