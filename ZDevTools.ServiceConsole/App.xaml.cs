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
            //没有调用自动生成的InitializeComponent()，就是为了仅加载资源字典，因为启动过程被安排在Host中完成。
            LoadComponent(this, new Uri("/ZDevTools.ServiceConsole;component/app.xaml", UriKind.Relative));
        }
    }
}
