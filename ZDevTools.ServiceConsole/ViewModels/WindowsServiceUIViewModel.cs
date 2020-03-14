using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using ZDevTools.ServiceCore;
using Microsoft.Extensions.Logging;
using ReactiveUI;
using System.Reactive;
using ReactiveUI.Fody.Helpers;

namespace ZDevTools.ServiceConsole.ViewModels
{
    public class WindowsServiceUIViewModel : HostedServiceUIViewModel
    {
        readonly ILogger<WindowsServiceUIViewModel> Logger;

        void logInfo(string message) => Logger.LogInformation($"【{DisplayName}】{message}");
        void logError(Exception exception, string message) => Logger.LogError($"【{DisplayName}】{message}", exception);

        public WindowsServiceUIViewModel(ILogger<WindowsServiceUIViewModel> logger, ILogger<HostedServiceUIViewModel> logger2)
            : base(logger2)
        {
            this.Logger = logger;


            ApplyCommand = ReactiveCommand.Create(() =>
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
                   logError(ex, "应用服务启动模式出错：" + ex.Message);
               }
           });
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

        [Reactive]
        public int StartupTypeIndex { get; set; }

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
                logError(ex, "刷新服务状态出错：" + ex.Message);
            }
        }

        public ReactiveCommand<Unit, Unit> ApplyCommand { get; }
    }
}
