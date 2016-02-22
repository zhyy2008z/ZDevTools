using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZDevTools.ServiceConsole.Schedules
{
    /// <summary>
    /// 按天重复的计划
    /// </summary>
    public class DayRepeatSchedule : BasicSchedule
    {
        public int RepeatPerDays { get; set; } = 1;


        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("每隔" + RepeatPerDays + "天，");
            sb.Append(base.ToString());

            return sb.ToString();
        }

        public override string Title { get { return "每天"; } }

        protected override void UpdateInterval()
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
                var days = (int)(now - BeginTime).TotalDays;

                var nextTime = BeginTime.AddDays(RepeatPerDays * (days / RepeatPerDays + 1)); //在无重复的情况下下次的执行时间

                if (RepeatPeriod.HasValue) //如果考虑重复
                {
                    var thisTime = nextTime.AddDays(-RepeatPerDays);
                    var nextPeriodTime = now + RepeatPeriod.Value;

                    if ((!(RepeatUntil.HasValue && nextPeriodTime >= thisTime + RepeatUntil.Value)) && nextPeriodTime < nextTime)
                        nextTime = nextPeriodTime;
                }

                if (EndTime.HasValue && nextTime >= EndTime.Value)
                    OnFinished(EventArgs.Empty);
                else
                {
                    ArrangedTime = nextTime;
                    SetTimer(now);
                }
            }
        }

    }
}
