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
    /// Interaction logic for ScheduleManageWindow.xaml
    /// </summary>
    partial class ScheduleManageWindow
    {
        public ScheduleManageWindow(ScheduleManageViewModel viewModel)
        {
            this.ViewModel = viewModel;

            InitializeComponent();


            this.WhenActivated(disposables =>
            {
                this.OneWayBind(ViewModel, vm => vm.Schedules, v => v.schedulesListView.ItemsSource).DisposeWith(disposables);
                this.Bind(ViewModel, vm => vm.SelectedSchedule, v => v.schedulesListView.SelectedItem).DisposeWith(disposables);
                this.BindCommand(ViewModel, vm => vm.EditScheduleCommand, v => v.schedulesListView, nameof(ListView.MouseDoubleClick)).DisposeWith(disposables);
                this.BindCommand(ViewModel, vm => vm.SelectionChangedCommand, v => v.schedulesListView, nameof(ListView.SelectionChanged)).DisposeWith(disposables);

                this.BindCommand(ViewModel, vm => vm.AddScheduleCommand, v => v.addButton).DisposeWith(disposables);
                this.BindCommand(ViewModel, vm => vm.EditScheduleCommand, v => v.editButton).DisposeWith(disposables);
                this.BindCommand(ViewModel, vm => vm.DeleteScheduleCommand, v => v.deleteButton).DisposeWith(disposables);
            });

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }
    }
}
