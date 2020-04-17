using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZDevTools.Utilities
{
    /// <summary>
    /// 代表一个峰
    /// </summary>
    public readonly struct PeakInfo
    {
        /// <summary>
        /// 初始化一个寻峰结果
        /// </summary>
        /// <param name="index">峰位</param>
        /// <param name="width">峰宽</param>
        /// <param name="value">峰值</param>
        public PeakInfo(int index, int width, double value)
        {
            this.Index = index;
            this.Width = width;
            this.Value = value;
        }

        /// <summary>
        /// 峰位
        /// </summary>
        public int Index { get; }

        /// <summary>
        /// 峰宽
        /// </summary>
        public int Width { get; }

        /// <summary>
        /// 峰值
        /// </summary>
        public double Value { get; }
    }
}
