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

namespace ZDevTools.ServiceConsole.Views
{
    using ViewModels;

    /// <summary>
    /// Interaction logic for OneKeyStartConfigWindow.xaml
    /// </summary>
    public partial class OneKeyStartConfigWindow : Window
    {
        public OneKeyStartConfigWindow()
        {
            DataContextChanged += (sender, e) =>
            {
                ViewModel = DataContext as OneKeyStartConfigWindowViewModel;
                ViewModel.Synchronizer = new Synchronizer(Dispatcher);
                ViewModel.Window = this;
            };

            InitializeComponent();
        }

        public OneKeyStartConfigWindowViewModel ViewModel { get; set; }
    }
}
