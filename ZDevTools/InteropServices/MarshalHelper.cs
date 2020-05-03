using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ZDevTools.InteropServices
{
    /// <summary>
    /// 封送相关辅助方法
    /// </summary>
    public static class MarshalHelper
    {

        /// <summary>
        /// 结构转为字节数组
        /// </summary>
        public static byte[] StructureToBytes(object structure)
        {
            var typeSize = Marshal.SizeOf(structure);
            byte[] result = new byte[typeSize];
            var handle = Marshal.AllocHGlobal(typeSize);
            Marshal.StructureToPtr(structure, handle, true);
            Marshal.Copy(handle, result, 0, typeSize);
            Marshal.FreeHGlobal(handle);
            return result;
        }

        /// <summary>
        /// 字节数组转结构
        /// </summary>
        public static T StructureFromBytes<T>(byte[] bytes)
        {
            var typeSize = Marshal.SizeOf<T>();
            var handle = Marshal.AllocHGlobal(typeSize);
            Marshal.Copy(bytes, 0, handle, typeSize);
            var result = Marshal.PtrToStructure<T>(handle);
            Marshal.FreeHGlobal(handle);
            return result;
        }

        /// <summary>
        /// 结构数组转字节数组
        /// </summary>
        public static byte[] ArrayToBytes<T>(T[] array)
        {
            int size;
            var typeT = typeof(T);

            if (typeT.IsEnum)
                size = Marshal.SizeOf(typeT.GetEnumUnderlyingType());
            else
                size = Marshal.SizeOf<T>();

            var bytesCount = size * array.Length;
            var result = new byte[bytesCount];
            Buffer.BlockCopy(array, 0, result, 0, bytesCount);
            return result;
        }

        /// <summary>
        /// 字节数组转结构数组
        /// </summary>
        public static T[] ArrayFromBytes<T>(byte[] bytes)
        {
            int size;
            var typeT = typeof(T);

            if (typeT.IsEnum)
                size = Marshal.SizeOf(typeT.GetEnumUnderlyingType());
            else
                size = Marshal.SizeOf<T>();

            var result = new T[bytes.Length / size];
            Buffer.BlockCopy(bytes, 0, result, 0, bytes.Length);
            return result;
        }

        /// <summary>
        /// 获取具有固定长度的字符串Buffer
        /// </summary>
        /// <param name="str">字符串</param>
        /// <param name="fixedLength">Byte固定长度</param>
        /// <param name="encoding">编码格式</param>
        /// <returns></returns>
        public static byte[] GetFixedBytes(string str, int fixedLength, Encoding encoding)
        {
            byte[] result = new byte[fixedLength];
            var bytes = encoding.GetBytes(str);
            Array.Copy(bytes, result, Math.Min(fixedLength, bytes.Length));
            return result;
        }
    }
}