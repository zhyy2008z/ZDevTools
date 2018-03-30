using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prism.Mvvm;
using System.ComponentModel;
using Newtonsoft.Json;
using System.Windows.Media;
using Microsoft.Practices.Unity;
using Prism.Commands;


namespace ZDevTools.ServiceConsole.ViewModels
{
    using ServiceCore;
    using Properties;
    using Schedules;
    using DIServices;

    public class ScheduledServiceUIViewModel : ServiceViewModelBase, IConfigurableUI
    {
        static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(ScheduledServiceUIViewModel));
        void logInfo(string message) => log.Info($"【{DisplayName}】{message}");
        void logError(string message) => log.Error($"【{DisplayName}】{message}");

        static readonly object Locker = new object();

        IDialogs _dialogs;
        public ScheduledServiceUIViewModel(IDialogs dialogs)
        {
            ButtonText = "启用";
            _dialogs = dialogs;
            ManageSchedulesCommand = new DelegateCommand(manageSchedule);
            OperateCommand = new DelegateCommand(operate);
            ImmediatelyChecked = true;
        }

        /// <summary>
        /// 服务状态
        /// </summary>
        public ScheduledServiceStatus ServiceStatus { get; private set; }

        bool isServiceEnabled { get { return ServiceStatus != ScheduledServiceStatus.Stopped; } }

        /// <summary>
        /// 获取执行当前服务需要用到的资源
        /// </summary>
        MyReaderWriterLock[] _relyOnResources;
        RequestAction[] _requestActions;

        int _waitTimeOut;

        IScheduledService _bindedService;
        bool _canbeCancelled;
        /// <summary>
        /// 绑定的服务
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public override IServiceBase BindedService
        {
            get { return _bindedService; }
            set
            {
                if (_bindedService != null)
                    throw new InvalidOperationException("不支持为该控件多次绑定服务！");

                _bindedService = (IScheduledService)value;
                DisplayName = value.DisplayName;
                _canbeCancelled = value is IServiceRevokable;

                //处理资源占用问题
                var attributes = value.GetType().GetCustomAttributes(false);
                var resources = new List<MyReaderWriterLock>();
                var actions = new List<RequestAction>();

                foreach (var attribute in attributes)
                {
                    var requestResource = attribute as RequestResourceAttribute;
                    if (requestResource != null)
                    {

                        MyReaderWriterLock myReaderWriterLock;
                        if (!allResources.TryGetValue(requestResource.ResourceName, out myReaderWriterLock))
                        {
                            myReaderWriterLock = new MyReaderWriterLock();
                            allResources.Add(requestResource.ResourceName, myReaderWriterLock);
                        }
                        resources.Add(myReaderWriterLock);
                        actions.Add(requestResource.RequestAction);
                        if (requestResource.WaitTimeout > _waitTimeOut)
                            _waitTimeOut = requestResource.WaitTimeout;
                    }
                }

                _relyOnResources = resources.ToArray();
                _requestActions = actions.ToArray();
            }
        }

        public override bool IsStopped => ServiceStatus == ScheduledServiceStatus.Stopped;


        /// <summary>
        /// 代表可以收集到的所有资源
        /// </summary>
        static Dictionary<string, MyReaderWriterLock> allResources = new Dictionary<string, MyReaderWriterLock>();

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
                if (_relyOnResources.Length > 0)
                {
                    if (!MyReaderWriterLock.EnterLocks(_relyOnResources, _requestActions, (_waitTimeOut < Settings.Default.ServiceWaitTimeOut ? Settings.Default.ServiceWaitTimeOut : _waitTimeOut) * 1000))
                    {
                        const string errorMessage = "等待资源释放超时，未执行";
                        logError(errorMessage);
                        ((ServiceBase)_bindedService).ReportError(errorMessage);
                        return;
                    }
                }
                try
                {
                    hasError = !_bindedService.Run();
                }
                finally
                {
                    MyReaderWriterLock.LeaveLocks(_relyOnResources, _requestActions);
                }
            });

            logInfo($"结束执行，总计耗时：{DateTime.Now - startTime}");

            updateServiceStatus(ScheduledServiceStatus.Waitting, hasError);
        }

        string _descriptionText;
        public string DescriptionText { get { return _descriptionText; } set { SetProperty(ref _descriptionText, value); } }

        bool _manageScheduleEnabled = true;
        /// <summary>
        /// 获取当前服务的执行状态名称
        /// </summary>
        void updateServiceStatus(ScheduledServiceStatus serviceStatus, bool hasError = false)
        {
            //如果要更新为等待状态且没有开启自动执行功能，则认为服务已停止，否则更新为指定状态

            if (serviceStatus == ScheduledServiceStatus.Waitting) //更新为等待状态时，需要进行特殊处理
            {
                if (ServiceStatus == ScheduledServiceStatus.Stopping || _willFinish) //由停止中的状态而来或本次运行已经是最后一次运行，说明任务将要结束
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
            Brush statusColor;
            string statusToolTip;

            switch (ServiceStatus)
            {
                case ScheduledServiceStatus.Stopped:
                    if (hasError)
                    {
                        statusName = "已停止，有错误";
                        statusColor = Brushes.Red;
                    }
                    else
                    {
                        statusName = "已停止";
                        statusColor = Brushes.Black;
                    }
                    buttonText = "启用";
                    operationEnabled = true;
                    _manageScheduleEnabled = true;
                    statusToolTip = null;
                    break;
                case ScheduledServiceStatus.Waitting:
                    if (hasError)
                    {
                        statusName = "等待运行，有错误";
                        statusColor = Brushes.Red;
                    }
                    else
                    {
                        statusName = "等待运行";
                        statusColor = Brushes.LimeGreen;
                    }
                    buttonText = "停用";
                    operationEnabled = true;
                    _manageScheduleEnabled = false;
                    statusToolTip = "下次执行时间：" + ServiceBase.FormatDateTime((from schedule in _enabledSchedules select schedule.ArrangedTime).Min());
                    break;
                case ScheduledServiceStatus.Running:
                    statusName = "正在运行";
                    statusColor = Brushes.Green;
                    buttonText = "停用";
                    operationEnabled = _canbeCancelled;
                    _manageScheduleEnabled = false;
                    statusToolTip = null;
                    break;
                case ScheduledServiceStatus.Stopping:
                    statusName = "正在停用";
                    statusColor = Brushes.DarkGray;
                    buttonText = "停用";
                    operationEnabled = false;
                    _manageScheduleEnabled = false;
                    statusToolTip = null;
                    break;
                default:
                    statusName = "未知状态";
                    buttonText = "未知";
                    statusColor = Brushes.Yellow;
                    operationEnabled = false;
                    _manageScheduleEnabled = false;
                    statusToolTip = null;
                    break;
            }

            StatusText = statusName;
            StatusColor = statusColor;
            ButtonText = buttonText;
            ButtonEnabled = operationEnabled;
            StatusToolTip = statusToolTip;

            logInfo(statusName);
        }

        bool _immediatelyChecked;
        public bool ImmediatelyChecked { get { return _immediatelyChecked; } set { SetProperty(ref _immediatelyChecked, value); } }

        public DelegateCommand OperateCommand { get; }
        private void operate()
        {
            switch (ServiceStatus)
            {
                case ScheduledServiceStatus.Stopped: //停止状态下，开启任务

                    startSchedules();

                    if (ImmediatelyChecked) //如果勾选了立即执行则直接开始任务，否则更新服务状态为等待状态
                        doWork();
                    else
                        updateServiceStatus(ScheduledServiceStatus.Waitting);

                    break;
                case ScheduledServiceStatus.Waitting: //等待状态下禁用任务

                    stopSchedules();
                    updateServiceStatus(ScheduledServiceStatus.Stopped);

                    break;
                case ScheduledServiceStatus.Running: //运行状态下，停止计时器防止再次启动任务，将任务状态标记为停止中，再取消任务

                    if (_bindedService is IServiceRevokable revokable) //修正不可停止的任务被强行终止造成bug的问题
                    {
                        stopSchedules();
                        updateServiceStatus(ScheduledServiceStatus.Stopping);
                        revokable.Cancel();
                    }

                    break;
                case ScheduledServiceStatus.Stopping: //这个状态下无操作
                    break;
                default:
                    break;
            }
        }


        void startSchedules()
        {
            if (_enabledSchedules.Count > 0)
            {
                _finishedSchedules.Clear();
                _willFinish = false;

                _enabledSchedules.ForEach(bs => bs.Start());
            }
            else
                _willFinish = true;
        }

        void stopSchedules()
        {
            _enabledSchedules.ForEach(bs => bs.Stop());

            _willFinish = true;
        }

        List<BasicSchedule>
            _enabledSchedules = new List<BasicSchedule>(),
            _finishedSchedules = new List<BasicSchedule>(),
            _schedules = new List<BasicSchedule>();
        public void LoadConfig(string jsonString)
        {
            var config = JsonConvert.DeserializeObject<ScheduledServiceConfig>(jsonString);

            ImmediatelyChecked = config.Immediately;

            var scheduleConfigs = config.scheduleConfigs;
            foreach (var item in scheduleConfigs)
            {
                var schedule = (BasicSchedule)JsonConvert.DeserializeObject(item.Item2, Type.GetType(item.Item1));
                _schedules.Add(schedule);
                addEnabledSchedule(schedule);
            }

            updateDescription();
        }

        private void addEnabledSchedule(BasicSchedule schedule)
        {
            if (schedule.Enabled)
            {
                schedule.DoWork -= schedule_DoWork;
                schedule.Finished -= schedule_Finished;
                schedule.DoWork += schedule_DoWork;
                schedule.Finished += schedule_Finished;
                _enabledSchedules.Add(schedule);
            }
        }

        private void updateDescription()
        {
            if (_enabledSchedules.Count == 1)
            {
                DescriptionText = _enabledSchedules[0].ToString();
            }
            else if (_enabledSchedules.Count > 1)
            {
                DescriptionText = "多个计划";
            }
            else
                DescriptionText = "没有计划";
        }

        bool _willFinish;
        private void schedule_Finished(object sender, EventArgs e)
        {
            _finishedSchedules.Add(sender as BasicSchedule);

            if (_finishedSchedules.Count == _enabledSchedules.Count)
                _willFinish = true;
        }

        private void schedule_DoWork(object sender, EventArgs e)
        {
            doWork();
        }

        public string SaveConfig()
        {
            var scheduleConfigs = new List<Tuple<string, string>>();

            foreach (var schedule in _schedules)
            {
                scheduleConfigs.Add(new Tuple<string, string>(schedule.GetType().FullName, JsonConvert.SerializeObject(schedule)));
            }

            return JsonConvert.SerializeObject(new ScheduledServiceConfig() { Immediately = ImmediatelyChecked, scheduleConfigs = scheduleConfigs });
        }

        public DelegateCommand ManageSchedulesCommand { get; }

        private void manageSchedule()
        {
            var tempSchedules = _dialogs.ShowSchedulesDialog(_manageScheduleEnabled, _schedules);

            if (tempSchedules != null)//此处可以这样写，是因为我们并没有直接修改BasicSchedule对象的成员
            {
                _schedules = tempSchedules;
                _enabledSchedules.Clear();
                _schedules.ForEach(bs => addEnabledSchedule(bs));
                updateDescription();
            }
        }


        public override void Stop()
        {
            if (isServiceEnabled)
                operate();
        }

        public override void Start()
        {
            if (!isServiceEnabled)
                operate();
        }

        public override void RefreshStatus() { }

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
