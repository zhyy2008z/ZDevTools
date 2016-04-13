using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Threading.Tasks;

namespace ZDevTools.ServiceConsole.Schedules
{
    public class MonthRepeatSchedule : BasicSchedule
    {
        public override string Title => "每月";

        public int[] Months { get; set; }

        public int[] Days { get; set; }

        public int[] WeekOrders { get; set; }

        public DayOfWeek[] WeekDays { get; set; }


        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("在");
            sb.Append(string.Join("、", Months));
            sb.Append("月的");

            if (Days != null)
            {
                sb.Append(string.Join("、", Days).Replace("32", "最后"));
                sb.Append("日");
            }
            else
            {
                sb.Append("第");
                sb.Append(string.Join("、", WeekOrders).Replace("5", "最后"));
                sb.Append("个");
                sb.Append(string.Join("、", Array.ConvertAll(WeekDays, (i) => CultureInfo.CurrentCulture.DateTimeFormat.GetDayName(i))));
            }

            sb.Append("，");
            sb.Append(base.ToString());

            return sb.ToString();
        }


        bool checkRightDay(DateTime dateTime)
        {
            var month = dateTime.Month;


            if (Array.IndexOf(Months, month) < 0) //检查月份
                return false;

            var day = dateTime.Day;

            if (Days != null) //按日期模式
            {
                if (Array.IndexOf(Days, day) < 0 && //今天不直接出现在列表中
                    !(Array.IndexOf(Days, 32) > -1 && //选择了最后一天
                      DateTime.DaysInMonth(dateTime.Year, month) == day //并且今天是本月的最后一天
                     )
                   )
                    return false;
                else
                    return true;
            }
            else //按星期模式
            {
                //本月有几天
                var days = DateTime.DaysInMonth(dateTime.Year, month);
                //是否是最后一个星期
                bool lastOrder = day > days - 7;
                //星期几
                DayOfWeek dayOfWeek = dateTime.DayOfWeek;
                //判断第几个
                int order = (day - 1) / 7 + 1;

                if (Array.IndexOf(WeekDays, dayOfWeek) > -1)
                    return lastOrder && Array.IndexOf(WeekOrders, 5) > -1 || Array.IndexOf(WeekOrders, order) > -1;
                else
                    return false;
            }
        }


        protected override void CalculateTime(DateTime now, out DateTime thisTime, out DateTime nextTime)
        {
            thisTime = BeginTime;
            nextTime = DateTime.MaxValue;

            if (now < thisTime)
            {
                nextTime = thisTime;
                while (!checkRightDay(nextTime))
                    nextTime = nextTime.AddDays(1);
            }
            else
            {
                thisTime = now.Date + BeginTime.TimeOfDay;
                nextTime = thisTime;

                if (thisTime > now)
                    thisTime = thisTime.AddDays(-1);
                else
                    nextTime = nextTime.AddDays(1);

                while (!checkRightDay(thisTime))
                    thisTime = thisTime.AddDays(-1);

                while (!checkRightDay(nextTime))
                    nextTime = nextTime.AddDays(1);
            }
        }

    }
}
