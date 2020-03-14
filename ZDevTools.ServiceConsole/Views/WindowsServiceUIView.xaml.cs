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
using System.Windows.Navigation;
using System.Windows.Shapes;
using ZDevTools.ServiceConsole.ViewModels;
using ReactiveUI;
using System.Reactive.Disposables;

namespace ZDevTools.ServiceConsole.Views
{
    /// <summary>
    /// Interaction logic for WindowsServiceUI.xaml
    /// </summary>
    partial class WindowsServiceUIView
    {
        public WindowsServiceUIView()
        {
            InitializeComponent();

            this.WhenActivated(disposables =>
            {
                this.OneWayBind(ViewModel, vm => vm.DisplayName, v => v.displayNameTextBlock.Text).DisposeWith(disposables);
                this.OneWayBind(ViewModel, vm => vm.StatusText, v => v.statusTextTextBlock.Text).DisposeWith(disposables); this.OneWayBind(ViewModel, vm => vm.StatusToolTip, v => v.statusTextTextBlock.ToolTip).DisposeWith(disposables); this.OneWayBind(ViewModel, vm => vm.StatusColor, v => v.statusTextTextBlock.Foreground).DisposeWith(disposables);

                this.Bind(ViewModel, vm => vm.StartupTypeIndex, v => v.startupTypeIndexComboBox.SelectedIndex).DisposeWith(disposables);
                this.BindCommand(ViewModel, vm => vm.ApplyCommand, v => v.applyButton).DisposeWith(disposables);
                this.BindCommand(ViewModel, vm => vm.OperateServiceCommand, v => v.operateButton).DisposeWith(disposables);
                this.OneWayBind(ViewModel, vm => vm.ButtonText, v => v.operateButton.Content).DisposeWith(disposables);
                this.OneWayBind(ViewModel, vm => vm.ButtonEnabled, v => v.operateButton.IsEnabled).DisposeWith(disposables);


            });
        }
    }
}
