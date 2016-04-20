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
        void logInfo(string message) => log.Info($"【{DisplayName}】{message}");
        void logError(string message, Exception exception) => log.Error($"【{DisplayName}】{message}", exception);

        public HostedServiceUI()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 服务名称
        /// </summary>
        public string DisplayName { get { return lServiceName.Text; } }

        IHostedService bindedService;
        public virtual IServiceBase BindedService
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
                lServiceName.Text = value.DisplayName;
                bindedService.Faulted += bindedService_Faulted;
            }
        }

        private void bindedService_Faulted(object sender, EventArgs e)
        {
            if (InvokeRequired)
                Invoke(new MethodInvoker(() => { UpdateServiceStatus(HostedServiceStatus.Stopped, true); }));
            else
                UpdateServiceStatus(HostedServiceStatus.Stopped, true);
        }

        public void Stop()
        {
            if (HostedServiceStatus == HostedServiceStatus.Running)
            {
                bOperation.PerformClick();
            }
        }
        public void Start()
        {
            if (HostedServiceStatus == HostedServiceStatus.Stopped)
            {
                bOperation.PerformClick();
            }
        }

        /// <summary>
        /// 获取当前服务的执行状态名称
        /// </summary>
        protected void UpdateServiceStatus(HostedServiceStatus serviceStatus, bool hasError = false)
        {
            if (serviceStatus == HostedServiceStatus)
                return;

            this.HostedServiceStatus = serviceStatus;

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
                        statusColor = Color.Red;
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
                    statusColor = Color.LimeGreen;
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
            switch (HostedServiceStatus)
            {
                case HostedServiceStatus.Starting:
                case HostedServiceStatus.Stopping:
                    break;

                case HostedServiceStatus.Running:
                    UpdateServiceStatus(HostedServiceStatus.Stopping);
                    try
                    {
                        await bindedService.StopAsync();
                    }
                    catch (Exception ex)
                    {
                        logError("停止服务时出错：" + ex.Message, ex);
                        UpdateServiceStatus(HostedServiceStatus.Running, true);
                        break;
                    }
                    UpdateServiceStatus(HostedServiceStatus.Stopped);
                    break;
                case HostedServiceStatus.Stopped:
                    UpdateServiceStatus(HostedServiceStatus.Starting);
                    try
                    {
                        await bindedService.StartAsync();
                    }
                    catch (Exception ex)
                    {
                        logError("启动服务时出错：" + ex.Message, ex);
                        UpdateServiceStatus(HostedServiceStatus.Stopped, true);
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

        private void lServiceName_Click(object sender, EventArgs e)
        {
            this.Focus();
        }

        public virtual void RefreshStatus() { }
    }

    public enum HostedServiceStatus
    {
        Starting, Running, Stopping, Stopped
    }
}
