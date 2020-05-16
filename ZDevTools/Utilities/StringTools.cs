using System;
using System.Linq;
using System.Text;

namespace ZDevTools.Utilities
{
    /// <summary>
    /// 字符串工具类
    /// </summary>
    public static class StringTools
    {
        /// <summary>
        /// 转换十六进制字符串为byte数组
        /// </summary>
        public static byte[] HexStringToBytes(string hexString)
        {
            byte[] bytes = new byte[hexString.Length / 2];
            for (int i = 0; i < bytes.Length; i++)
                bytes[i] = byte.Parse(hexString.Substring(i * 2, 2), System.Globalization.NumberStyles.HexNumber);
            return bytes;
        }

        /// <summary>
        /// 转换Byte数组为十六进制字符串
        /// </summary>
        public static string HexStringFromBytes(byte[] bytes)
        {
            return string.Concat(bytes.Select(byt => byt.ToString("x2")));
            //return string.Concat(Array.ConvertAll(bytes, b => b.ToString("x2")));
        }

#if NETCOREAPP
        /// <summary>
        /// 转换Span为十六进制字符串
        /// </summary>
        public static string HexStringFromSpan(ReadOnlySpan<byte> span)
        {
            StringBuilder sb = new StringBuilder();
            foreach (var byt in span)
            {
                sb.Append(byt.ToString("x2"));
            }
            return sb.ToString();
        }
#endif

    }
}
