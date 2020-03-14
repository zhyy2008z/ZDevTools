using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZDevTools.Utilities
{
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


    }

}
