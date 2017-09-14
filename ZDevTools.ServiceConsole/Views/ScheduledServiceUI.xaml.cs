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

namespace ZDevTools.ServiceConsole.Views
{
    using ViewModels;

    /// <summary>
    /// Interaction logic for ScheduledServiceUI.xaml
    /// </summary>
    [Description("代表一个计划的服务"), DefaultProperty("ServiceName")]
    public partial class ScheduledServiceUI : UserControl
    {
        public ScheduledServiceUI()
        {
            DataContextChanged += (sender, e) =>
            {
                ViewModel = DataContext as ScheduledServiceUIViewModel;
                ViewModel.Synchronizer = new Synchronizer(Dispatcher);
            };

            InitializeComponent();
        }

        public ScheduledServiceUIViewModel ViewModel { get; private set; }
    }
}
