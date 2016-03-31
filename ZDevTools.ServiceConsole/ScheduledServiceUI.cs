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
using ZDevTools.ServiceConsole.Schedules;

namespace ZDevTools.ServiceConsole
{
    [Description("代表一个计划的服务"), DefaultProperty("ServiceName")]
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
            bool hasError = false;

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
                    hasError = !bindedService.Run();
                }
                finally
                {
                    //释放所有锁
                    foreach (var autoResetEvent in relyOnResources)
                        autoResetEvent.Set();
                }
            });

            logInfo($"结束执行，总计耗时：{DateTime.Now - startTime}");

            updateServiceStatus(ScheduledServiceStatus.Waitting, hasError);
        }


        bool manageScheduleEnabled = true;
        /// <summary>
        /// 获取当前服务的执行状态名称
        /// </summary>
        void updateServiceStatus(ScheduledServiceStatus serviceStatus, bool hasError = false)
        {
            //如果要更新为等待状态且没有开启自动执行功能，则认为服务已停止，否则更新为指定状态

            if (serviceStatus == ScheduledServiceStatus.Waitting) //更新为等待状态时，需要进行特殊处理
            {
                if (ServiceStatus == ScheduledServiceStatus.Stopping || willFinish) //由停止中的状态而来或本次运行已经是最后一次运行，说明任务将要结束
                    this.ServiceStatus = ScheduledServiceStatus.Stopped;
                else
                    this.ServiceStatus = ScheduledServiceStatus.Waitting;
            }
            else
                this.ServiceStatus = serviceStatus;

            //更新控件状态
            string statusName;
            string buttonText;
            bool operationEnabled;
            Color statusColor;
            switch (ServiceStatus)
            {
                case ScheduledServiceStatus.Stopped:
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
                    buttonText = "启用";
                    operationEnabled = true;
                    manageScheduleEnabled = true;
                    break;
                case ScheduledServiceStatus.Waitting:
                    if (hasError)
                    {
                        statusName = "等待运行，有错误";
                        statusColor = Color.Red;
                    }
                    else
                    {
                        statusName = "等待运行";
                        statusColor = Color.Gray;
                    }
                    buttonText = "停用";
                    operationEnabled = true;
                    manageScheduleEnabled = false;
                    break;
                case ScheduledServiceStatus.Running:
                    statusName = "正在运行";
                    statusColor = Color.Green;
                    buttonText = "停用";
                    operationEnabled = canbeCancelled;
                    manageScheduleEnabled = false;
                    break;
                case ScheduledServiceStatus.Stopping:
                    statusName = "正在停用";
                    statusColor = Color.DarkGray;
                    buttonText = "停用";
                    operationEnabled = false;
                    manageScheduleEnabled = false;
                    break;
                default:
                    statusName = "未知状态";
                    buttonText = "未知";
                    statusColor = Color.Yellow;
                    operationEnabled = false;
                    manageScheduleEnabled = false;
                    break;
            }

            lStatus.Text = statusName;
            lStatus.ForeColor = statusColor;
            bOperation.Text = buttonText;
            bOperation.Enabled = operationEnabled;

            logInfo(statusName);
        }


        private void bOperation_Click(object sender, EventArgs e)
        {
            switch (ServiceStatus)
            {
                case ScheduledServiceStatus.Stopped: //停止状态下，开启任务

                    startSchedules();

                    if (cbImmediately.Checked) //如果勾选了立即执行则直接开始任务，否则更新服务状态为等待状态
                        doWork();
                    else
                        updateServiceStatus(ScheduledServiceStatus.Waitting);

                    break;
                case ScheduledServiceStatus.Waitting: //等待状态下禁用任务

                    stopSchedules();
                    updateServiceStatus(ScheduledServiceStatus.Stopped);

                    break;
                case ScheduledServiceStatus.Running: //运行状态下，停止计时器防止再次启动任务，将任务状态标记为停止中，再取消任务

                    stopSchedules();
                    updateServiceStatus(ScheduledServiceStatus.Stopping);
                    (bindedService as IServiceRevokable).Cancel();

                    break;
                case ScheduledServiceStatus.Stopping: //这个状态下无操作
                    break;
                default:
                    break;
            }
        }


        void startSchedules()
        {
            if (enabledSchedules.Count > 0)
            {
                finishedSchedules.Clear();
                willFinish = false;

                enabledSchedules.ForEach(bs => bs.Start());
            }
            else
                willFinish = true;
        }

        void stopSchedules()
        {
            enabledSchedules.ForEach(bs => bs.Stop());

            willFinish = true;
        }

        List<BasicSchedule> enabledSchedules = new List<BasicSchedule>(), finishedSchedules = new List<BasicSchedule>(), schedules = new List<BasicSchedule>();
        public void LoadConfig(string jsonString)
        {
            var config = JsonConvert.DeserializeObject<ScheduledServiceConfig>(jsonString);

            cbImmediately.Checked = config.Immediately;

            var scheduleConfigs = config.scheduleConfigs;
            foreach (var item in scheduleConfigs)
            {
                var schedule = (BasicSchedule)JsonConvert.DeserializeObject(item.Item2, Type.GetType(item.Item1));
                schedules.Add(schedule);
                addEnabledSchedule(schedule);
            }

            updateDescription();
        }

        private void addEnabledSchedule(BasicSchedule schedule)
        {
            schedule.DoWork -= schedule_DoWork;
            schedule.Finished -= schedule_Finished;
            schedule.DoWork += schedule_DoWork;
            schedule.Finished += schedule_Finished;
            if (schedule.Enabled)
                enabledSchedules.Add(schedule);
        }

        private void updateDescription()
        {
            if (enabledSchedules.Count == 1)
            {
                lDescription.Text = enabledSchedules[0].ToString();
            }
            else if (enabledSchedules.Count > 1)
            {
                lDescription.Text = "多个计划";
            }
            else
                lDescription.Text = "没有计划";
        }

        bool willFinish;
        private void schedule_Finished(object sender, EventArgs e)
        {
            finishedSchedules.Add(sender as BasicSchedule);

            if (finishedSchedules.Count == enabledSchedules.Count)
                willFinish = true;
        }

        private void schedule_DoWork(object sender, EventArgs e)
        {
            doWork();
        }

        public string SaveConfig()
        {
            var scheduleConfigs = new List<Tuple<string, string>>();

            foreach (var schedule in schedules)
            {
                scheduleConfigs.Add(new Tuple<string, string>(schedule.GetType().FullName, JsonConvert.SerializeObject(schedule)));
            }

            return JsonConvert.SerializeObject(new ScheduledServiceConfig() { Immediately = cbImmediately.Checked, scheduleConfigs = scheduleConfigs });
        }

        private void bManageSchedule_Click(object sender, EventArgs e)
        {
            using (var form = new ScheduleManageForm())
            {
                form.Schedules = schedules.ToList();
                form.CanManage = manageScheduleEnabled;
                if (form.ShowDialog() == DialogResult.OK)
                {
                    schedules = form.Schedules;
                    enabledSchedules.Clear();
                    schedules.ForEach(bs => addEnabledSchedule(bs));
                    updateDescription();
                }
            }
        }

        private void lServiceName_Click(object sender, EventArgs e)
        {
            Focus();
        }

        public void Stop()
        {
            if (isServiceEnabled)
                bOperation.PerformClick();
        }

        public void Start()
        {
            if (!isServiceEnabled)
                bOperation.PerformClick();
        }
    }

    public enum ScheduledServiceStatus
    {
        Stopped, Waitting, Running, Stopping
    }

    public class ScheduledServiceConfig
    {
        public bool Immediately { get; set; }

        public List<Tuple<string, string>> scheduleConfigs { get; set; }
    }
}
