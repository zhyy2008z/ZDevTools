using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace ZDevTools.ServiceConsole
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    partial class App
    {
        public App()
        {
            LoadComponent(this, new Uri("/ZDevTools.ServiceConsole;component/app.xaml", UriKind.Relative));
        }
    }
}
