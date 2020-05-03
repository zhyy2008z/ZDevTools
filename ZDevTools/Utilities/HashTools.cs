using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace ZDevTools.Utilities
{
    /// <summary>
    /// 哈希工具
    /// </summary>
    public static class HashTools
    {
        /// <summary>
        /// 使用UTF8编码计算明文的Sha384编码字符串
        /// </summary>
        /// <param name="clearText"></param>
        /// <returns></returns>
        public static string ComputeSha384(string clearText) => ComputeSha384(Encoding.UTF8.GetBytes(clearText));

        /// <summary>
        /// 计算明文字节数组的Sha384编码字符串
        /// </summary>
        public static string ComputeSha384(byte[] clearBytes)
        {
            using (var sha384 = SHA384.Create())
                return StringTools.HexStringFromBytes(sha384.ComputeHash(clearBytes));
        }

        /// <summary>
        /// 使用UTF8编码计算明文的Md5编码字符串
        /// </summary>
        public static string ComputeMd5(string clearText) => ComputeMd5(Encoding.UTF8.GetBytes(clearText));

        /// <summary>
        /// 计算明文字节数组的Md5编码字符串
        /// </summary>
        public static string ComputeMd5(byte[] clearBytes)
        {
            using (var md5 = MD5.Create())
                return StringTools.HexStringFromBytes(md5.ComputeHash(clearBytes));
        }
    }
}
