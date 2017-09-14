using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prism.Mvvm;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Threading;
using System.Windows.Media;
using Prism.Commands;


namespace ZDevTools.ServiceConsole.ViewModels
{
    using ServiceCore;

    public class HostedServiceUIViewModel : ServiceViewModelBase, IControllableUI
    {
        static readonly log4net.ILog Log = log4net.LogManager.GetLogger(typeof(HostedServiceUIViewModel));
        void logInfo(string message) => Log.Info($"【{DisplayName}】{message}");
        void logError(string message, Exception exception) => Log.Error($"【{DisplayName}】{message}", exception);

        public HostedServiceUIViewModel()
        {
            OperateServiceCommand = new DelegateCommand(operateServiceAsync);
            ButtonText = "启动";
        }

        IHostedService _bindedService;
        public override IServiceBase BindedService
        {
            get
            {
                return _bindedService;
            }
            set
            {
                if (_bindedService != null)
                    throw new InvalidOperationException("不支持为该控件多次绑定服务！");

                _bindedService = (IHostedService)value;
                DisplayName = value.DisplayName;
                _bindedService.Faulted += _bindedService_Faulted;
            }
        }

        private void _bindedService_Faulted(object sender, ErrorEventArgs e)
        {
            Synchronizer.Invoke(() => UpdateServiceStatus(HostedServiceStatus.Stopped, true, e.ErrorMessage));
        }

        public Synchronizer Synchronizer { get; set; }

        public virtual void RefreshStatus() { }

        /// <summary>
        /// 获取当前服务的执行状态名称
        /// </summary>
        protected void UpdateServiceStatus(HostedServiceStatus serviceStatus, bool hasError = false, string errorMessage = null)
        {
            if (serviceStatus == HostedServiceStatus)
                return;

            this.HostedServiceStatus = serviceStatus;

            string statusName;
            Brush statusColor;
            string buttonText;
            bool buttonEnabled;
            string statusTooltip;

            switch (serviceStatus)
            {
                case HostedServiceStatus.Stopped:
                    if (hasError)
                    {
                        statusName = "已停止，有错误";
                        statusTooltip = errorMessage;
                        statusColor = Brushes.Red;
                    }
                    else
                    {
                        statusName = "已停止";
                        statusColor = Brushes.Black;
                        statusTooltip = null;
                    }
                    buttonText = "启动";
                    buttonEnabled = true;
                    break;
                case HostedServiceStatus.Starting:
                    statusName = "正在启动";
                    statusColor = Brushes.LimeGreen;
                    buttonText = "启动";
                    buttonEnabled = false;
                    statusTooltip = null;
                    break;
                case HostedServiceStatus.Running:
                    statusName = "正在运行";
                    statusColor = Brushes.Green;
                    buttonText = "停止";
                    buttonEnabled = true;
                    statusTooltip = null;
                    break;
                case HostedServiceStatus.Stopping:
                    statusName = "正在停止";
                    statusColor = Brushes.DarkGray;
                    buttonText = "停止";
                    buttonEnabled = false;
                    statusTooltip = null;
                    break;
                default:
                    statusName = "未知状态";
                    statusColor = Brushes.Yellow;
                    buttonText = "未知";
                    buttonEnabled = false;
                    statusTooltip = null;
                    break;
            }

            StatusText = statusName;
            StatusColor = statusColor;
            ButtonText = buttonText;
            ButtonEnabled = buttonEnabled;
            StatusToolTip = statusTooltip;

            logInfo(statusName);
        }

        public void Stop()
        {
            if (HostedServiceStatus == HostedServiceStatus.Running)
            {
                operateServiceAsync();
            }
        }

        public void Start()
        {
            if (HostedServiceStatus == HostedServiceStatus.Stopped)
            {
                operateServiceAsync();
            }
        }

        public DelegateCommand OperateServiceCommand { get; }

        private async void operateServiceAsync()
        {
            switch (HostedServiceStatus)
            {
                case HostedServiceStatus.Starting:
                case HostedServiceStatus.Stopping:
                    break;

                case HostedServiceStatus.Running:
                    UpdateServiceStatus(HostedServiceStatus.Stopping);
                    try
                    {
                        await _bindedService.StopAsync();
                    }
                    catch (Exception ex)
                    {
                        logError("停止服务时出错：" + ex.Message, ex);
                        UpdateServiceStatus(HostedServiceStatus.Running, true, ex.Message);
                        break;
                    }
                    UpdateServiceStatus(HostedServiceStatus.Stopped);
                    break;
                case HostedServiceStatus.Stopped:
                    UpdateServiceStatus(HostedServiceStatus.Starting);
                    try
                    {
                        await _bindedService.StartAsync();
                    }
                    catch (Exception ex)
                    {
                        logError("启动服务时出错：" + ex.Message, ex);
                        UpdateServiceStatus(HostedServiceStatus.Stopped, true, ex.Message);
                        break;
                    }
                    UpdateServiceStatus(HostedServiceStatus.Running);
                    break;
                default:
                    break;
            }
        }

        public HostedServiceStatus HostedServiceStatus { get; private set; } = HostedServiceStatus.Stopped;

        public bool IsStopped => HostedServiceStatus == HostedServiceStatus.Stopped;
    }

    public enum HostedServiceStatus
    {
        Starting, Running, Stopping, Stopped
    }
}
