using System.Linq;

namespace ZDevTools.Utilities
{
    public static class StringTools
    {
        public static byte[] HexStringToBytes(string hexString)
        {
            byte[] bytes = new byte[hexString.Length / 2];
            for (int i = 0; i < bytes.Length; i++)
                bytes[i] = byte.Parse(hexString.Substring(i * 2, 2), System.Globalization.NumberStyles.HexNumber);
            return bytes;
        }

        public static string HexStringFromBytes(byte[] bytes)
        {
            return string.Concat(bytes.Select(byt => byt.ToString("x2")));
        }

    }
}
