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

namespace ZDevTools.ServiceConsole.Views
{
    using ViewModels;
    using Models;


    /// <summary>
    /// Interaction logic for ScheduleWindow.xaml
    /// </summary>
    public partial class ScheduleWindow : Window
    {
        public ScheduleWindow()
        {
            DataContextChanged += (sender, e) =>
            {
                ViewModel = DataContext as ScheduleWindowViewModel;
                ViewModel.Synchronizer = new Synchronizer(Dispatcher);
                ViewModel.Window = this;
            };

            InitializeComponent();
        }

        public ScheduleWindowViewModel ViewModel { get; set; }


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

        private void onceButton_Checked(object sender, RoutedEventArgs e) => AdvancedOptionsGroupBox.Content = null;


        private void everyDayButton_Checked(object sender, RoutedEventArgs e) => AdvancedOptionsGroupBox.Content = AdvancedOptionsGroupBox.FindResource("EveryDayOption");


        private void everyWeekButton_Checked(object sender, RoutedEventArgs e) => AdvancedOptionsGroupBox.Content = AdvancedOptionsGroupBox.FindResource("EveryWeekOption");


        private void everyMonthButton_Checked(object sender, RoutedEventArgs e) => AdvancedOptionsGroupBox.Content = AdvancedOptionsGroupBox.FindResource("EveryMonthOption");
    }
}
