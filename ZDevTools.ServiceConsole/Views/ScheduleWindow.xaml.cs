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
using Xceed.Wpf.Toolkit;
using Xceed.Wpf.Toolkit.Primitives;
using System.Collections.ObjectModel;
using System.Collections;
using ZDevTools.ServiceConsole.ViewModels;
using ZDevTools.ServiceConsole.Models;
using ReactiveUI;
using System.Reactive.Disposables;

namespace ZDevTools.ServiceConsole.Views
{
    /// <summary>
    /// Interaction logic for ScheduleWindow.xaml
    /// </summary>
    partial class ScheduleWindow
    {
        public ScheduleWindow(ScheduleViewModel viewModel)
        {
            this.ViewModel = viewModel;

            InitializeComponent();

            this.WhenActivated(disposables =>
            {
                this.Bind(ViewModel, vm => vm.IsOnce, v => v.onceButton.IsChecked).DisposeWith(disposables);
                this.Bind(ViewModel, vm => vm.IsEveryDay, v => v.everyDayButton.IsChecked).DisposeWith(disposables);
                this.Bind(ViewModel, vm => vm.IsEveryWeek, v => v.everyWeekButton.IsChecked).DisposeWith(disposables);
                this.Bind(ViewModel, vm => vm.IsEveryMonth, v => v.everyMonthButton.IsChecked).DisposeWith(disposables);

                this.Bind(ViewModel, vm => vm.StartAtDate, v => v.startAtDateDateTimePicker.Value).DisposeWith(disposables);

                this.advancedOptionsGroupBox.DataContext = ViewModel;

                this.Bind(ViewModel, vm => vm.RepeatSchedule, v => v.repeatScheduleCheckBox.IsChecked).DisposeWith(disposables);
                this.Bind(ViewModel, vm => vm.RepeatPeriod, v => v.repeatPeriodTimeSpanUpDown.Value).DisposeWith(disposables);
                this.Bind(ViewModel, vm => vm.RepeatUntil, v => v.repeatUntilCheckBox.IsChecked).DisposeWith(disposables);
                this.Bind(ViewModel, vm => vm.RepeatUntilTime, v => v.repeatUntilTimeTimeSpanUpDown.Value).DisposeWith(disposables);
                this.Bind(ViewModel, vm => vm.HasEndTime, v => v.hasEndTimeCheckBox.IsChecked).DisposeWith(disposables);
                this.Bind(ViewModel, vm => vm.EndTime, v => v.endTimeDateTimePicker.Value).DisposeWith(disposables);
                this.Bind(ViewModel, vm => vm.IsEnabled, v => v.isEnabledCheckBox.IsChecked).DisposeWith(disposables);
            });
        }


        private void allCheckMenuItem_Click(object sender, RoutedEventArgs e)
        {
            var parent = (e.Source as MenuItem).Parent as ContextMenu;
            var ccb = parent.PlacementTarget as CheckComboBox;
            var seletedList = ccb.SelectedItemsOverride;
            seletedList.Clear();
            foreach (object item in ccb.ItemsSource) { seletedList.Add(item); }
        }

        private void allNotCheckMenuItem_Click(object sender, RoutedEventArgs e)
        {
            var parent = (e.Source as MenuItem).Parent as ContextMenu;
            var ccb = parent.PlacementTarget as CheckComboBox;
            ccb.SelectedItemsOverride.Clear();
        }

        private void onceButton_Checked(object sender, RoutedEventArgs e) => advancedOptionsGroupBox.Content = null;


        private void everyDayButton_Checked(object sender, RoutedEventArgs e) => advancedOptionsGroupBox.Content = advancedOptionsGroupBox.FindResource("EveryDayOption");


        private void everyWeekButton_Checked(object sender, RoutedEventArgs e) => advancedOptionsGroupBox.Content = advancedOptionsGroupBox.FindResource("EveryWeekOption");


        private void everyMonthButton_Checked(object sender, RoutedEventArgs e) => advancedOptionsGroupBox.Content = advancedOptionsGroupBox.FindResource("EveryMonthOption");

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (ViewModel.CheckOk())
                DialogResult = true;
        }
    }
}
