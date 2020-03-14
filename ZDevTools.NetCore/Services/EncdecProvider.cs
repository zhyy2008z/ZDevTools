using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Options;

namespace ZDevTools.NetCore.Services
{
    class EncdecProvider : IEncdecProvider
    {
        readonly IOptions<EncdecOptions> Options;
        public EncdecProvider(IOptions<EncdecOptions> options)
        {
            Options = options;
        }

        public string Encrypt(string clearText)
        {
            if (string.IsNullOrEmpty(clearText))
                return clearText;

            return Convert.ToBase64String(Encrypt(Encoding.UTF8.GetBytes(clearText)));
        }

        public string Decrypt(string cipherText)
        {
            if (string.IsNullOrEmpty(cipherText))
                return cipherText;

            return Encoding.UTF8.GetString(Decrypt(Convert.FromBase64String(cipherText)));
        }

        public byte[] Encrypt(byte[] clearBytes)
        {
            if (clearBytes == null)
                throw new ArgumentNullException(nameof(clearBytes));

            using (AesManaged aesManaged = new AesManaged())
            {
                aesManaged.Key = Convert.FromBase64String(Options.Value.EncryptionKey.Substring(0, 32));
                aesManaged.IV = Convert.FromBase64String(Options.Value.EncryptionKey.Substring(32));

                using (var encryptor = aesManaged.CreateEncryptor())
                using (MemoryStream ms = new MemoryStream())
                using (CryptoStream cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                {
                    cs.Write(clearBytes, 0, clearBytes.Length);
                    cs.FlushFinalBlock();
                    return ms.ToArray();
                }
            }
        }

        public byte[] Decrypt(byte[] cipherBytes)
        {
            if (cipherBytes == null)
                throw new ArgumentNullException(nameof(cipherBytes));

            using (AesManaged aesManaged = new AesManaged())
            {
                aesManaged.Key = Convert.FromBase64String(Options.Value.EncryptionKey.Substring(0, 32));
                aesManaged.IV = Convert.FromBase64String(Options.Value.EncryptionKey.Substring(32));

                using (var decryptor = aesManaged.CreateDecryptor())
                using (MemoryStream ms = new MemoryStream())
                using (CryptoStream cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Write))
                {
                    cs.Write(cipherBytes, 0, cipherBytes.Length);
                    cs.FlushFinalBlock();
                    return ms.ToArray();
                }
            }
        }
    }
}
