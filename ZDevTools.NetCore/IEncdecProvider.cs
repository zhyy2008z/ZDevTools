namespace ZDevTools.NetCore
{
    /// <summary>
    /// 对称加密服务
    /// </summary>
    public interface IEncdecProvider
    {
        /// <summary>
        /// 解密文本（UTF-8编码）
        /// </summary>
        /// <param name="ciphertext"></param>
        /// <returns></returns>
        string Decrypt(string ciphertext);

        /// <summary>
        /// 加密文本（UTF-8编码）
        /// </summary>
        /// <param name="cleartext"></param>
        /// <returns></returns>
        string Encrypt(string cleartext);

        /// <summary>
        /// 加密二进制
        /// </summary>
        /// <param name="clearBytes"></param>
        /// <returns></returns>
        byte[] Encrypt(byte[] clearBytes);

        /// <summary>
        /// 解密二进制
        /// </summary>
        /// <param name="cipherBytes"></param>
        /// <returns></returns>
        byte[] Decrypt(byte[] cipherBytes);
    }
}