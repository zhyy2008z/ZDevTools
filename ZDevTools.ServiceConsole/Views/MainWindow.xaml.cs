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
using System.Windows.Threading;

namespace ZDevTools.ServiceConsole.Views
{
    using ViewModels;

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            DataContextChanged += (sender, e) =>
            {
                ViewModel = DataContext as MainWindowViewModel;
                ViewModel.Synchronizer = new Synchronizer(Dispatcher);
                ViewModel.Window = this;
            };

            InitializeComponent();

            MainViewModel = ViewModel;
        }

        public MainWindowViewModel ViewModel { get; set; }

        public static MainWindowViewModel MainViewModel { get; private set; }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (ViewModel.CanClose)
                ViewModel.SaveConfig();
            else
            {
                e.Cancel = true;
                this.Hide();
            }
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            MainViewModel = null;
            ViewModel.Dispose();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            ViewModel.AutoStart();
        }
    }
}
