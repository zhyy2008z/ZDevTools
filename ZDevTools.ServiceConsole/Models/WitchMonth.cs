using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZDevTools.ServiceConsole.Models
{
    public class WitchMonth : IEquatable<WitchMonth>
    {
        public int Month { get; set; }

        public string Text { get; set; }

        public override bool Equals(object obj)
        {
            return Equals(obj as WitchMonth);
        }

        public bool Equals(WitchMonth other)
        {
            return other != null &&
                   Month == other.Month;
        }

        public override int GetHashCode()
        {
            return -1193200019 + Month.GetHashCode();
        }

        public static bool operator ==(WitchMonth month1, WitchMonth month2)
        {
            return EqualityComparer<WitchMonth>.Default.Equals(month1, month2);
        }

        public static bool operator !=(WitchMonth month1, WitchMonth month2)
        {
            return !(month1 == month2);
        }
    }
}
