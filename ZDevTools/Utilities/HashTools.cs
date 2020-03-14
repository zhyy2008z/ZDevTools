using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace ZDevTools.Utilities
{
    public static class HashTools
    {
        public static string ComputeSha384(string clearText)
        {
            return ComputeSha384(Encoding.UTF8.GetBytes(clearText));
        }

        public static string ComputeSha384(byte[] clearBytes)
        {
            using (var sha384 = SHA384.Create())
                return string.Concat(Array.ConvertAll(sha384.ComputeHash(clearBytes), b => b.ToString("x2")));
        }

        public static string ComputeMd5(string clearText)
        {
            return ComputeMd5(Encoding.UTF8.GetBytes(clearText));
        }

        public static string ComputeMd5(byte[] clearBytes)
        {
            using (var md5 = MD5.Create())
                return string.Concat(Array.ConvertAll(md5.ComputeHash(clearBytes), b => b.ToString("x2")));
        }
    }
}
