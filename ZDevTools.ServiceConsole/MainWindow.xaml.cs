using System;
using System.Reactive.Disposables;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using FSLib.App.SimpleUpdater;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using ReactiveUI;

using Serilog.Events;

namespace ZDevTools.ServiceConsole
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    partial class MainWindow
    {
        readonly ILogger<MainWindow> Logger;
        readonly IOptions<ConsoleOptions> Options;

        public MainWindow(MainViewModel viewModel, EventSink eventSink, ILogger<MainWindow> logger, IOptions<ConsoleOptions> options)
        {
            this.ViewModel = viewModel;
            this.Logger = logger;
            this.Options = options;

            InitializeComponent();

            this.WhenActivated(disposables =>
            {
                this.BindCommand(ViewModel, vm => vm.ShowVersionCommand, v => v.versionButton).DisposeWith(disposables);
                this.BindCommand(ViewModel, vm => vm.RefreshStatusCommand, v => v.refreshButton).DisposeWith(disposables);
                this.BindCommand(ViewModel, vm => vm.InstallCommand, v => v.installButton).DisposeWith(disposables);
                this.BindCommand(ViewModel, vm => vm.ConfigOneKeyStartCommand, v => v.settingButton).DisposeWith(disposables);
                this.BindCommand(ViewModel, vm => vm.DoOneKeyStartCommand, v => v.oneKeyStartButton).DisposeWith(disposables);
                this.BindCommand(ViewModel, vm => vm.StopAllCommand, v => v.stopAllButton).DisposeWith(disposables);
                this.OneWayBind(ViewModel, vm => vm.InstallButtonText, v => v.installButton.Content).DisposeWith(disposables);
                this.OneWayBind(ViewModel, vm => vm.ServiceViewModels, v => v.servicesItemsControl.ItemsSource).DisposeWith(disposables);

                eventSink.DisposeWith(disposables);
                //disposables.Add(Disposable.Create(() => eventSink.Log -= eventSink_Log));
                eventSink.Log += eventSink_Log;

                ViewModel.AutoStart();

                ViewModel.OnClose = delegate { this.Dispatcher.Invoke(() => { this.Close(); }); };
            });


        }

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

        System.Windows.Forms.NotifyIcon _niMain = new System.Windows.Forms.NotifyIcon();
        System.Windows.Forms.ContextMenuStrip _contextMenu = new System.Windows.Forms.ContextMenuStrip();
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            _niMain.Icon = new System.Drawing.Icon("server.ico");
            _niMain.Visible = true;
            _niMain.DoubleClick += niMain_DoubleClick;
            _niMain.Text = Title = Options.Value.ServiceConsoleTitle;
            _niMain.ContextMenuStrip = _contextMenu;

            _contextMenu.Items.Add("退出").Click += (sender, e) =>
            {
                if (!ViewModel.JudgeCanClose()) return;
                ViewModel.CanClose = true;
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

        private void ReactiveWindow_Closed(object sender, EventArgs e)
        {
            _niMain.Dispose();
            _contextMenu.Dispose();
        }


        const int MaxMessageCount = 1000;
        const int RemoveItemsCount = 300;
        private void eventSink_Log(LogEventLevel level, string message)
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                if (logListBox.Items.Count > MaxMessageCount)
                {

                    for (int i = MaxMessageCount; i >= MaxMessageCount - RemoveItemsCount; i--)
                        logListBox.Items.RemoveAt(i);
                }

                Brush brush = level switch
                {
                    LogEventLevel.Verbose => Brushes.LightGray,
                    LogEventLevel.Debug => Brushes.Gray,
                    LogEventLevel.Information => Brushes.Black,
                    LogEventLevel.Warning => Brushes.Orange,
                    LogEventLevel.Error => Brushes.Red,
                    LogEventLevel.Fatal => Brushes.DarkRed,
                    _ => throw new ArgumentOutOfRangeException(nameof(level)),
                };

                logListBox.Items.Insert(0, new ListBoxItem() { Content = message.Replace(Environment.NewLine, "|"), Foreground = brush });
            }));
        }

        private void logListBox_Executed(object sender, System.Windows.Input.ExecutedRoutedEventArgs e)
        {
            try
            {
                Clipboard.Clear();//设置文本前必须先清空剪贴板，否则可能会报错
                Clipboard.SetText((logListBox.SelectedItem as ListBoxItem).Content + Environment.NewLine);
            }
            catch (Exception ex)
            {
                logError(ex, "剪贴板错误：" + ex.Message);
            }
        }

        void logError(Exception exception, string message) => Logger.LogError(exception, $"【主界面】{message}");
    }
}
