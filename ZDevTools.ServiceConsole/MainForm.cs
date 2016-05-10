using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;
using ZDevTools.ServiceCore;
using System.ComponentModel.Composition.Hosting;
using System.Linq;
using static ZDevTools.ServiceConsole.CommonFunctions;
using log4net.Core;
using System.ServiceProcess;
using System.Configuration.Install;
using System.Collections;
using Microsoft.Win32;


namespace ZDevTools.ServiceConsole
{
    public partial class MainForm : Form
    {
        static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(MainForm));
        void logError(string message, Exception exception) => log.Error($"【主界面】{message}", exception);
        void logInfo(string message) => log.Info($"【主界面】{message}");


        const int MaxMessageCount = 1000;
        const int RemoveItemsCount = 300;
        const string configFile = "config.json";

        public static MainForm Instance { get; private set; }

        Dictionary<string, IControllableUI> controllableUIs = new Dictionary<string, IControllableUI>();
        Dictionary<string, IBindedServiceUI> bindedServiceUIs = new Dictionary<string, IBindedServiceUI>();

        AppConfig appConfig;
        public MainForm()
        {
            InitializeComponent();
            this.Text = Properties.Settings.Default.ServiceConsoleTitle;
            niMain.Text = this.Text;

            IServiceBase[] services = GetServicesFromMef();

            //加载/初始化配置
            if (File.Exists(configFile))
            {
                var jsonStr = File.ReadAllText(configFile);
                appConfig = JsonConvert.DeserializeObject<AppConfig>(jsonStr);
            }
            else
                appConfig = new AppConfig();

            if (appConfig.OneKeyStart == null)
                appConfig.OneKeyStart = new Dictionary<string, ServiceItemConfig>();

            if (appConfig.ServicesConfig == null)
                appConfig.ServicesConfig = new Dictionary<string, string>();

            //添加服务到界面
            pDown.SuspendLayout();
            var x = 3;
            var y = 38;

            Color defaultColor = Color.FromArgb(255, 255, 202);
            Color alternateColor = Color.FromArgb(255, 233, 240);

            int i = 0;
            foreach (var service in services)
            {
                Control ui = null;
                if(service is IScheduledService)
                    ui = new ScheduledServiceUI();
                else if(service is  IHostedService)
                {
                     if (service is WindowsServiceBase)
                        ui = new WindowsServiceUI();
                     else
                        ui = new HostedServiceUI();
                }
          

                if (ui == null)
                    continue;

                IBindedServiceUI bindedServiceUI = ui as IBindedServiceUI;
                IConfigurableUI configurableUI = ui as IConfigurableUI;
                IControllableUI controllableUI = ui as IControllableUI;


                ui.Left = x;
                ui.Top = y;
                ui.Width = pDown.Width - x;
                ui.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
                ui.BackColor = i % 2 == 0 ? defaultColor : alternateColor;
                bindedServiceUI.BindedService = service;

                //加载服务配置到界面
                var serviceName = service.ServiceName;
                string jsonString;
                if (configurableUI != null && appConfig.ServicesConfig.TryGetValue(serviceName, out jsonString))
                    configurableUI.LoadConfig(jsonString);

                pDown.Controls.Add(ui);
                y += ui.Height;

                bindedServiceUIs.Add(serviceName, bindedServiceUI);
                if (controllableUI != null)
                    controllableUIs.Add(serviceName, controllableUI);

                i++;
            }

            pDown.ResumeLayout();

            //初始化一键启动配置
            var oneKeyStart = appConfig.OneKeyStart;

            //移除多余配置
            var keys = oneKeyStart.Keys.ToArray();
            foreach (var key in keys)
            {
                if (!controllableUIs.ContainsKey(key))
                    oneKeyStart.Remove(key);
            }

            //添加应有配置
            foreach (var keyValue in controllableUIs)
            {
                ServiceItemConfig serviceItemConfig;
                if (!oneKeyStart.TryGetValue(keyValue.Key, out serviceItemConfig))
                {
                    serviceItemConfig = new ServiceItemConfig();
                    oneKeyStart.Add(keyValue.Key, serviceItemConfig);
                }
                serviceItemConfig.ServiceName = bindedServiceUIs[keyValue.Key].DisplayName;
            }

            updateInstallButton();

            if (containWindowsService && !containNotInstall) //有windows服务并且都已安装到系统中
                refreshServicesStatus();

            MainForm.Instance = this;
        }

        /// <summary>
        /// 包含未安装到系统中的Windows服务
        /// </summary>
        bool containNotInstall;
        /// <summary>
        /// 包含Windows服务
        /// </summary>
        bool containWindowsService;
        private void updateInstallButton()
        {
            var services = GetServicesFromMef();
            var windowsServices = ServiceController.GetServices();
            containWindowsService = false;
            containNotInstall = false;

            foreach (var service in services)
            {
                if (service is WindowsServiceBase)
                {
                    containWindowsService = true;
                    if (!windowsServices.Any(sc => sc.ServiceName == service.ServiceName))
                    {
                        containNotInstall = true;
                        break;
                    }
                }
            }

            bInstall.Visible = containWindowsService;
            bRefreshStatus.Visible = containWindowsService;

            if (containNotInstall)
                bInstall.Text = "安装服务";
            else
                bInstall.Text = "卸载服务";
        }


        static IServiceBase[] services;
        public static IServiceBase[] GetServicesFromMef()
        {
            if (services == null)
            {
                DirectoryCatalog directoryCatalog = new DirectoryCatalog("services");
                CompositionContainer container = new CompositionContainer(directoryCatalog);
                //定义载入的服务
                services = (from l in container.GetExports<IServiceBase, IServiceMetadata>()
                            orderby l.Metadata.DisplayOrder
                            select l.Value).ToArray();
            }
            return services;
        }

        /// <summary>
        /// 输出日志【允许非UI线程直接调用】
        /// </summary>
        /// <param name="message">要输出的消息</param>
        public void OutputLog(MessageItem message)
        {
            var handler = new Action(() =>
            {
                lbConsole.Items.Insert(0, message);
                if (lbConsole.Items.Count > MaxMessageCount)
                {
                    for (int i = MaxMessageCount; i >= MaxMessageCount - RemoveItemsCount; i--)
                        lbConsole.Items.RemoveAt(i);
                }
            });

            if (this.InvokeRequired)
                this.BeginInvoke(handler);
            else
                handler();
        }

        private void bDisableAll_Click(object sender, EventArgs e)
        {
            foreach (var controllableUI in controllableUIs.Values)
                controllableUI.Stop();
            logInfo("已停止所有服务");
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
#if !DEBUG
            if (e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;
                this.Visible = false;
            }
#endif
            if (!e.Cancel)
            {
                //收集配置到字典
                var dic = new Dictionary<string, string>();
                foreach (Control control in pDown.Controls)
                {
                    var bindedServiceUI = control as IBindedServiceUI;
                    var configurableUI = control as IConfigurableUI;

                    if (configurableUI != null)
                        dic.Add(bindedServiceUI.BindedService.ServiceName, configurableUI.SaveConfig());
                }
                //保存配置到文件
                appConfig.ServicesConfig = dic;

                var configStr = JsonConvert.SerializeObject(appConfig);
                File.WriteAllText(configFile, configStr);

                MainForm.Instance = null;
            }
        }

        private void niMain_DoubleClick(object sender, EventArgs e)
        {
            this.Visible = !this.Visible;
            if (Visible)
            {
                this.WindowState = FormWindowState.Normal;
                this.Focus();
            }
        }

        private void 退出ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            bool isAllStopped = checkIsAllStopped();

            if (isAllStopped || ShowConfirm("某些服务还未停止，真的要退出吗？"))
                Application.Exit();
        }

        private bool checkIsAllStopped()
        {
            bool isAllStopped = true;
            foreach (var controllableUI in controllableUIs.Values)
                if (!controllableUI.IsStopped)
                {
                    isAllStopped = false;
                    break;
                }

            return isAllStopped;
        }

        private void lbConsole_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.C)
            {
                if (lbConsole.SelectedItem != null)
                {
                    try
                    {
                        Clipboard.Clear();//设置文本前必须先清空剪贴板，否则可能会报错
                        Clipboard.SetText(lbConsole.SelectedItem.ToString() + Environment.NewLine);
                    }
                    catch (Exception ex)
                    {
                        logError("剪贴板错误：" + ex.Message, ex);
                    }
                }
            }
        }

        private void bOneKeyStart_Click(object sender, EventArgs e)
        {
            var shouldOneKeyStartServiceKeys = (from keyValue in appConfig.OneKeyStart where keyValue.Value.OneKeyStart select keyValue.Key).ToArray();

            if (shouldOneKeyStartServiceKeys.Length > 0)
            {
                foreach (var key in shouldOneKeyStartServiceKeys)
                    controllableUIs[key].Start();
                logInfo("完成一键启动操作");
            }
            else
                CommonFunctions.ShowMessage("请先点击左侧按钮设置需要一键启动的服务!");
        }

        private void bConfigOneKeyStart_Click(object sender, EventArgs e)
        {
            //弹出配置窗口
            using (var form = new OneKeyStartConfigForm())
            {
                form.Configs = appConfig.OneKeyStart;
                form.ShowDialog();
            }
        }

        private void lbConsole_DrawItem(object sender, DrawItemEventArgs e)
        {
            Brush brush = null;

            ListBox listBox = sender as ListBox;

            if (e.Index > -1)
            {
                var item = listBox.Items[e.Index] as MessageItem;

                var level = item.Level;

                if (level == Level.Info)
                    brush = new SolidBrush(e.ForeColor);
                else if (level == Level.Warn)
                    brush = Brushes.DarkOrange;
                else if (level == Level.Error)
                    brush = Brushes.Red;
                else if (level == Level.Debug)
                    brush = Brushes.Blue;
                else if (level == Level.Fatal)
                    brush = Brushes.DarkRed;

                e.DrawBackground();

                e.Graphics.DrawString(item.ToString(), e.Font, brush, e.Bounds);

                e.DrawFocusRectangle();
            }
        }

        private void bInstall_Click(object sender, EventArgs e)
        {
            try
            {
                if (containNotInstall) //安装
                {
                    if (ShowConfirm("如果您之前执行过安装操作，请先使用原程序卸载服务再使用本程序执行安装操作！"))
                    {
                        using (TransactedInstaller transactedInstaller = new TransactedInstaller())
                        {
                            var installLog = new Hashtable();
                            AssemblyInstaller assemblyInstaller = new AssemblyInstaller(Application.ExecutablePath, null);
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
                            AssemblyInstaller assemblyInstaller = new AssemblyInstaller(Application.ExecutablePath, null);
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
                    tsmiExit.PerformClick();

                    //创建启动对象
                    System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
                    startInfo.UseShellExecute = true;
                    startInfo.WorkingDirectory = Environment.CurrentDirectory;
                    startInfo.FileName = Application.ExecutablePath;
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

        private void bRefreshStatus_Click(object sender, EventArgs e)
        {
            refreshServicesStatus();
            logInfo("已刷新所有Windows服务状态");
        }

        private void refreshServicesStatus()
        {
            foreach (var controllableUI in controllableUIs.Values)
                controllableUI.RefreshStatus();
        }
    }
}