namespace ZDevTools.NetCore
{
    public interface IEncdecProvider
    {
        string Decrypt(string ciphertext);
        string Encrypt(string cleartext);

        byte[] Encrypt(byte[] clearBytes);

        byte[] Decrypt(byte[] cipherBytes);
    }
}