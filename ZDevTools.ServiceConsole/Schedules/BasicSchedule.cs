using System;
using System.Text;
using System.Windows.Forms;
using Newtonsoft.Json;
using ZDevTools.ServiceCore;

namespace ZDevTools.ServiceConsole.Schedules
{
    /// <summary>
    /// 基本执行计划
    /// </summary>
    public class BasicSchedule : IDisposable
    {
        /// <summary>
        /// 设定计时器的最大时间
        /// </summary>
        const int MaxPeriod = 24 * 3600 * 1000;

        public event EventHandler DoWork;
        public event EventHandler Finished;


        protected virtual void OnDoWork(EventArgs e)
        {
            DoWork?.Invoke(this, e);
        }

        protected virtual void OnFinished(EventArgs e)
        {
            Finished?.Invoke(this, e);
        }

        [NonSerialized]
        readonly Timer scheduleTimer = new Timer();

        public BasicSchedule()
        {
            scheduleTimer.Tick += scheduleTimer_Tick;
        }

        /// <summary>
        /// 安排的执行时间
        /// </summary>
        [NonSerialized]
        DateTime _arrangedTime;

        public DateTime ArrangedTime { get { return _arrangedTime; } private set { _arrangedTime = value; } }


        void scheduleTimer_Tick(object sender, EventArgs e)
        {
            scheduleTimer.Stop();

            var now = DateTime.Now;
            if (now < ArrangedTime)
                setTimer(now);
            else
            {
                updateInterval();
                OnDoWork(EventArgs.Empty);
            }
        }

        /// <summary>
        /// 是否启用
        /// </summary>
        public bool Enabled { get; set; }

        /// <summary>
        /// 计划开始于
        /// </summary>
        public DateTime BeginTime { get; set; }

        /// <summary>
        /// 计划终结时间，为null时没有终结时间
        /// </summary>
        public DateTime? EndTime { get; set; }

        /// <summary>
        /// 重复周期
        /// </summary>
        public TimeSpan? RepeatPeriod { get; set; }

        /// <summary>
        /// 重复持续时间，如果为null，则永远持续
        /// </summary>
        public TimeSpan? RepeatUntil { get; set; }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("在" + ServiceBase.FormatDateTime(BeginTime) + "开始执行");

            if (RepeatPeriod != null)
                sb.Append("，每隔" + RepeatPeriod.Value.ToString("h'小时'm'分's'秒'") + "执行一次");

            if (RepeatUntil != null)
                sb.Append("，持续" + RepeatUntil.Value.ToString("h'小时'm'分钟's'秒后'") + "不再重复执行");

            if (EndTime != null)
                sb.Append(",计划于" + ServiceBase.FormatDateTime(EndTime.Value) + "到期");

            return sb.ToString();
        }

        /// <summary>
        /// 更新计时器间隔时间，重写时注意设置<see cref="ArrangedTime"/>
        /// </summary>
        void updateInterval()
        {
            var now = DateTime.Now;

            DateTime thisTime, nextTime;

            CalculateTime(now, out thisTime, out nextTime);

            if (now >= BeginTime && RepeatPeriod.HasValue) //当前时间比起始时间晚才考虑重复
            {
                var nextPeriodTime = now + RepeatPeriod.Value;
                if ((!(RepeatUntil.HasValue && nextPeriodTime >= thisTime + RepeatUntil.Value)) && nextPeriodTime < nextTime)
                    nextTime = nextPeriodTime;
            }

            if (EndTime.HasValue && nextTime >= EndTime.Value)
            {
                nextTime = DateTime.MaxValue;
            }

            ArrangedTime = nextTime;

            if (ArrangedTime == DateTime.MaxValue)
                OnFinished(EventArgs.Empty); //通知任务已经结束
            else
                setTimer(now);
        }

        protected virtual void CalculateTime(DateTime now, out DateTime thisTime, out DateTime nextTime)
        {
            thisTime = BeginTime;
            if (now < thisTime)
                nextTime = thisTime;
            else
                nextTime = DateTime.MaxValue;
        }

        void setTimer(DateTime nowTime)
        {
            var elapsedMiliseconds = Math.Ceiling((ArrangedTime - nowTime).TotalMilliseconds);
            scheduleTimer.Interval = elapsedMiliseconds > MaxPeriod ? MaxPeriod : (int)elapsedMiliseconds;
            scheduleTimer.Start();
        }

        public void Start()
        {
            updateInterval();
        }

        public void Stop()
        {
            scheduleTimer.Stop();
        }

        public void Dispose()
        {
            scheduleTimer.Dispose();
        }

        [JsonIgnore]
        public virtual string Title { get { return "一次"; } }
    }
}
