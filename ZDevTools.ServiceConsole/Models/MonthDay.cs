using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZDevTools.ServiceConsole.Models
{
    public class MonthDay : IEquatable<MonthDay>
    {
        public int Day { get; set; }

        public string Text { get; set; }

        public override bool Equals(object obj)
        {
            return Equals(obj as MonthDay);
        }

        public bool Equals(MonthDay other)
        {
            return other != null &&
                   Day == other.Day;
        }

        public override int GetHashCode()
        {
            return 1561898021 + Day.GetHashCode();
        }

        public static bool operator ==(MonthDay day1, MonthDay day2)
        {
            return EqualityComparer<MonthDay>.Default.Equals(day1, day2);
        }

        public static bool operator !=(MonthDay day1, MonthDay day2)
        {
            return !(day1 == day2);
        }
    }
}
