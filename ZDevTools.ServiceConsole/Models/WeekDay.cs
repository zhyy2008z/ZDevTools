using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZDevTools.ServiceConsole.Models
{
    public class WeekDay : IEquatable<WeekDay>
    {
        public DayOfWeek DayOfWeek { get; set; }

        public string Text { get; set; }

        public override bool Equals(object obj)
        {
            return Equals(obj as WeekDay);
        }

        public bool Equals(WeekDay other)
        {
            return other != null &&
                   DayOfWeek == other.DayOfWeek;
        }

        public override int GetHashCode()
        {
            return -472147460 + DayOfWeek.GetHashCode();
        }

        public static bool operator ==(WeekDay day1, WeekDay day2)
        {
            return EqualityComparer<WeekDay>.Default.Equals(day1, day2);
        }

        public static bool operator !=(WeekDay day1, WeekDay day2)
        {
            return !(day1 == day2);
        }
    }
}
