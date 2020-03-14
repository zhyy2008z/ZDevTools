using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZDevTools.Utilities
{
    public readonly struct PeakInfo
    {

        public PeakInfo(int index, int width, double value)
        {
            this.Index = index;
            this.Width = width;
            this.Value = value;
        }

        public int Index { get; }

        public int Width { get; }

        public double Value { get; }
    }
}
