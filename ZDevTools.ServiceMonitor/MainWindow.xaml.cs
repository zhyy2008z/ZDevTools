using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
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
using Microsoft.Extensions.Options;
using ReactiveUI;

namespace ZDevTools.ServiceMonitor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    partial class MainWindow
    {
        readonly IOptions<MonitorOptions> Options;

        public MainWindow(MainViewModel viewModel, IOptions<MonitorOptions> options)
        {
            this.Options = options;
            this.ViewModel = viewModel;

            InitializeComponent();


            this.WhenActivated(disposables =>
            {
                this.OneWayBind(ViewModel, vm => vm.Reports, v => v.reportsListView.ItemsSource).DisposeWith(disposables);


                this.ViewModel.WhenAnyValue(vm => vm.CurrentReport).Subscribe(ri =>
                {
                    if (ri != null)
                    {
                        if (!ri.IsSuccess)
                        {
                            _niMain.ShowBalloonTip(120, "刷新服务状态时出错", ri.ErrorMessage, System.Windows.Forms.ToolTipIcon.Error);
                        }
                        else if (!ri.IsAll)
                        {
                            reportIfHasError(ri.ServiceReport);
                        }
                    }
                }).DisposeWith(disposables);
            });
        }



        private void window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (!_canClose)
            {
                e.Cancel = true;
                this.Hide();
            }
        }

        void reportIfHasError(ServiceReport report)
        {
            if (report.HasError && (DateTime.Now - report.UpdateTime).TotalDays < 3)
                _niMain.ShowBalloonTip(120, report.ServiceName + "异常", report.Message, System.Windows.Forms.ToolTipIcon.Error);
        }

        System.Windows.Forms.NotifyIcon _niMain = new System.Windows.Forms.NotifyIcon();
        System.Windows.Forms.ContextMenuStrip _contextMenu = new System.Windows.Forms.ContextMenuStrip();
        bool _canClose;
        private void window_Loaded(object sender, RoutedEventArgs e)
        {
            Title = Options.Value.ServiceMonitorTitle;
            _niMain.Icon = new System.Drawing.Icon("server.ico");
            _niMain.Visible = true;
            _niMain.DoubleClick += niMain_DoubleClick;
            _niMain.Text = Options.Value.ServiceMonitorTitle;
            _niMain.ContextMenuStrip = _contextMenu;

            _contextMenu.Items.Add("退出").Click += (sender, e) =>
            {
                _canClose = true;
                this.Close();
            };
        }

        private void niMain_DoubleClick(object sender, EventArgs e)
        {
            if (this.IsVisible)
                this.Hide();
            else
            {
                this.Show();
                this.Activate();
            }
        }

        private void reactiveWindow_Closed(object sender, EventArgs e)
        {
            _niMain.Dispose();
            _contextMenu.Dispose();
        }

        private void textBlock_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (reportsListView.SelectedItem is ServiceReport serviceReport)
            {
                var lb = new System.Windows.Forms.ListBox();
                lb.Width = 150;
                lb.Height = 180;
                lb.Items.AddRange(serviceReport.MessageArray);

                System.Windows.Forms.ToolStripDropDown dropDown = new System.Windows.Forms.ToolStripDropDown();
                dropDown.Padding = System.Windows.Forms.Padding.Empty;
                dropDown.Margin = System.Windows.Forms.Padding.Empty;

                var controlHost = new System.Windows.Forms.ToolStripControlHost(lb);
                controlHost.Padding = System.Windows.Forms.Padding.Empty;
                controlHost.Margin = System.Windows.Forms.Padding.Empty;
                controlHost.AutoSize = false;

                dropDown.Items.Add(controlHost);

                var p = PointToScreen(e.GetPosition(this));

                dropDown.Show(new System.Drawing.Point((int)p.X, (int)p.Y));
            }
        }
    }
}
