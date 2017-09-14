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

namespace ZDevTools.ServiceConsole.Views
{
    using ViewModels;

    /// <summary>
    /// Interaction logic for WindowsServiceUI.xaml
    /// </summary>
    public partial class WindowsServiceUI : UserControl
    {
        public WindowsServiceUI()
        {
            DataContextChanged += (sender, e) =>
            {
                ViewModel = DataContext as WindowsServiceUIViewModel;
                ViewModel.Synchronizer = new Synchronizer(Dispatcher);
            };

            InitializeComponent();
        }

        public WindowsServiceUIViewModel ViewModel { get; set; }
    }
}
