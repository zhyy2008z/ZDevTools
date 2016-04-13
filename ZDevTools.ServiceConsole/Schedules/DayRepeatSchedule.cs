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
        public int RepeatPerDays { get; set; }


        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("每隔" + RepeatPerDays + "天，");
            sb.Append(base.ToString());

            return sb.ToString();
        }

        public override string Title { get { return "每天"; } }


        protected override void CalculateTime(DateTime now, out DateTime thisTime, out DateTime nextTime)
        {
            thisTime = BeginTime;
            if (now < thisTime)
            {
                nextTime = thisTime;
            }
            else
            {
                var days = (int)(now - BeginTime).TotalDays;
                nextTime = BeginTime.AddDays(RepeatPerDays * (days / RepeatPerDays + 1)); //在无重复的情况下下次的执行时间
                thisTime = nextTime.AddDays(-RepeatPerDays);
            }
        }
    }
}
