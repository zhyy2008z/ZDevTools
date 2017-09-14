using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZDevTools.ServiceConsole.Models
{
    public class WitchWeek : IEquatable<WitchWeek>
    {
        public int WeekNumber { get; set; }

        public string Text { get; set; }

        public override bool Equals(object obj)
        {
            return Equals(obj as WitchWeek);
        }

        public bool Equals(WitchWeek other)
        {
            return other != null &&
                   WeekNumber == other.WeekNumber;
        }

        public override int GetHashCode()
        {
            return -1801709224 + WeekNumber.GetHashCode();
        }

        public static bool operator ==(WitchWeek week1, WitchWeek week2)
        {
            return EqualityComparer<WitchWeek>.Default.Equals(week1, week2);
        }

        public static bool operator !=(WitchWeek week1, WitchWeek week2)
        {
            return !(week1 == week2);
        }
    }
}
