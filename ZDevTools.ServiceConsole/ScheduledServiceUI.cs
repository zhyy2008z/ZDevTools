using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using log4net;
using System.Threading;
using ZDevTools.ServiceConsole.Properties;
using Newtonsoft.Json;
using ZDevTools.ServiceCore;

namespace ZDevTools.ServiceConsole
{
    [Description("代表一个计划的服务"), DefaultEvent("DoWork"), DefaultProperty("ServiceName")]
    public partial class ScheduledServiceUI : UserControl, IBindedServiceUI, IConfigurableUI, IControllableUI
    {
        static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(ScheduledServiceUI));
        void logInfo(string message) => log.Info($"【{ServiceName}】{message}");
        void logError(string message) => log.Error($"【{ServiceName}】{message}");

        /// <summary>
        /// 服务名称
        /// </summary>
        public string ServiceName { get { return lServiceName.Text; } }

        /// <summary>
        /// 服务状态
        /// </summary>
        public ScheduledServiceStatus ServiceStatus { get; private set; }

        bool isServiceEnabled { get { return ServiceStatus != ScheduledServiceStatus.Stopped; } }

        /// <summary>
        /// 获取执行当前服务需要用到的资源
        /// </summary>
        AutoResetEvent[] relyOnResources;

        /// <summary>
        /// 等待资源可用的最大超时时间【为0时使用默认值，单位：秒】
        /// </summary>
        int waitTimeOut;

        IScheduledService bindedService;
        bool canbeCancelled;
        /// <summary>
        /// 绑定的服务
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public IServiceBase BindedService
        {
            get { return bindedService; }
            set
            {
                if (bindedService != null)
                    throw new InvalidOperationException("不支持为该控件多次绑定服务！");

                bindedService = (IScheduledService)value;
                lServiceName.Text = value.ServiceName;
                canbeCancelled = value is IServiceRevokable;

                //处理资源占用问题
                var attributes = value.GetType().GetCustomAttributes(false);
                var resources = new List<AutoResetEvent>();
                foreach (var attribute in attributes)
                {
                    var requestResource = attribute as RequestResourceAttribute;
                    if (requestResource != null)
                    {
                        AutoResetEvent resetEvent = null;

                        if (!allResources.TryGetValue(requestResource.ResourceName, out resetEvent))
                        {
                            resetEvent = new AutoResetEvent(true);
                            allResources.Add(requestResource.ResourceName, resetEvent);
                        }

                        resources.Add(resetEvent);
                    }
                    else
                    {
                        var waitResourceTimeout = attribute as WaitResourceTimeoutAttribute;
                        if (waitResourceTimeout != null)
                        {
                            waitTimeOut = waitResourceTimeout.WaitTimeout;
                        }
                    }
                }

                relyOnResources = resources.ToArray();
            }
        }

        public bool IsStopped => ServiceStatus == ScheduledServiceStatus.Stopped;


        /// <summary>
        /// 代表可以收集到的所有资源
        /// </summary>
        static Dictionary<string, AutoResetEvent> allResources = new Dictionary<string, AutoResetEvent>();

        public ScheduledServiceUI()
        {
            InitializeComponent();

            mtbIntervalTime.ValidatingType = typeof(DateTime);
            mtbOntime.GotFocus += mtbOntime_GotFocus;
            mtbIntervalTime.GotFocus += mtbIntervalTime_GotFocus;
        }

        async void doWork()
        {
            //当前正在执行任务，无法执行本轮任务
            if (ServiceStatus == ScheduledServiceStatus.Running)
            {
                logError("重复执行，已阻止，请检查任务执行的间隔时间并适当调大");
                return;
            }

            updateServiceStatus(ScheduledServiceStatus.Running);

            logInfo($"开始执行");
            DateTime startTime = DateTime.Now;

            await Task.Run(() =>
            {
                //检查是否存在资源竞争问题
                if (relyOnResources.Length > 0 && !AutoResetEvent.WaitAll(relyOnResources, (waitTimeOut == 0 ? Settings.Default.ServiceWaitTimeOut : waitTimeOut) * 1000))
                {
                    const string errorMessage = "等待资源释放超时，未执行";
                    logError(errorMessage);
                    ((ServiceBase)bindedService).ReportError(errorMessage);
                    return;
                }
                try
                {
                    bindedService.Run();
                }
                finally
                {
                    //释放所有锁
                    foreach (var autoResetEvent in relyOnResources)
                        autoResetEvent.Set();
                }
            });

            logInfo($"结束执行，总计耗时：{DateTime.Now - startTime}");

            updateServiceStatus(ScheduledServiceStatus.Waitting);
        }

        /// <summary>
        /// 获取当前服务的执行状态名称
        /// </summary>
        void updateServiceStatus(ScheduledServiceStatus serviceStatus)
        {
            //如果要更新为等待状态且没有开启自动执行功能，则认为服务已停止，否则更新为指定状态

            if (serviceStatus == ScheduledServiceStatus.Waitting) //更新为等待状态时，需要进行特殊处理
            {
                if (ServiceStatus == ScheduledServiceStatus.Stopping) //由停止中的状态而来，说明用户强行终止任务
                    this.ServiceStatus = ScheduledServiceStatus.Stopped;
                else if (cbAutoRun.Checked) //勾选了自动运行，等待下一轮任务执行
                    this.ServiceStatus = ScheduledServiceStatus.Waitting;
                else //没有勾选自动运行，任务终止
                    this.ServiceStatus = ScheduledServiceStatus.Stopped;
            }
            else
                this.ServiceStatus = serviceStatus;

            //更新控件状态
            string statusName;
            string buttonText;
            bool buttonEnabled;
            switch (ServiceStatus)
            {
                case ScheduledServiceStatus.Stopped:
                    statusName = "已停止";
                    buttonText = "启用";
                    buttonEnabled = true;
                    break;
                case ScheduledServiceStatus.Waitting:
                    statusName = "等待运行";
                    buttonText = "停用";
                    buttonEnabled = true;
                    break;
                case ScheduledServiceStatus.Running:
                    statusName = "正在运行";
                    buttonText = "停用";
                    buttonEnabled = canbeCancelled;
                    break;
                case ScheduledServiceStatus.Stopping:
                    statusName = "正在停用";
                    buttonText = "停用";
                    buttonEnabled = false;
                    break;
                default:
                    statusName = "未知状态";
                    buttonText = "未知";
                    buttonEnabled = false;
                    break;
            }

            lStatus.Text = statusName;
            bOperation.Text = buttonText;
            bOperation.Enabled = buttonEnabled;

            logInfo(statusName);
        }

        void updateInterval()
        {
            if (rbRecycle.Checked)
            {
                var dateValue = mtbIntervalTime.ValidateText();
                if (dateValue != null)
                {
                    var timeSpan = ((DateTime)(dateValue)).TimeOfDay;
                    if (timeSpan.TotalSeconds >= 1) //防止用户将小于1秒钟的参数值传入程序
                        tJob.Interval = (int)timeSpan.TotalMilliseconds;
                }
            }
            else if (rbOnTime.Checked)
            {
                var targetTime = (DateTime)mtbOntime.ValidateText();
                var nowTime = DateTime.Now;
                if (targetTime > nowTime)
                    tJob.Interval = (int)(targetTime.AddSeconds(10) - nowTime).TotalMilliseconds;
                else
                    tJob.Interval = (int)(targetTime.AddDays(1).AddSeconds(10) - nowTime).TotalMilliseconds;//这里多加了十秒钟是为了防止：计时器不准确，提前执行后当前时间小于目标时间，将再次执行本任务的问题。
            }
        }

        private void tJob_Tick(object sender, EventArgs e)
        {
            //更新下一轮执行间隔
            updateInterval();

            //开始执行任务
            doWork();
        }

        private void bOperation_Click(object sender, EventArgs e)
        {
            switch (ServiceStatus)
            {
                case ScheduledServiceStatus.Stopped: //停止状态下，开启任务

                    if (cbAutoRun.Checked)
                    {
                        updateInterval();
                        tJob.Start();
                    }
                    if (cbImmediately.Checked) //如果勾选了立即执行则直接开始任务，否则更新服务状态为等待状态
                        doWork();
                    else
                        updateServiceStatus(ScheduledServiceStatus.Waitting);

                    break;
                case ScheduledServiceStatus.Waitting: //等待状态下禁用任务

                    tJob.Stop();
                    updateServiceStatus(ScheduledServiceStatus.Stopped);

                    break;
                case ScheduledServiceStatus.Running: //运行状态下，停止计时器防止再次启动任务，将任务状态标记为停止中，再取消任务

                    tJob.Stop();
                    updateServiceStatus(ScheduledServiceStatus.Stopping);
                    (bindedService as IServiceRevokable).Cancel();

                    break;
                case ScheduledServiceStatus.Stopping: //这个状态下无操作
                    break;
                default:
                    break;
            }
        }

        #region 修正MaskTextBox的Modified事件不可用问题

        string oldValue;
        private void mtbOntime_GotFocus(object sender, EventArgs e)
        {
            oldValue = mtbOntime.Text;
        }

        private void mtbOntime_Validating(object sender, CancelEventArgs e)
        {
            e.Cancel = mtbOntime.ValidateText() == null;
            if (!e.Cancel && mtbOntime.Text != oldValue)
                mtbOntime.Modified = true;
        }

        private void mtbOntime_ModifiedChanged(object sender, EventArgs e)
        {
            //当勾选按时间点执行时，更新间隔时间
            if (rbOnTime.Checked)
                updateInterval();
        }

        private void mtbIntervalTime_GotFocus(object sender, EventArgs e)
        {
            oldValue = mtbIntervalTime.Text;
        }

        private void mtbIntervalTime_Validating(object sender, CancelEventArgs e)
        {
            e.Cancel = mtbIntervalTime.ValidateText() == null;
            if (!e.Cancel && mtbIntervalTime.Text != oldValue)
                mtbIntervalTime.Modified = true;
        }

        private void mtbIntervalTime_ModifiedChanged(object sender, EventArgs e)
        {
            //当勾选按时间点执行时，更新间隔时间
            if (rbRecycle.Checked)
                updateInterval();
        }
        #endregion

        private void rbRecycle_CheckedChanged(object sender, EventArgs e)
        {
            //当自动执行方式发生改变时更新自动执行间隔时间
            updateInterval();
        }

        private void cbAutoRun_CheckedChanged(object sender, EventArgs e)
        {
            //当服务被启用并且勾选自动执行时将计时器打开，否则关闭之
            if (isServiceEnabled && cbAutoRun.Checked)
            {
                updateInterval();
                tJob.Start();
            }
            else
                tJob.Stop();
        }

        public void LoadConfig(string jsonString)
        {
            var config = JsonConvert.DeserializeObject<ScheduledServiceConfig>(jsonString);

            cbAutoRun.Checked = config.IsAutoRun;
            if (config.IsRecycle)
                rbRecycle.Checked = true;
            else
                rbOnTime.Checked = true;
            cbImmediately.Checked = config.Immediately;
            if (!string.IsNullOrEmpty(config.OnTimeTime))
                mtbOntime.Text = config.OnTimeTime;
            if (!string.IsNullOrEmpty(config.RecycleInterval))
                mtbIntervalTime.Text = config.RecycleInterval;
        }

        public string SaveConfig()
        {
            return JsonConvert.SerializeObject(new ScheduledServiceConfig() { IsAutoRun = cbAutoRun.Checked, IsRecycle = rbRecycle.Checked, OnTimeTime = mtbOntime.Text, RecycleInterval = mtbIntervalTime.Text, Immediately = cbImmediately.Checked });
        }

        public void Stop()
        {
            if (isServiceEnabled)
                bOperation.PerformClick();
        }
    }

    public enum ScheduledServiceStatus
    {
        Stopped, Waitting, Running, Stopping
    }

    public class ScheduledServiceConfig
    {
        public bool IsAutoRun { get; set; }

        public bool IsRecycle { get; set; }

        public string RecycleInterval { get; set; }

        public string OnTimeTime { get; set; }

        public bool Immediately { get; set; }
    }


}
