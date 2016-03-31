using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json;
using ZDevTools.ServiceCore;

namespace ZDevTools.ServiceConsole
{
    public partial class HostedServiceUI : UserControl, IBindedServiceUI, IControllableUI
    {
        static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(HostedServiceUI));
        void logInfo(string message) => log.Info($"【{ServiceName}】{message}");
        void logError(string message, Exception exception) => log.Error($"【{ServiceName}】{message}", exception);


        public HostedServiceUI()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 服务名称
        /// </summary>
        public string ServiceName { get { return lServiceName.Text; } }

        IHostedService bindedService;
        public IServiceBase BindedService
        {
            get
            {
                return bindedService;
            }
            set
            {
                if (bindedService != null)
                    throw new InvalidOperationException("不支持为该控件多次绑定服务！");

                bindedService = (IHostedService)value;
                lServiceName.Text = value.ServiceName;
                bindedService.Faulted += bindedService_Faulted;
            }
        }

        private void bindedService_Faulted(object sender, EventArgs e)
        {
            if (InvokeRequired)
                Invoke(new MethodInvoker(() => { updateServiceStatus(HostedServiceStatus.Stopped, true); }));
            else
                updateServiceStatus(HostedServiceStatus.Stopped, true);
        }

        public void Stop()
        {
            if (ServiceHostStatus == HostedServiceStatus.Running)
            {
                bOperation.PerformClick();
            }
        }
        public void Start()
        {
            if (ServiceHostStatus == HostedServiceStatus.Stopped)
            {
                bOperation.PerformClick();
            }
        }

        /// <summary>
        /// 获取当前服务的执行状态名称
        /// </summary>
        void updateServiceStatus(HostedServiceStatus serviceStatus, bool hasError = false)
        {
            this.ServiceHostStatus = serviceStatus;

            string statusName;
            Color statusColor;
            string buttonText;
            bool buttonEnabled;

            switch (serviceStatus)
            {
                case HostedServiceStatus.Stopped:
                    if (hasError)
                    {
                        statusName = "已停止，有错误";
                        statusColor = Color.Yellow;
                    }
                    else
                    {
                        statusName = "已停止";
                        statusColor = Color.Black;
                    }
                    buttonText = "启动";
                    buttonEnabled = true;
                    break;
                case HostedServiceStatus.Starting:
                    statusName = "正在启动";
                    statusColor = Color.LightGreen;
                    buttonText = "启动";
                    buttonEnabled = false;
                    break;
                case HostedServiceStatus.Running:
                    statusName = "正在运行";
                    statusColor = Color.Green;
                    buttonText = "停止";
                    buttonEnabled = true;
                    break;
                case HostedServiceStatus.Stopping:
                    statusName = "正在停止";
                    statusColor = Color.DarkGray;
                    buttonText = "停止";
                    buttonEnabled = false;
                    break;
                default:
                    statusName = "未知状态";
                    statusColor = Color.Yellow;
                    buttonText = "未知";
                    buttonEnabled = false;
                    break;
            }

            lStatus.Text = statusName;
            lStatus.ForeColor = statusColor;
            bOperation.Text = buttonText;
            bOperation.Enabled = buttonEnabled;

            logInfo(statusName);
        }

        private async void bOperation_Click(object sender, EventArgs e)
        {
            switch (ServiceHostStatus)
            {
                case HostedServiceStatus.Starting:
                case HostedServiceStatus.Stopping:
                    break;

                case HostedServiceStatus.Running:
                    updateServiceStatus(HostedServiceStatus.Stopping);
                    try
                    {
                        await bindedService.StopAsync();
                    }
                    catch (Exception ex)
                    {
                        logError("停止服务时出错：" + ex.Message, ex);
                    }
                    updateServiceStatus(HostedServiceStatus.Stopped);
                    break;
                case HostedServiceStatus.Stopped:
                    updateServiceStatus(HostedServiceStatus.Starting);
                    try
                    {
                        await bindedService.StartAsync();
                    }
                    catch (Exception ex)
                    {
                        logError("启动服务时出错：" + ex.Message, ex);
                    }
                    updateServiceStatus(HostedServiceStatus.Running);
                    break;
                default:
                    break;
            }
        }


        public HostedServiceStatus ServiceHostStatus { get; private set; } = HostedServiceStatus.Stopped;

        public bool IsStopped => ServiceHostStatus == HostedServiceStatus.Stopped;

        private void lServiceName_Click(object sender, EventArgs e)
        {
            this.Focus();
        }
    }

    public enum HostedServiceStatus
    {
        Starting, Running, Stopping, Stopped
    }
}
