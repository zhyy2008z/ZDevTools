using System;
using System.Windows.Media;
using ReactiveUI;
using ZDevTools.ServiceCore;
using Microsoft.Extensions.Logging;
using ReactiveUI.Fody.Helpers;
using System.Reactive.Linq;
using System.Reactive;

namespace ZDevTools.ServiceConsole.ViewModels
{
    public class HostedServiceUIViewModel : ServiceViewModelBase
    {
        readonly ILogger<HostedServiceUIViewModel> Logger;
        void logInfo(string message) => Logger.LogInformation($"【{DisplayName}】{message}");
        void logError(Exception exception, string message) => Logger.LogError(exception, $"【{DisplayName}】{message}");

        public HostedServiceUIViewModel(ILogger<HostedServiceUIViewModel> logger)
        {
            Logger = logger;
            OperateServiceCommand = ReactiveCommand.Create(operateServiceAsync);
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
            UpdateServiceStatus(HostedServiceStatus.Stopped, true, e.ErrorMessage);
        }

        public override void RefreshStatus() { }

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

        public override void Stop()
        {
            if (HostedServiceStatus == HostedServiceStatus.Running)
            {
                operateServiceAsync();
            }
        }

        public override void Start()
        {
            if (HostedServiceStatus == HostedServiceStatus.Stopped)
            {
                operateServiceAsync();
            }
        }

        public ReactiveCommand<Unit, Unit> OperateServiceCommand { get; }

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
                        logError(ex, "停止服务时出错：" + ex.Message);
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
                        logError(ex, "启动服务时出错：" + ex.Message);
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

        public override bool IsStopped => HostedServiceStatus == HostedServiceStatus.Stopped;
    }

    public enum HostedServiceStatus
    {
        Starting, Running, Stopping, Stopped
    }
}
