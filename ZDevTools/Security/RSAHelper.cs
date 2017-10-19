using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Management;
using System.Numerics;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

namespace ZDevTools.Security
{
    /// <summary>
    /// 非对称RSA加密类 参考
    /// http://www.cnblogs.com/hhh/archive/2011/06/03/2070692.html
    /// http://blog.csdn.net/zhilunchen/article/details/2943158
    /// http://www.cnblogs.com/yyl8781697/archive/2013/04/28/RSA.html
    /// 在此基础上修改而来，并非标准的RSA算法，可能与其他实现无法相容，也就是说，加密与解密你都要使用本Helper
    /// 本类使用了<see cref="System.Numerics.BigInteger"/> 结构作为大数计算的工具和表示类型
    /// </summary>
    public static class RSAHelper
    {
        /// <summary>
        /// RSA加密的密匙结构 公钥和私匙
        /// </summary>
        public struct RSAKey
        {
            /// <summary>
            /// 公钥
            /// </summary>
            public string PublicKey;
            /// <summary>
            /// 私钥
            /// </summary>
            public string PrivateKey;
        }


        #region 获取RSA密钥
        /// <summary>
        /// 得到RSA的解谜的密匙对
        /// </summary>
        /// <returns></returns>
        public static RSAKey GenerateRASKey(int keySize = 2048)
        {
            //声明一个指定大小的RSA容器
            RSACryptoServiceProvider rsaProvider = new RSACryptoServiceProvider(keySize);
            //取得RSA容易里的各种参数
            RSAParameters p = rsaProvider.ExportParameters(true);

            return new RSAKey()
            {
                PublicKey = componentKey(p.Exponent, p.Modulus),
                PrivateKey = componentKey(p.D, p.Modulus)
            };
        }


        /// <summary>
        /// 读取公钥或私钥
        /// </summary>
        /// <param name="includePrivateparameters">为True则包含私钥</param>
        /// <param name="path">Xml格式保存的完整公/私钥路径</param>
        /// <returns>公钥或私钥参数形式 </returns>
        public static RSAKey ReadKeyFromXml(bool includePrivateparameters, string path)
        {
            using (StreamReader reader = new StreamReader(path))
            {
                string publickey = reader.ReadToEnd();
                RSACryptoServiceProvider rcp = new RSACryptoServiceProvider();
                rcp.FromXmlString(publickey);
                var rsaParams = rcp.ExportParameters(includePrivateparameters);
                RSAKey rsaKeys;
                if (includePrivateparameters)
                    rsaKeys.PrivateKey = componentKey(rsaParams.D, rsaParams.Modulus);
                else
                    rsaKeys.PrivateKey = null;
                rsaKeys.PublicKey = componentKey(rsaParams.Exponent, rsaParams.Modulus);
                return rsaKeys;
            }
        }
        /// <summary>
        /// 从X.509证书中读取密钥
        /// </summary>
        /// <param name="certificate2">X.509证书</param>
        /// <param name="exportPrivateKey">是否导出私钥</param>
        /// <returns></returns>
        public static RSAKey ReadKeyFromCertificate(X509Certificate2 certificate2, bool exportPrivateKey)
        {
            RSAKey key;
            if (exportPrivateKey)
            {
                var privateParams = ((RSACryptoServiceProvider)certificate2.PrivateKey).ExportParameters(true);
                key.PrivateKey = componentKey(privateParams.D, privateParams.Modulus);
            }
            else
                key.PrivateKey = null;

            var publicParams = ((RSACryptoServiceProvider)certificate2.PublicKey.Key).ExportParameters(false);
            key.PublicKey = componentKey(publicParams.Exponent, publicParams.Modulus);

            return key;
        }
        #endregion

        #region 组合解析密匙
        /// <summary>
        /// 组合成密匙字符串
        /// </summary>
        /// <param name="eOrD">接受正序字节序，自动转为倒序字节序并自动加一位0，能够被<see cref="BigInteger"/>正确识别</param>
        /// <param name="n">接受正序字节序，自动转为倒序字节序并自动加一位0，能够被<see cref="BigInteger"/>正确识别</param>
        /// <returns></returns>
        static string componentKey(byte[] eOrD, byte[] n)
        {
            List<byte> list = new List<byte>();
            //在前端加上第一个数组的长度值 这样今后可以根据这个值分别取出来两个数组
            list.AddRange(BitConverter.GetBytes(eOrD.Length + 1));
            var zeroSequence = Enumerable.Repeat((byte)0, 1);
            list.AddRange(eOrD.Reverse().Concat(zeroSequence));
            list.AddRange(n.Reverse().Concat(zeroSequence));
            byte[] b = list.ToArray();
            return Convert.ToBase64String(b);
        }

        /// <summary>
        /// 解析密匙
        /// </summary>
        /// <param name="key">密匙</param>
        /// <param name="eOrD">RSA的Exponent or D</param>
        /// <param name="n">RSA的Modulus</param>
        static void resolveKey(string key, out byte[] eOrD, out byte[] n)
        {
            //从base64字符串 解析成原来的字节数组
            byte[] b = Convert.FromBase64String(key);
            //初始化参数的数组长度
            var b1Length = BitConverter.ToInt32(b, 0);
            eOrD = new byte[b1Length];
            n = new byte[b.Length - b1Length - 4]; //int32 count 4bytes
            //将相应位置是值放进相应的数组
            Array.Copy(b, 4, eOrD, 0, b1Length);
            Array.Copy(b, 4 + b1Length, n, 0, n.Length);

            //for (int n = 4, i = 0, j = 0; n < b.Length; n++)
            //{
            //    if (n < b1Length + 4)
            //    {
            //        b1[i++] = b[n];
            //    }
            //    else
            //    {
            //        b2[j++] = b[n];
            //    }
            //}
        }
        #endregion

        #region RSA加密解密
        /// <summary>
        /// 使用密钥加密字符串（非标准方法）
        /// </summary>
        /// <param name="clearBytes">源字符串 明文</param>
        /// <param name="key">密钥：公钥 或 私钥</param>
        /// <returns>密钥处理过的字符串</returns>
        /// <remarks>注意，加密字符串解密时，取决于你所使用的密钥，如果用公钥加密字符串，你应该用私钥解密，反之就要用公钥解密</remarks>
        public static List<byte> Encrypt(IList<byte> clearBytes, string key)
        {
            byte[] eOrD;
            byte[] n;
            //解析这个密钥
            resolveKey(key, out eOrD, out n);
            BigInteger biEOrD = new BigInteger(eOrD);
            BigInteger biN = new BigInteger(n);
            return Encrypt(clearBytes, biEOrD, biN);
        }

        /// <summary>
        /// 使用密钥解密字符串（非标准方法，与<see cref="Encrypt(IList{byte}, string)"/>成对使用）
        /// </summary>
        /// <param name="cipherBytes">密文</param>
        /// <param name="key">密钥：公钥 或 私钥</param>
        /// <returns>注意，字符串解密时，取决于你加密所使用的密钥，如果用公钥加密字符串，你应该用私钥解密，反之就要用公钥解密</returns>
        public static List<byte> Decrypt(IList<byte> cipherBytes, string key)
        {
            byte[] eOrD;
            byte[] n;
            //解析这个密钥
            resolveKey(key, out eOrD, out n);
            BigInteger biEOrD = new BigInteger(eOrD);
            BigInteger biN = new BigInteger(n);
            return Decrypt(cipherBytes, biEOrD, biN);
        }

        /// <summary>
        /// 用指定的密匙加密（非标准方法）
        /// </summary>
        /// <param name="clearBytes">明文字节List</param>
        /// <param name="eOrD">可以是RSACryptoServiceProvider生成的Exponent 或 D</param>
        /// <param name="n">可以是RSACryptoServiceProvider生成的Modulus</param>
        /// <returns>返回密文</returns>
        public static List<byte> Encrypt(IList<byte> clearBytes, BigInteger eOrD, BigInteger n)
        {
            int clearLength = clearBytes.Count;
            var nBytes = n.ToByteArray();
            int cipherBytesMaxLength = nBytes.Length;
            int blockSize = nBytes.Length - 1 - 1;
            //每次加密的clear的bytes大小必须比n小，因此，我们这里让blockSize为n size -1 ，然后由于还要防止\0被加密丢，所以每个block还要再补一位，因此，为了保证每个block不超过n的大小还要再-1
            //极端情况  1 255 255 255 //clear bytes 最大的情况
            //     0 128   0   0   0 // n bytes 最小的值（nBytes长度比 clear bytes大2）


            List<byte> result = new List<byte>();

            int remainLength = clearLength;
            int currentBlockSize;

            for (int currentIndex = 0; currentIndex < clearLength; currentIndex += currentBlockSize, remainLength -= currentBlockSize)
            {
                currentBlockSize = remainLength > blockSize ? blockSize : remainLength;
                List<byte> clearList = new List<byte>(clearBytes.Skip(currentIndex).Take(currentBlockSize));
                clearList.Add(1);
                BigInteger clearInteger = new BigInteger(clearList.ToArray());
                BigInteger cipherInteger = BigInteger.ModPow(clearInteger, eOrD, n);
                var cipherBytes = cipherInteger.ToByteArray();
                result.AddRange(cipherBytes);
                if (cipherBytes.Length < cipherBytesMaxLength) //补零，让每个cipher block大小一致
                    result.AddRange(Enumerable.Repeat((byte)0, cipherBytesMaxLength - cipherBytes.Length));
            }
            return result;
        }

        /// <summary>
        /// 用指定的密匙解密（非标准方法）
        /// </summary>
        /// <param name="cipherBytes">密文字节List</param>
        /// <param name="eOrD">可以是RSACryptoServiceProvider生成的Exponent 或者 D</param>
        /// <param name="n">可以是RSACryptoServiceProvider生成的Modulus</param>
        /// <returns>返回明文</returns>
        public static List<byte> Decrypt(IList<byte> cipherBytes, BigInteger eOrD, BigInteger n)
        {
            int cipherLength = cipherBytes.Count;
            var nBytes = n.ToByteArray();
            int blockSize = nBytes.Length; //cipher block size

            List<byte> result = new List<byte>();

            for (int currentIndex = 0; currentIndex < cipherLength; currentIndex += blockSize)
            {
                BigInteger cipherInteger = new BigInteger(cipherBytes.Skip(currentIndex).Take(blockSize).ToArray());
                BigInteger clearInteger = BigInteger.ModPow(cipherInteger, eOrD, n);
                var clearBytes = clearInteger.ToByteArray();
                result.AddRange(clearBytes);
                result.RemoveAt(result.Count - 1);//跳过补的1 byte
            }
            return result;
        }
        #endregion



        /// <summary>
        /// 获取本机的MAC地址
        /// </summary>
        /// <returns></returns>
        public static string GetLocalMac()
        {
            string mac = null;
            ManagementObjectSearcher query = new ManagementObjectSearcher("SELECT * FROM Win32_NetworkAdapterConfiguration");
            ManagementObjectCollection queryCollection = query.Get();
            foreach (ManagementObject mo in queryCollection)
            {
                if (mo["IPEnabled"].ToString() == "True")
                    mac = mo["MacAddress"].ToString();
            }
            return (mac);
        }

        /// <summary>
        /// 得到CPU序列号
        /// </summary>
        /// <returns></returns>
        public static string GetCpuID()
        {
            try
            {
                //获取CPU序列号代码
                string cpuInfo = "";//cpu序列号
                ManagementClass mc = new ManagementClass("Win32_Processor");
                ManagementObjectCollection moc = mc.GetInstances();
                foreach (ManagementObject mo in moc)
                {
                    cpuInfo = mo.Properties["ProcessorId"].Value.ToString();
                }
                moc = null;
                mc = null;
                return cpuInfo;
            }
            catch
            {
                return "unknow";
            }
        }

        /// <summary>
        /// 获取硬盘ID
        /// </summary>
        /// <returns>硬盘ID</returns>
        public static string GetHardID()
        {
            string HDInfo = "";
            ManagementClass cimobject1 = new ManagementClass("Win32_DiskDrive");
            ManagementObjectCollection moc1 = cimobject1.GetInstances();
            foreach (ManagementObject mo in moc1)
                HDInfo = (string)mo.Properties["Model"].Value;
            return HDInfo;
        }
    }
}
