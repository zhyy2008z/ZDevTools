using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using Prism.Commands;

namespace ZDevTools.ServiceConsole.ViewModels
{
    using ServiceCore;

    public class WindowsServiceUIViewModel : HostedServiceUIViewModel
    {

        static readonly log4net.ILog Log = log4net.LogManager.GetLogger(typeof(WindowsServiceUIViewModel));
        void logInfo(string message) => Log.Info($"【{DisplayName}】{message}");
        void logError(string message, Exception exception) => Log.Error($"【{DisplayName}】{message}", exception);

        public WindowsServiceUIViewModel()
        {
            ApplyCommand = new DelegateCommand(raiseApply);
        }

        ServiceController _serviceController;
        public override IServiceBase BindedService
        {
            get
            {
                return base.BindedService;
            }
            set
            {
                base.BindedService = value;
                _serviceController = new ServiceController(value.ServiceName);
            }
        }

        int _startupTypeIndex;
        public int StartupTypeIndex { get { return _startupTypeIndex; } set { SetProperty(ref _startupTypeIndex, value); } }

        public override void RefreshStatus()
        {
            try
            {
                _serviceController.Refresh();

                if (_serviceController.Status == ServiceControllerStatus.Running)
                    UpdateServiceStatus(HostedServiceStatus.Running);
                else if (_serviceController.Status == ServiceControllerStatus.Stopped)
                    UpdateServiceStatus(HostedServiceStatus.Stopped);

                ServiceInfo serviceInfo = ServiceHelper.QueryServiceConfig(_serviceController.ServiceName, out bool delayedAutoStart);
                if (serviceInfo.startType == 2 && delayedAutoStart)
                    StartupTypeIndex = 0;
                else if (serviceInfo.startType == 2 && !delayedAutoStart)
                    StartupTypeIndex = 1;
                else if (serviceInfo.startType == 3)
                    StartupTypeIndex = 2;
                else if (serviceInfo.startType == 4)
                    StartupTypeIndex = 3;
            }
            catch (Exception ex)
            {
                logError("刷新服务状态出错：" + ex.Message, ex);
            }
        }

        public DelegateCommand ApplyCommand { get; }

        private void raiseApply()
        {
            try
            {
                switch (StartupTypeIndex)
                {
                    case 0:
                        ServiceHelper.ChangeStartMode(_serviceController.ServiceName, ServiceStartMode.Automatic, true);
                        break;
                    case 1:
                        ServiceHelper.ChangeStartMode(_serviceController.ServiceName, ServiceStartMode.Automatic, false);
                        break;
                    case 2:
                        ServiceHelper.ChangeStartMode(_serviceController.ServiceName, ServiceStartMode.Manual, false);
                        break;
                    case 3:
                        ServiceHelper.ChangeStartMode(_serviceController.ServiceName, ServiceStartMode.Disabled, false);
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
    }
}
