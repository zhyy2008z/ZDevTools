using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using ZDevTools.Security;


namespace ZDevTools.Test.Security
{
    public class UnitTest_RSAHelper
    {
        [Fact]
        public RSAHelper.RSAKey GenerateKey()
        {
            var rsaKey = RSAHelper.GenerateRASKey();
            Assert.True(!string.IsNullOrEmpty(rsaKey.PrivateKey));
            Assert.True(!string.IsNullOrEmpty(rsaKey.PublicKey));

            return rsaKey;
        }


        [Fact]
        public (List<byte> CipherBytes, RSAHelper.RSAKey Key, string Clear) Encrypt()
        {
            var key = GenerateKey();

            string clear = "\0\0\0测试一级棒\0\0\0skfsdlkfklsjfliwu;" +
                "skjflksjf;s" +
                "sdfsdklfjsdlfjsl;ufeio" +
                "sdfjsldjfoieurpjzavna" +
                "skfjskfjsldfj";
            var clearBytes = Encoding.Default.GetBytes(clear);

            //测试私钥加密
            var cipherBytes = RSAHelper.Encrypt(clearBytes, key.PrivateKey);

            Assert.NotNull(cipherBytes);

            return (cipherBytes, key, clear);
        }

        [Fact]
        public void Decrypt()
        {
            (var cipherBytes, var key, var clear) = Encrypt();

            //公钥解密
            var clearBytes = RSAHelper.Decrypt(cipherBytes, key.PublicKey);

            var deClear = Encoding.Default.GetString(clearBytes.ToArray());

            //原文与解密后的原文必须一致
            Assert.True(clear == deClear);
        }

    }
}
