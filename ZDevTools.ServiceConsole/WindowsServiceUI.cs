using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ZDevTools.ServiceCore;
using System.ServiceProcess;

namespace ZDevTools.ServiceConsole
{
    public partial class WindowsServiceUI : HostedServiceUI
    {
        static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(HostedServiceUI));
        void logInfo(string message) => log.Info($"【{DisplayName}】{message}");
        void logError(string message, Exception exception) => log.Error($"【{DisplayName}】{message}", exception);


        public WindowsServiceUI()
        {
            InitializeComponent();
        }

        ServiceController serviceController;
        public override IServiceBase BindedService
        {
            get
            {
                return base.BindedService;
            }

            set
            {
                base.BindedService = value;
                serviceController = new ServiceController(value.ServiceName);
            }
        }
        
        public override void RefreshStatus()
        {
            try
            {
                serviceController.Refresh();

                if (serviceController.Status == ServiceControllerStatus.Running)
                    UpdateServiceStatus(HostedServiceStatus.Running);
                else if (serviceController.Status == ServiceControllerStatus.Stopped)
                    UpdateServiceStatus(HostedServiceStatus.Stopped);

                bool delayedAutoStart;
                ServiceInfo serviceInfo = ServiceHelper.QueryServiceConfig(serviceController.ServiceName, out delayedAutoStart);
                if (serviceInfo.startType == 2 && delayedAutoStart)
                    cbStartupType.SelectedIndex = 0;
                else if (serviceInfo.startType == 2 && !delayedAutoStart)
                    cbStartupType.SelectedIndex = 1;
                else if (serviceInfo.startType == 3)
                    cbStartupType.SelectedIndex = 2;
                else if (serviceInfo.startType == 4)
                    cbStartupType.SelectedIndex = 3;
            }
            catch (Exception ex)
            {
                logError("刷新服务状态出错：" + ex.Message, ex);
            }
        }

        private void bApply_Click(object sender, EventArgs e)
        {
            try
            {
                switch (cbStartupType.SelectedIndex)
                {
                    case 0:
                        ServiceHelper.ChangeStartMode(serviceController.ServiceName, ServiceStartMode.Automatic, true);
                        break;
                    case 1:
                        ServiceHelper.ChangeStartMode(serviceController.ServiceName, ServiceStartMode.Automatic, false);
                        break;
                    case 2:
                        ServiceHelper.ChangeStartMode(serviceController.ServiceName, ServiceStartMode.Manual, false);
                        break;
                    case 3:
                        ServiceHelper.ChangeStartMode(serviceController.ServiceName, ServiceStartMode.Disabled, false);
                        break;
                    default:
                        throw new IndexOutOfRangeException("未选择启动模式");
                }
                logInfo("应用服务启动模式成功");
            }
            catch (Exception ex)
            {
                logError("应用服务启动模式出错：" + ex.Message, ex);
            }

        }

        private void lStartupType_Click(object sender, EventArgs e)
        {
            this.Focus();
        }
    }
}
