using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;
using System.Linq;

namespace ZDevTools.ServiceConsole.Schedules
{
    public class WeekRepeatSchedule : BasicSchedule
    {
        /// <summary>
        /// 每隔几周重复任务
        /// </summary>
        public int RepeatPerWeeks { get; set; }

        /// <summary>
        /// 在每周的周几执行任务
        /// </summary>
        public DayOfWeek[] RepeatWeekDays { get; set; }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("每隔" + RepeatPerWeeks + "周的");
            sb.Append(string.Join("、", RepeatWeekDays.Select(dw => CultureInfo.CurrentCulture.DateTimeFormat.GetDayName(dw))));
            sb.Append("，");
            sb.Append(base.ToString());

            return sb.ToString();
        }

        public override string Title => "每周";


        protected override void CalculateTime(DateTime now, out DateTime thisTime, out DateTime nextTime)
        {
            nextTime = DateTime.MaxValue;
            thisTime = BeginTime;

            var days = now < thisTime ? 0 : (int)(now - thisTime).TotalDays;

            var thisWeekTime = thisTime.AddDays(7 * RepeatPerWeeks * (days / (7 * RepeatPerWeeks))); //本执行周起始时间
            var thisWeekEndTime = thisWeekTime.AddDays(7); //本执行周结束时间


            bool findNextTime = false;
            for (DateTime time = thisWeekTime; time < thisWeekEndTime; time = time.AddDays(1)) //在本执行周查找第一个符合的星期
            {
                if (RepeatWeekDays.Contains(time.DayOfWeek) && now < time)
                {
                    nextTime = time;
                    findNextTime = true;
                    break;
                }
            }
            if (!findNextTime)  //在本执行周没有找到一个符合的星期，那么在下一执行周去找，而且一定能找到的
            {
                nextTime = thisWeekTime.AddDays(7 * RepeatPerWeeks); //下一个执行周起始时间
                while (true)
                {
                    if (RepeatWeekDays.Contains(nextTime.DayOfWeek))
                        break;
                    nextTime = nextTime.AddDays(1);
                }
            }

            if (now >= thisTime)
            {
                bool findThisTime = false;
                for (DateTime time = thisWeekEndTime; time >= thisWeekTime; time = time.AddDays(-1))
                {
                    if (RepeatWeekDays.Contains(time.DayOfWeek) && now >= time)
                    {
                        thisTime = time;
                        findThisTime = true;
                        break;
                    }
                }
                if (!findThisTime)
                {
                    thisTime = thisWeekEndTime.AddDays(-7 * RepeatPerWeeks); //上一个执行周结束时间
                    while (true)
                    {
                        if (RepeatWeekDays.Contains(thisTime.DayOfWeek))
                            break;
                        thisTime = thisTime.AddDays(-1);
                    }
                }
            }
        }
    }
}
