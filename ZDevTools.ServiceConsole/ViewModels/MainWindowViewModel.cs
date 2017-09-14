using System;
using Prism.Mvvm;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using FSLib.App.SimpleUpdater;
using System.ComponentModel.Composition.Hosting;
using System.Linq;
using System.ServiceProcess;
using System.Configuration.Install;
using log4net.Core;
using System.Collections;
using static ZDevTools.ServiceConsole.CommonFunctions;
using System.IO;
using Newtonsoft.Json;
using System.Timers;
using Prism.Commands;
using System.Windows;


namespace ZDevTools.ServiceConsole.ViewModels
{
    using Views;
    using ServiceCore;
    using DIServices;

    public class MainWindowViewModel : BindableBase, IDisposable
    {
        static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(MainWindowViewModel));
        void logError(string message, Exception exception) => log.Error($"【主界面】{message}", exception);
        void logInfo(string message) => log.Info($"【主界面】{message}");


        public Synchronizer Synchronizer { get; set; }

        public MainWindow Window { get; set; }


        private string _title;
        public string Title { get { return _title; } set { SetProperty(ref _title, value); } }


        ObservableCollection<MessageItem> _messages = new ObservableCollection<MessageItem>();
        public ObservableCollection<MessageItem> Messages { get { return _messages; } set { SetProperty(ref _messages, value); } }


        MessageItem _selectedMessage;
        public MessageItem SelectedMessage { get { return _selectedMessage; } set { SetProperty(ref _selectedMessage, value); } }

        ObservableCollection<ServiceViewModelBase> _serviceViewModels;
        public ObservableCollection<ServiceViewModelBase> ServiceViewModels { get { return _serviceViewModels; } set { SetProperty(ref _serviceViewModels, value); } }


        const int MaxMessageCount = 1000;
        const int RemoveItemsCount = 300;
        const string ConfigFile = "config.json";


        Dictionary<string, IControllableUI> _controllableUIs = new Dictionary<string, IControllableUI>();
        Dictionary<string, IBindedServiceUI> _bindedServiceUIs = new Dictionary<string, IBindedServiceUI>();

        AppConfig _appConfig;
        Updater _updater;



        System.Windows.Forms.NotifyIcon _niMain = new System.Windows.Forms.NotifyIcon();
        System.Windows.Forms.ContextMenu _contextMenu = new System.Windows.Forms.ContextMenu();
        IDialogs _dialogs;
        Timer _autoUpdateTimer;
        public MainWindowViewModel(IDialogs dialogs)
        {
            _dialogs = dialogs;
            StopAllCommand = new DelegateCommand(stopAll);
            CopyMessageCommand = new DelegateCommand(copyMessage);
            DoOneKeyStartCommand = new DelegateCommand(doOneKeyStart);
            ConfigOneKeyStartCommand = new DelegateCommand(configOneKeyStart);
            InstallCommand = new DelegateCommand(install);
            RefreshStatusCommand = new DelegateCommand(refreshStatus);

            Title = Properties.Settings.Default.ServiceConsoleTitle;

            _niMain.Icon = new System.Drawing.Icon("server.ico");
            _niMain.Visible = true;
            _niMain.DoubleClick += _niMain_DoubleClick;
            _niMain.Text = Title;
            _niMain.ContextMenu = _contextMenu;
            _contextMenu.MenuItems.Add("退出").Click += (sender, e) =>
            {
                bool isAllStopped = checkIsAllStopped();

                if (isAllStopped || ShowConfirm("某些服务还未停止，真的要退出吗？"))
                {
                    CanClose = true;
                    Window.Close();
                }
            };


            IServiceBase[] services = GetServicesFromMef();

            //加载/初始化配置
            if (File.Exists(ConfigFile))
            {
                var jsonStr = File.ReadAllText(ConfigFile);
                _appConfig = JsonConvert.DeserializeObject<AppConfig>(jsonStr);
            }
            else
                _appConfig = new AppConfig();

            if (_appConfig.OneKeyStart == null)
                _appConfig.OneKeyStart = new Dictionary<string, ServiceItemConfig>();

            if (_appConfig.ServicesConfig == null)
                _appConfig.ServicesConfig = new Dictionary<string, string>();


            List<ServiceViewModelBase> serviceViewModels = new List<ServiceViewModelBase>();

            int i = 0;
            foreach (var service in services)
            {
                ServiceViewModelBase ui = null;

                if (service is IScheduledService)
                    ui = new ScheduledServiceUIViewModel(dialogs);
                else if (service is IHostedService)
                {
                    if (service is WindowsServiceBase)
                        ui = new WindowsServiceUIViewModel();
                    else
                        ui = new HostedServiceUIViewModel();
                }

                if (ui == null)
                    continue;

                IBindedServiceUI bindedServiceUI = ui as IBindedServiceUI;
                IConfigurableUI configurableUI = ui as IConfigurableUI;
                IControllableUI controllableUI = ui as IControllableUI;

                bindedServiceUI.BindedService = service;

                //加载服务配置到界面
                var serviceName = service.ServiceName;
                if (configurableUI != null && _appConfig.ServicesConfig.TryGetValue(serviceName, out string jsonString))
                    configurableUI.LoadConfig(jsonString);

                _bindedServiceUIs.Add(serviceName, bindedServiceUI);
                if (controllableUI != null)
                    _controllableUIs.Add(serviceName, controllableUI);

                serviceViewModels.Add(ui);

                i++;
            }

            ServiceViewModels = new ObservableCollection<ServiceViewModelBase>(serviceViewModels);


            //初始化一键启动配置
            var oneKeyStart = _appConfig.OneKeyStart;

            //移除多余配置
            var keys = oneKeyStart.Keys.ToArray();
            foreach (var key in keys)
            {
                if (!_controllableUIs.ContainsKey(key))
                    oneKeyStart.Remove(key);
            }

            //添加应有配置
            foreach (var keyValue in _controllableUIs)
            {
                if (!oneKeyStart.TryGetValue(keyValue.Key, out ServiceItemConfig serviceItemConfig))
                {
                    serviceItemConfig = new ServiceItemConfig();
                    oneKeyStart.Add(keyValue.Key, serviceItemConfig);
                }
                serviceItemConfig.ServiceName = _bindedServiceUIs[keyValue.Key].DisplayName;
            }

            updateInstallButton();

            if (_containWindowsService && !_containNotInstall) //有windows服务并且都已安装到系统中
                refreshServicesStatus();

            if (!string.IsNullOrEmpty(Properties.Settings.Default.AutoUpdateUri))
            {
                _updater = Updater.CreateUpdaterInstance(Properties.Settings.Default.AutoUpdateUri, "update_c.xml");
                _autoUpdateTimer = new Timer(Properties.Settings.Default.AutoUpdateInterval * 1000);
                _autoUpdateTimer.Elapsed += (sender, e) =>
                {
                    //自动升级检查
                    _updater.CheckUpdateSync();

                };
                _autoUpdateTimer.Start();
            }
        }

        bool _showInstallButton;
        public bool ShowInstallButton { get { return _showInstallButton; } set { SetProperty(ref _showInstallButton, value); } }

        bool _showRefreshButton;
        public bool ShowRefreshButton { get { return _showRefreshButton; } set { SetProperty(ref _showRefreshButton, value); } }

        string _installButtonText;
        public string InstallButtonText { get { return _installButtonText; } set { SetProperty(ref _installButtonText, value); } }

        /// <summary>
        /// 包含未安装到系统中的Windows服务
        /// </summary>
        bool _containNotInstall;
        /// <summary>
        /// 包含Windows服务
        /// </summary>
        bool _containWindowsService;
        private void updateInstallButton()
        {
            var services = GetServicesFromMef();
            var windowsServices = ServiceController.GetServices();
            _containWindowsService = false;
            _containNotInstall = false;

            foreach (var service in services)
            {
                if (service is WindowsServiceBase)
                {
                    _containWindowsService = true;
                    if (!windowsServices.Any(sc => sc.ServiceName == service.ServiceName))
                    {
                        _containNotInstall = true;
                        break;
                    }
                }
            }

            ShowInstallButton = _containWindowsService;
            ShowRefreshButton = _containWindowsService;

            if (_containNotInstall)
                InstallButtonText = "安装服务";
            else
                InstallButtonText = "卸载服务";
        }


        static IServiceBase[] _services;
        public static IServiceBase[] GetServicesFromMef()
        {
            if (_services == null)
            {
                DirectoryCatalog directoryCatalog = new DirectoryCatalog("services");
                CompositionContainer container = new CompositionContainer(directoryCatalog);
                //定义载入的服务
                _services = (from l in container.GetExports<IServiceBase, IServiceMetadata>()
                             orderby l.Metadata.DisplayOrder
                             select l.Value).ToArray();
            }
            return _services;
        }

        /// <summary>
        /// 输出日志【允许非UI线程直接调用】
        /// </summary>
        /// <param name="message">要输出的消息</param>
        public void OutputLog(MessageItem message)
        {
            Synchronizer.Invoke(() =>
            {
                Messages.Insert(0, message);
                if (Messages.Count > MaxMessageCount)
                {
                    for (int i = MaxMessageCount; i >= MaxMessageCount - RemoveItemsCount; i--)
                        Messages.RemoveAt(i);
                }
            });
        }


        public DelegateCommand StopAllCommand { get; }
        private void stopAll()
        {
            foreach (var controllableUI in _controllableUIs.Values)
                controllableUI.Stop();
            logInfo("已停止所有服务");
        }

        public bool CanClose { get; private set; }

        public void SaveConfig()
        {
            //收集配置到字典
            var dic = new Dictionary<string, string>();
            foreach (ServiceViewModelBase serviceViewModel in ServiceViewModels)
            {
                var bindedServiceUI = serviceViewModel as IBindedServiceUI;
                var configurableUI = serviceViewModel as IConfigurableUI;

                if (configurableUI != null)
                    dic.Add(bindedServiceUI.BindedService.ServiceName, configurableUI.SaveConfig());
            }
            //保存配置到文件
            _appConfig.ServicesConfig = dic;

            var configStr = JsonConvert.SerializeObject(_appConfig);
            File.WriteAllText(ConfigFile, configStr);
        }

        private void _niMain_DoubleClick(object sender, EventArgs e)
        {
            if (Window.IsVisible)
                Window.Hide();
            else
            {
                Window.Show();
                Window.Activate();
            }
        }


        private bool checkIsAllStopped()
        {
            bool isAllStopped = true;
            foreach (var controllableUI in _controllableUIs.Values)
                if (!controllableUI.IsStopped)
                {
                    isAllStopped = false;
                    break;
                }

            return isAllStopped;
        }

        public DelegateCommand CopyMessageCommand { get; }
        void copyMessage()
        {
            if (SelectedMessage != null)
            {
                try
                {
                    Clipboard.Clear();//设置文本前必须先清空剪贴板，否则可能会报错
                    Clipboard.SetText(SelectedMessage.Content + Environment.NewLine);
                }
                catch (Exception ex)
                {
                    logError("剪贴板错误：" + ex.Message, ex);
                }
            }
        }

        public DelegateCommand DoOneKeyStartCommand { get; }
        private void doOneKeyStart()
        {
            var shouldOneKeyStartServiceKeys = (from keyValue in _appConfig.OneKeyStart where keyValue.Value.OneKeyStart select keyValue.Key).ToArray();

            if (shouldOneKeyStartServiceKeys.Length > 0)
            {
                foreach (var key in shouldOneKeyStartServiceKeys)
                    _controllableUIs[key].Start();
                logInfo("完成一键启动操作");
            }
            else
                ShowMessage("请先点击左侧按钮设置需要一键启动的服务!");
        }


        public DelegateCommand ConfigOneKeyStartCommand { get; }
        private void configOneKeyStart()
        {
            //弹出配置窗口
            _dialogs.ShowOneKeyStartConfigDialog(_appConfig.OneKeyStart);
        }

        public DelegateCommand InstallCommand { get; }
        private void install()
        {
            try
            {
                if (_containNotInstall) //安装
                {
                    if (ShowConfirm("如果您之前执行过安装操作，请先使用原程序卸载服务再使用本程序执行安装操作！"))
                    {
                        using (TransactedInstaller transactedInstaller = new TransactedInstaller())
                        {
                            var installLog = new Hashtable();
                            AssemblyInstaller assemblyInstaller = new AssemblyInstaller(System.Windows.Forms.Application.ExecutablePath, null);
                            transactedInstaller.Installers.Add(assemblyInstaller);
                            transactedInstaller.Install(installLog);
                        }


                        //修改ImagePath
                        foreach (var service in GetServicesFromMef())
                        {
                            if (service is WindowsServiceBase)
                            {
                                bool delayedAutoStart;
                                ServiceInfo serviceInfo = ServiceHelper.QueryServiceConfig(service.ServiceName, out delayedAutoStart);
                                ServiceHelper.ChangeExePath(service.ServiceName, $"{serviceInfo.binaryPathName} -Daemon {service.ServiceName}");
                            }
                        }
                        updateInstallButton();

                        logInfo("安装所有Windows服务成功");
                    }
                }
                else //卸载
                {
                    if (ShowConfirm("确定要卸载本控制台中的所有Windows服务吗？"))
                    {
                        using (TransactedInstaller transactedInstaller = new TransactedInstaller())
                        {
                            AssemblyInstaller assemblyInstaller = new AssemblyInstaller(System.Windows.Forms.Application.ExecutablePath, null);
                            transactedInstaller.Installers.Add(assemblyInstaller);
                            transactedInstaller.Uninstall(null);
                        }
                        updateInstallButton();

                        logInfo("卸载所有Windows服务成功");
                    }
                }
            }
            catch (Exception ex)
            {
                logError("操作失败：" + ex.Message, ex);
                if (ShowConfirm($"操作失败【{ex.Message}】，是否以管理员权限重启本程序以执行操作？"))
                {
                    //退出
                    _niMain.ContextMenu.MenuItems[0].PerformClick();

                    //创建启动对象
                    System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
                    startInfo.UseShellExecute = true;
                    startInfo.WorkingDirectory = Environment.CurrentDirectory;
                    startInfo.FileName = System.Windows.Forms.Application.ExecutablePath;
                    //设置启动动作,确保以管理员身份运行
                    startInfo.Verb = "runas";
                    try
                    {
                        System.Diagnostics.Process.Start(startInfo);
                    }
                    catch { }
                }

            }
        }

        public DelegateCommand RefreshStatusCommand { get; }
        private void refreshStatus()
        {
            refreshServicesStatus();
            logInfo("已刷新所有Windows服务状态");
        }

        private void refreshServicesStatus()
        {
            foreach (var controllableUI in _controllableUIs.Values)
                controllableUI.RefreshStatus();
        }

        // Track whether Dispose has been called.
        bool _disposed = false;

        /// <summary>
        /// 释放资源内部包装方法
        /// </summary>
        /// <param name="disposing">true值表示用户直接或间接调用了这个方法，释放托管和非托管资源；false值表示垃圾回收器调用该方法，仅释放非托管资源</param>
        void dispose(bool disposing)
        {
            // Check to see if Dispose has already been called.
            if (!_disposed)
            {
                Dispose(disposing);
                _disposed = true;
            }
        }

        /// <summary>
        /// 只能在包装器（dispose）中调用此方法，其它地方请勿调用
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing)
        {
            // If disposing equals true, dispose all managed
            // and unmanaged resources.
            if (disposing)
            {
                _niMain.Dispose();
                _contextMenu.Dispose();
                _autoUpdateTimer?.Dispose();
            }

            // Call the appropriate methods to clean up
            // unmanaged resources here.
        }

        public void Dispose()
        {
            //手动释放资源
            this.dispose(true);
            //请求系统不要调用该对象的析构函数。
            GC.SuppressFinalize(this);
        }

        /// <summary>
        ///	垃圾回收器调用该析构函数释放非托管资源
        /// </summary>
        ~MainWindowViewModel()
        {
            this.dispose(false);
        }
    }
}
