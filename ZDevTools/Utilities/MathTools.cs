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
        /// 自编写寻峰函数（by 穿越中的逍遥）
        /// </summary>
        /// <param name="values"></param>
        /// <param name="valueLimit"></param>
        /// <param name="widthLimit"></param>
        /// <returns></returns>
        public static PeakInfo[] FindPeaks(double[] values, double valueLimit, int widthLimit)
        {
            List<PeakInfo> result = new List<PeakInfo>();
            int width = 0;
            bool? lastSign = default;
            int peakIndex = -1;
            for (int i = 1; i < values.Length; i++)
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
                    //峰结束位置
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
            //合并小峰
            for (int i = 1; i < result.Count; i++)
            {
                var last = result[i - 1];
                var current = result[i];
                var interval = current.Index - last.Index;
                if (interval < widthLimit)
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
            return result.Where(pi => pi.Width > widthLimit).ToArray();
        }

        /// <summary>
        /// 应用移动窗口滤波（对一维数组整体运用窗口滤波算法，效率较高）
        /// </summary>
        /// <param name="window">窗口参数</param>
        /// <param name="values">数据</param>
        /// <returns></returns>
        public static double[] ApplyWindowFilter(double[] window, double[] values)
        {
            var r = window.Length / 2;
            if (values.Length - r < 0)
                throw new ArgumentException("数据点数过少", nameof(values));

            double[] result = (double[])values.Clone();
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
        /// 应用移动窗口滤波（支持仅对一维数组的部分数据进行窗口滤波算法）
        /// </summary>
        /// <param name="window">窗口滤波参数</param>
        /// <param name="values">数据</param>
        /// <param name="start">开始应用窗口滤波的起点坐标（包含）</param>
        /// <param name="end">停止应用窗口滤波的终点坐标（不包含）</param>
        public static double[] ApplyWindowFilter(double[] window, double[] values, int start, int end)
        {
            double[] result = (double[])values.Clone();
            var r = window.Length / 2;

            start = Math.Max(start, 0);
            end = Math.Min(end, values.Length);

            for (int i = start; i < end; i++)
            {
                //每个点都需要计算自己的高斯权值和
                double windowSum = 0;
                double sum = 0;
                for (int j = 0; j < window.Length; j++)
                {
                    var valueIndex = i - r + j;

                    if (valueIndex > start - 1 && valueIndex < end)
                    {
                        windowSum += window[j];
                        sum += window[j] * values[valueIndex];
                    }
                }
                result[i] = sum / windowSum;
            }
            return result;
        }
    }

}
