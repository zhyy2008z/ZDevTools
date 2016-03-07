using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using System.IO;
using Newtonsoft.Json;
using ZDevTools.ServiceCore;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Linq;
using static ZDevTools.ServiceConsole.CommonFunctions;

namespace ZDevTools.ServiceConsole
{
    public partial class MainForm : Form
    {
        const int MaxMessageCount = 1000;
        const int RemoveItemsCount = 300;
        const string configFile = "config.json";

        public static MainForm Instance { get; private set; }

        List<IControllableUI> controllableUIs = new List<IControllableUI>();

        public MainForm()
        {
            InitializeComponent();
            this.Text = Properties.Settings.Default.ServiceConsoleTitle;
            niMain.Text = this.Text;
            DirectoryCatalog directoryCatalog = new DirectoryCatalog("services");
            CompositionContainer container = new CompositionContainer(directoryCatalog);

            //定义载入的服务
            IServiceBase[] services = (from l in container.GetExports<IServiceBase, IServiceMetadata>()
                                       orderby l.Metadata.DisplayOrder
                                       select l.Value).ToArray();

            //从文件读取配置
            Dictionary<string, string> config = null;
            if (File.Exists(configFile))
            {
                var jsonStr = File.ReadAllText(configFile);
                config = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonStr);
            }

            //添加服务到界面
            pDown.SuspendLayout();
            var x = 3;
            var y = 38;

            Color defaultColor = Color.FromArgb(110, 220, 110);
            Color alternateColor = Color.FromArgb(110, 200, 110);

            int i = 0;
            foreach (var service in services)
            {
                Control ui = null;

                if (service is IScheduledService)
                    ui = new ScheduledServiceUI();
                else if (service is IHostedService)
                    ui = new HostedServiceUI();

                IBindedServiceUI bindedServiceUI = ui as IBindedServiceUI;
                IConfigurableUI configurableUI = ui as IConfigurableUI;

                ui.Left = x;
                ui.Top = y;
                ui.Width = pDown.Width - x;
                ui.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
                ui.BackColor = i % 2 == 0 ? defaultColor : alternateColor;
                bindedServiceUI.BindedService = service;

                //加载服务配置到界面
                var serviceName = service.GetType().Name;
                string jsonString;
                if (configurableUI != null && config != null && config.TryGetValue(serviceName, out jsonString))
                    configurableUI.LoadConfig(jsonString);

                pDown.Controls.Add(ui);
                y += ui.Height;

                controllableUIs.Add((IControllableUI)ui);
                i++;
            }

            pDown.ResumeLayout();

            MainForm.Instance = this;
        }

        /// <summary>
        /// 输出日志【允许非UI线程直接调用】
        /// </summary>
        /// <param name="message">要输出的消息</param>
        public void OutputLog(string message)
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
            foreach (var controllableUI in controllableUIs)
                controllableUI.Stop();
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
                        dic.Add(bindedServiceUI.BindedService.GetType().Name, configurableUI.SaveConfig());
                }
                //保存配置到文件
                var configStr = JsonConvert.SerializeObject(dic);
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
            bool isAllStopped = true;
            foreach (var controllableUI in controllableUIs)
                if (!controllableUI.IsStopped)
                {
                    isAllStopped = false;
                    break;
                }

            if (isAllStopped || ShowConfirm("某些服务还未停止，真的要退出吗？"))
                Application.Exit();
        }
    }
}
