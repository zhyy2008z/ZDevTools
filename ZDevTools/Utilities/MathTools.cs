using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZDevTools.Utilities
{
    /// <summary>
    /// 数学工具
    /// </summary>
    public static class MathTools
    {
        /// <summary>
        /// 寻峰函数（一阶微分法）
        /// </summary>
        /// <param name="values">用来寻峰的数据</param>
        /// <param name="valueLimit">峰值限制（大于）</param>
        /// <param name="widthLimit">峰宽限制（大于等于）</param>
        /// <param name="mergeDistance">合并峰距（小于），当两峰相距较近时，允许相对较小的峰合并到大峰</param>
        /// <returns></returns>
        public static List<PeakInfo> FindPeaks(ReadOnlySpan<double> values, double valueLimit, int widthLimit, int mergeDistance)
        {
            List<PeakInfo> result = new List<PeakInfo>();
            int width = 0;
            bool? lastSign = default;
            int peakIndex = -1;
            for (int i = 1; i < values.Length - 1; i++)
            {
                bool sign = values[i] - values[i - 1] >= 0;
                if (lastSign.HasValue)
                {
                    width++;
                    //连续
                    if (sign == lastSign)
                    {

                    }
                    // 顶峰位置
                    else if (!sign && lastSign.Value)
                    {
                        if (values[i - 1] > valueLimit)
                        {
                            peakIndex = i - 1;
                        }
                    }
                    //峰开始/结束位置
                    else
                    {
                        if (peakIndex > -1)
                        {
                            result.Add(new PeakInfo(peakIndex, width, values[peakIndex]));
                        }

                        width = 0;
                        peakIndex = -1;
                    }
                }
                lastSign = sign;
            }

            if (peakIndex > -1)
            {
                width++;
                result.Add(new PeakInfo(peakIndex, width, values[peakIndex]));
            }

            //合并小峰
            for (int i = 1; i < result.Count; i++)
            {
                var last = result[i - 1];
                var current = result[i];
                var interval = current.Index - last.Index;
                if (interval < mergeDistance)
                {
                    if (current.Value > last.Value)
                    {
                        result[i - 1] = new PeakInfo(current.Index, current.Width + last.Width, current.Value);
                    }
                    else
                    {
                        result[i - 1] = new PeakInfo(last.Index, current.Width + last.Width, last.Value);
                    }
                    result.RemoveAt(i);
                    i--;
                }
            }

            //移除不符合峰宽限制的结果
            result.RemoveAll(pi => pi.Width < widthLimit);

            return result;
        }

        /// <summary>
        /// 寻峰函数（一阶微分法）
        /// </summary>
        /// <param name="values">用来寻峰的数据</param>
        /// <param name="valueLimit">峰值限制（大于）</param>
        /// <param name="widthLimit">峰宽限制（大于等于）</param>
        /// <param name="mergeDistance">合并峰距（小于），当两峰相距较近时，允许相对较小的峰合并到大峰</param>
        /// <returns></returns>
        public static List<PeakInfo<T>> FindPeaks<T>(ReadOnlySpan<T> values, T valueLimit, int widthLimit, int mergeDistance) where T : IComparable<T>
        {
            List<PeakInfo<T>> result = new List<PeakInfo<T>>();
            int width = 0;
            bool? lastSign = default;
            int peakIndex = -1;
            for (int i = 1; i < values.Length - 1; i++)
            {
                bool sign = values[i].CompareTo(values[i - 1]) >= 0;
                if (lastSign.HasValue)
                {
                    width++;
                    //连续
                    if (sign == lastSign)
                    {

                    }
                    // 顶峰位置
                    else if (!sign && lastSign.Value)
                    {
                        if (values[i - 1].CompareTo(valueLimit) > 0)
                        {
                            peakIndex = i - 1;
                        }
                    }
                    //峰开始/结束位置
                    else
                    {
                        if (peakIndex > -1)
                        {
                            result.Add(new PeakInfo<T>(peakIndex, width, values[peakIndex]));
                        }

                        width = 0;
                        peakIndex = -1;
                    }
                }
                lastSign = sign;
            }

            if (peakIndex > -1)
            {
                width++;
                result.Add(new PeakInfo<T>(peakIndex, width, values[peakIndex]));
            }

            //合并小峰
            for (int i = 1; i < result.Count; i++)
            {
                var last = result[i - 1];
                var current = result[i];
                var interval = current.Index - last.Index;
                if (interval < mergeDistance)
                {
                    if (current.Value.CompareTo(last.Value) > 0)
                    {
                        result[i - 1] = new PeakInfo<T>(current.Index, current.Width + last.Width, current.Value);
                    }
                    else
                    {
                        result[i - 1] = new PeakInfo<T>(last.Index, current.Width + last.Width, last.Value);
                    }
                    result.RemoveAt(i);
                    i--;
                }
            }

            //移除不符合峰宽限制的结果
            result.RemoveAll(pi => pi.Width < widthLimit);

            return result;
        }

        /// <summary>
        /// 寻峰函数（使用窗口内峰值法，不支持计算峰宽）
        /// </summary>
        /// <param name="values">用来寻峰的数据</param>
        /// <param name="peakValueLimit">峰值限制（大于）</param>
        /// <param name="windowSize">寻峰窗口大小（大于等于）</param>
        public static List<PeakInfo> FindPeaksByWindow(ReadOnlySpan<double> values, double peakValueLimit, int windowSize)
        {
            List<PeakInfo> result = new List<PeakInfo>();
            var end = values.Length - windowSize;
            int middleIndex = windowSize / 2;
            for (int i = 0; i < end; i++)
            {
                //找出本次窗口中的最大值与在窗口中的峰位
                double max = peakValueLimit;
                int peakIndex = -1;

                for (int j = 0; j < windowSize; j++)
                {
                    var value = values[i + j];
                    if (max < value)
                    {
                        peakIndex = j;
                        max = value;
                    }
                }

                //如果峰位在窗口正中间且峰值大于限制值，那么判定为一个峰
                if (peakIndex == middleIndex && max > peakValueLimit)
                {
                    result.Add(new PeakInfo(i + middleIndex, 0, values[i + middleIndex]));
                }
            }
            return result;
        }

        /// <summary>
        /// 寻峰函数（使用窗口内峰值法，不支持计算峰宽）
        /// </summary>
        /// <param name="values">用来寻峰的数据</param>
        /// <param name="peakValueLimit">峰值限制</param>
        /// <param name="windowSize">寻峰窗口大小（大于等于）</param>
        public static List<PeakInfo<T>> FindPeaksByWindow<T>(ReadOnlySpan<T> values, T peakValueLimit, int windowSize)
            where T : IComparable<T>
        {
            List<PeakInfo<T>> result = new List<PeakInfo<T>>();
            var end = values.Length - windowSize;
            int middleIndex = windowSize / 2;
            for (int i = 0; i < end; i++)
            {
                //找出本次窗口中的最大值与在窗口中的峰位
                T max = peakValueLimit;
                int peakIndex = -1;

                for (int j = 0; j < windowSize; j++)
                {
                    var value = values[i + j];
                    if (max.CompareTo(value) < 0)
                    {
                        peakIndex = j;
                        max = value;
                    }
                }

                //如果峰位在窗口正中间且峰值大于限制值，那么判定为一个峰
                if (peakIndex == middleIndex && max.CompareTo(peakValueLimit) > 0)
                {
                    result.Add(new PeakInfo<T>(i + middleIndex, 0, values[i + middleIndex]));
                }
            }
            return result;
        }

        /// <summary>
        /// 寻峰函数（使用砍低值法，不支持计算峰宽）
        /// </summary>
        /// <param name="values">用来寻峰的数据</param>
        /// <param name="peakValueLimit">峰值限制（大于）</param>
        /// <param name="peakWidthLimit">峰宽限制（大于等于）</param>
        /// <param name="lowPass">低于此值的视为本底信号（大于）</param>
        /// <remarks>基础性能最好的算法</remarks>
        public static List<PeakInfo> FindPeaksByCutLowValue(ReadOnlySpan<double> values, double peakValueLimit, int peakWidthLimit, double lowPass)
        {
            List<PeakInfo> result = new List<PeakInfo>();

            bool lastHasValue = false;
            int startIndex = default;
            double max = peakValueLimit;
            int maxIndex = default;

            for (int i = 0; i < values.Length; i++)
            {
                bool hasValue = values[i] > lowPass;

                if (lastHasValue)
                {
                    if (hasValue) //上一次有值，本次也有值，连续，求最值
                    {
                        if (max < values[i])
                        {
                            max = values[i];
                            maxIndex = i;
                        }
                    }
                    else //上一次有值，本次没有值，发现一个可能的峰
                    {
                        int width = i - startIndex;

                        //寻到一个峰值
                        if (width >= peakWidthLimit && max > peakValueLimit)
                        {
                            result.Add(new PeakInfo(maxIndex, 0, max));
                        }
                    }
                }
                else
                {
                    if (hasValue) //上一次无值，本次有值，开始一个峰
                    {
                        startIndex = i;

                        max = values[i];
                        maxIndex = i;
                    }
                    else  //上一次无值，本次无值，不管它
                    {

                    }
                }

                lastHasValue = hasValue;
            }

            return result;
        }

        /// <summary>
        /// 寻峰函数（使用砍低值法，不支持计算峰宽）
        /// </summary>
        /// <param name="values">用来寻峰的数据</param>
        /// <param name="peakValueLimit">峰值限制（大于）</param>
        /// <param name="peakWidthLimit">峰宽限制（大于等于）</param>
        /// <param name="lowPass">低于此值的视为本底信号（大于）</param>
        /// <remarks>基础性能最好的算法</remarks>
        public static List<PeakInfo<T>> FindPeaksByCutLowValue<T>(ReadOnlySpan<T> values, T peakValueLimit, int peakWidthLimit, T lowPass)
            where T : IComparable<T>
        {
            List<PeakInfo<T>> result = new List<PeakInfo<T>>();

            bool lastHasValue = false;
            int startIndex = default;
            T max = peakValueLimit;
            int maxIndex = default;

            for (int i = 0; i < values.Length; i++)
            {
                bool hasValue = values[i].CompareTo(lowPass) > 0;

                if (lastHasValue)
                {
                    if (hasValue) //上一次有值，本次也有值，连续，求最值
                    {
                        if (max.CompareTo(values[i]) < 0)
                        {
                            max = values[i];
                            maxIndex = i;
                        }
                    }
                    else //上一次有值，本次没有值，发现一个可能的峰
                    {
                        int width = i - startIndex;

                        //寻到一个峰值
                        if (width >= peakWidthLimit && max.CompareTo(peakValueLimit) > 0)
                        {
                            result.Add(new PeakInfo<T>(maxIndex, 0, max));
                        }
                    }
                }
                else
                {
                    if (hasValue) //上一次无值，本次有值，开始一个峰
                    {
                        startIndex = i;

                        max = values[i];
                        maxIndex = i;
                    }
                    else  //上一次无值，本次无值，不管它
                    {

                    }
                }

                lastHasValue = hasValue;
            }

            return result;
        }

        /// <summary>
        /// 应用移动窗口滤波（对一维数组整体运用窗口滤波算法，效率较高）
        /// </summary>
        /// <param name="window">窗口参数</param>
        /// <param name="values">数据</param>
        /// <returns>返回已应用滤波的数组（新实例）</returns>
        public static double[] ApplyWindowFilter(ReadOnlySpan<double> values, double[] window)
        {
            var r = window.Length / 2;
            if (values.Length - r < 0)
                throw new ArgumentException("数据点数过少", nameof(values));

            double[] result = new double[values.Length];
            var windowSum = window.Sum();
            //从r位置开始到length-r位置，应用以j为中心的高斯滤波
            for (int i = r; i < values.Length - r; i++)
            {
                double sum = 0;
                for (int j = 0; j < window.Length; j++)
                    sum += window[j] * values[i + j - r];
                result[i] = sum / windowSum;
            }

            for (int i = 0; i < r; i++)
            {
                //每个点都需要计算自己的高斯权值和
                windowSum = 0;
                double sum = 0;
                for (int j = 0; j < window.Length; j++)
                {
                    var valueIndex = i - r + j;

                    if (valueIndex > -1 && valueIndex < values.Length)
                    {
                        windowSum += window[j];
                        sum += window[j] * values[valueIndex];
                    }
                }
                result[i] = sum / windowSum;
            }

            for (int i = values.Length - r; i < values.Length; i++)
            {
                //每个点都需要计算自己的高斯权值和
                windowSum = 0;
                double sum = 0;
                for (int j = 0; j < window.Length; j++)
                {
                    var valueIndex = i - r + j;

                    if (valueIndex > -1 && valueIndex < values.Length)
                    {
                        windowSum += window[j];
                        sum += window[j] * values[valueIndex];
                    }
                }
                result[i] = sum / windowSum;
            }
            return result;
        }

        /// <summary>
        /// 对一维数组的部分数据直接进行窗口滤波算法（直接应用到传入的数组上）
        /// </summary>
        /// <param name="values">数据</param>
        /// <param name="start">开始应用窗口滤波的起点坐标（包含）</param>
        /// <param name="end">停止应用窗口滤波的终点坐标（不包含）</param>
        /// <param name="window">窗口滤波参数</param>
        public static void ApplyWindowFilter(double[] values, int start, int end, double[] window)
        {
            var arr = ApplyWindowFilter(values.AsSpan()[start..end], window);
            Array.Copy(arr, 0, values, start, arr.Length);
        }

        ///// <summary>
        ///// 应用移动窗口滤波（支持仅对一维数组的部分数据进行窗口滤波算法）
        ///// </summary>
        ///// <param name="window">窗口滤波参数</param>
        ///// <param name="values">数据</param>
        ///// <param name="start">开始应用窗口滤波的起点坐标（包含）</param>
        ///// <param name="end">停止应用窗口滤波的终点坐标（不包含）</param>
        //public static double[] ApplyWindowFilter(double[] window, double[] values, int start, int end)
        //{
        //    double[] result = (double[])values.Clone();
        //    var r = window.Length / 2;

        //    start = Math.Max(start, 0);
        //    end = Math.Min(end, values.Length);

        //    for (int i = start; i < end; i++)
        //    {
        //        //每个点都需要计算自己的高斯权值和
        //        double windowSum = 0;
        //        double sum = 0;
        //        for (int j = 0; j < window.Length; j++)
        //        {
        //            var valueIndex = i - r + j;

        //            if (valueIndex > start - 1 && valueIndex < end)
        //            {
        //                windowSum += window[j];
        //                sum += window[j] * values[valueIndex];
        //            }
        //        }
        //        result[i] = sum / windowSum;
        //    }
        //    return result;
        //}
    }

}
