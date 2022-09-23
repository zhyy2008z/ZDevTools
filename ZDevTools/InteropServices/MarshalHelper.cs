using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace ZDevTools.InteropServices
{
    /// <summary>
    /// 封送相关辅助方法
    /// </summary>
    public static class MarshalHelper
    {
        /// <summary>
        /// 结构转为字节数组，如果为unmanaged类型使用托管内存模型进行序列化，其他类型使用非托管内存模型进行序列化
        /// </summary>
        public static byte[] StructureToBytes<T>(T structure)
        {
            byte[] result;

            if (IsReferenceOrContainsReferences<T>()) //引用类型
            {
                var typeSize = Marshal.SizeOf(structure);
                result = new byte[typeSize];
                var handle = Marshal.AllocHGlobal(typeSize);
                try
                {
                    Marshal.StructureToPtr(structure, handle, true);
                    Marshal.Copy(handle, result, 0, typeSize);
                }
                finally
                {
                    Marshal.FreeHGlobal(handle);
                }
            }
            else //unmanaged类型
            {
                result = new byte[Unsafe.SizeOf<T>()];
                ref var firstByte = ref Unsafe.As<T, byte>(ref structure);
                Unsafe.CopyBlock(ref result[0], ref firstByte, (uint)result.Length);
            }

            return result;
        }

        /// <summary>
        /// 字节数组转结构，如果为unmanaged类型的使用托管内存模型进行反序列化，其他类型使用非托管内存模型进行反序列化
        /// </summary>
        public static T StructureFromBytes<T>(byte[] bytes)
        {
            T result;

            if (IsReferenceOrContainsReferences<T>()) //引用类型
            {
                var typeSize = Marshal.SizeOf<T>();
                var handle = Marshal.AllocHGlobal(typeSize);
                try
                {
                    Marshal.Copy(bytes, 0, handle, typeSize);
                    result = Marshal.PtrToStructure<T>(handle);
                }
                finally
                {
                    Marshal.FreeHGlobal(handle);
                }
                return result;
            }
            else //unmanaged类型
                result = Unsafe.As<byte, T>(ref bytes[0]);

            return result;
        }

        /// <summary>
        /// 从对象数组序列化为字节数组，如果为unmanaged类型的数组使用托管内存模型进行序列化，其他类型使用非托管内存模型进行序列化
        /// </summary>
        public static byte[] ArrayToBytes<T>(T[] array)
        {
            if (array.Length == 0) return Array.Empty<byte>();

            byte[] result;
            if (IsReferenceOrContainsReferences<T>()) //引用类型及包含引用类型时走平台封送机制
            {
                int size = Marshal.SizeOf<T>();
                result = new byte[size * array.Length];
                GCHandle handle = GCHandle.Alloc(result, GCHandleType.Pinned);
                try
                {
                    nint address = handle.AddrOfPinnedObject();
                    for (int i = 0; i < array.Length; i++)
                        Marshal.StructureToPtr(array[i], address + i * size, false);
                }
                finally
                {
                    handle.Free();
                }
            }
            else //unmanaged类型走内存导出方式
            {
                var typeT = typeof(T);

                result = new byte[Unsafe.SizeOf<T>() * array.Length];
                if (typeT.IsPrimitive || typeT.IsEnum) //基元系类型走BlockCopy
                    Buffer.BlockCopy(array, 0, result, 0, result.Length);
                else //其他类型走转型方式
                    Unsafe.CopyBlock(ref result[0], ref Unsafe.As<T, byte>(ref array[0]), (uint)result.Length);
            }

            return result;
        }

        /// <summary>
        /// 从字节数组反序列化为对象数组，如果为unmanaged类型的数组使用托管内存模型进行反序列化，其他类型使用非托管内存模型进行反序列化
        /// </summary>
        public static T[] ArrayFromBytes<T>(byte[] bytes)
        {
            if (bytes.Length == 0) return Array.Empty<T>();

            T[] result;
            if (IsReferenceOrContainsReferences<T>())
            {
                int size = Marshal.SizeOf<T>();
                result = new T[bytes.Length / size];

                GCHandle handle = GCHandle.Alloc(bytes, GCHandleType.Pinned);
                try
                {
                    nint address = handle.AddrOfPinnedObject();
                    for (int i = 0; i < result.Length; i++)
                        result[i] = Marshal.PtrToStructure<T>(address + i * size);
                }
                finally
                {
                    handle.Free();
                }
            }
            else
            {
                var typeT = typeof(T);
                result = new T[bytes.Length / Unsafe.SizeOf<T>()];
                if (typeT.IsPrimitive || typeT.IsEnum) //基元系类型走BlockCopy
                    Buffer.BlockCopy(bytes, 0, result, 0, bytes.Length);
                else //其他类型走转型方式
                    Unsafe.CopyBlock(ref Unsafe.As<T, byte>(ref result[0]), ref bytes[0], (uint)bytes.Length);
            }
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

        /// <summary>
        /// 指定类型是否是引用类型或者包含引用类型
        /// </summary>
        public static bool IsReferenceOrContainsReferences<T>()
        {
            return PerTypeValues<T>.IsReferenceOrContainsReferences;
        }

        /// <summary>
        /// 针对每种类型计算一个值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        static class PerTypeValues<T>
        {
            /// <summary>
            /// 该类型是否是引用类型或者包含引用类型
            /// </summary>
            public static readonly bool IsReferenceOrContainsReferences = isReferenceOrContainsReferencesCore(typeof(T));

            static bool isReferenceOrContainsReferencesCore(Type type)
            {
                if (type.GetTypeInfo().IsPrimitive)
                {
                    return false;
                }
                if (!type.GetTypeInfo().IsValueType)
                {
                    return true;
                }
                Type underlyingType = Nullable.GetUnderlyingType(type);
                if (underlyingType != null)
                {
                    type = underlyingType;
                }
                if (type.GetTypeInfo().IsEnum)
                {
                    return false;
                }
                foreach (FieldInfo declaredField in type.GetTypeInfo().DeclaredFields)
                {
                    if (!declaredField.IsStatic && isReferenceOrContainsReferencesCore(declaredField.FieldType))
                    {
                        return true;
                    }
                }
                return false;
            }
        }
    }
}