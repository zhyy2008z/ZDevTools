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
    /// Interaction logic for HostedServiceUI.xaml
    /// </summary>
    public partial class HostedServiceUI : UserControl
    {
        public HostedServiceUI()
        {
            DataContextChanged += (sender, e) =>
            {
                ViewModel = DataContext as HostedServiceUIViewModel;
                ViewModel.Synchronizer = new Synchronizer(Dispatcher);
            };

            InitializeComponent();
        }


        public HostedServiceUIViewModel ViewModel { get; private set ; }
    }
}
