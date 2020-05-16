using System;
using System.Collections.Generic;
using System.Text;

namespace ZDevTools.Utilities
{
    /// <summary>
    /// 寻峰结果，泛型版
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public struct PeakInfo<T>
    {
        /// <summary>
        /// 初始化一个寻峰结果
        /// </summary>
        /// <param name="index">峰位</param>
        /// <param name="width">峰宽</param>
        /// <param name="value">峰值</param>
        public PeakInfo(int index, int width, T value)
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
        public T Value { get; }
    }
}
