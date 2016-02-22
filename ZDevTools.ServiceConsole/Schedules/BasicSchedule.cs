using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZDevTools.ServiceCore;
using System.Windows.Forms;
using Newtonsoft.Json;

namespace ZDevTools.ServiceConsole.Schedules
{
    /// <summary>
    /// 基本执行计划
    /// </summary>
    public class BasicSchedule : IDisposable
    {
        public event EventHandler DoWork;
        public event EventHandler Finished;


        protected virtual void OnDoWork(EventArgs e)
        {
            if (DoWork != null)
                DoWork(this, e);
        }

        protected virtual void OnFinished(EventArgs e)
        {
            if (Finished != null)
                Finished(this, e);
        }

        [NonSerialized]
        protected readonly Timer ScheduleTimer = new Timer();

        public BasicSchedule()
        {
            ScheduleTimer.Tick += timer_Tick;
        }

        /// <summary>
        /// 安排的执行时间
        /// </summary>
        [NonSerialized]
        protected DateTime ArrangedTime;
        void timer_Tick(object sender, EventArgs e)
        {
            var now = DateTime.Now;
            if (now < ArrangedTime)
                SetTimer(now);
            else
            {
                UpdateInterval();
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
        protected virtual void UpdateInterval()
        {
            ScheduleTimer.Stop();

            var now = DateTime.Now;
            if (now < BeginTime)
            {
                ArrangedTime = BeginTime;
                SetTimer(now);
            }
            else
            {
                var nextTime = DateTime.MaxValue;

                if (RepeatPeriod.HasValue) //如果考虑重复
                {
                    var nextPeriodTime = now + RepeatPeriod.Value;

                    if ((!(RepeatUntil.HasValue && nextPeriodTime >= BeginTime + RepeatUntil.Value)) && nextPeriodTime < nextTime)
                        nextTime = nextPeriodTime;
                }

                if (nextTime == DateTime.MaxValue || EndTime.HasValue && nextTime >= EndTime.Value)
                {
                    OnFinished(EventArgs.Empty); //通知任务已经结束
                }
                else
                {
                    ArrangedTime = nextTime;
                    SetTimer(now);
                }
            }
        }

        protected void SetTimer(DateTime nowTime)
        {
            ScheduleTimer.Interval = (int)Math.Ceiling((ArrangedTime - nowTime).TotalMilliseconds);
            ScheduleTimer.Start();
        }

        public void Start()
        {
            UpdateInterval();
        }

        public void Stop()
        {
            ScheduleTimer.Stop();
        }

        public void Dispose()
        {
            ScheduleTimer.Dispose();
        }

        [JsonIgnore]
        public virtual string Title { get { return "一次"; } }
    }
}
