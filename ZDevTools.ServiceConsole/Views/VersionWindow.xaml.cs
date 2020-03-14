using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using ZDevTools.ServiceConsole.ViewModels;
using ReactiveUI;
using System.Reactive.Disposables;

namespace ZDevTools.ServiceConsole.Views
{
    /// <summary>
    /// Interaction logic for VersionWindow.xaml
    /// </summary>
    partial class VersionWindow
    {
        public VersionWindow(VersionViewModel viewModel)
        {
            this.ViewModel = viewModel;

            InitializeComponent();

            this.WhenActivated(disposables =>
            {
                this.OneWayBind(ViewModel, vm => vm.Modules, v => v.modulesListView.ItemsSource).DisposeWith(disposables);
            });
        }

    }
}
