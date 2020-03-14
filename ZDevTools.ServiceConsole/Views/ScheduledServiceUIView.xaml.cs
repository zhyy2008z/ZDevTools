using System;
using System.Collections.Generic;
using System.ComponentModel;
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
    /// Interaction logic for ScheduledServiceUI.xaml
    /// </summary>
    [Description("代表一个计划的服务"), DefaultProperty("ServiceName")]
    partial class ScheduledServiceUIView
    {
        public ScheduledServiceUIView()
        {
            InitializeComponent();


            this.WhenActivated(disposables =>
            {
                this.OneWayBind(ViewModel, vm => vm.DisplayName, v => v.displayNameTextBlock.Text).DisposeWith(disposables);
                this.OneWayBind(ViewModel, vm => vm.StatusText, v => v.statusTextTextBlock.Text).DisposeWith(disposables);
                this.OneWayBind(ViewModel, vm => vm.StatusColor, v => v.statusTextTextBlock.Foreground).DisposeWith(disposables);
                this.OneWayBind(ViewModel, vm => vm.StatusToolTip, v => v.statusTextTextBlock.ToolTip).DisposeWith(disposables);
                this.OneWayBind(ViewModel, vm => vm.DescriptionText, v => v.descriptionTextTextBlock.Text).DisposeWith(disposables);
                this.BindCommand(ViewModel, vm => vm.ManageSchedulesCommand, v => v.manageSchedulesButton).DisposeWith(disposables);
                this.Bind(ViewModel, vm => vm.ImmediatelyChecked, v => v.immediatelyCheckedCheckBox.IsChecked).DisposeWith(disposables);
                this.BindCommand(ViewModel, vm => vm.OperateCommand, v => v.operateButton).DisposeWith(disposables);
                this.OneWayBind(ViewModel, vm => vm.ButtonEnabled, v => v.operateButton.IsEnabled).DisposeWith(disposables);
                this.OneWayBind(ViewModel, vm => vm.ButtonText, v => v.operateButton.Content).DisposeWith(disposables);
            });

        }
    }
}
