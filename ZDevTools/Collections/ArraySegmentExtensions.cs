using System;
using System.Collections.Generic;
using System.Text;

namespace ZDevTools.Collections
{
#if NET
    /// <summary>
    /// 针对数组片段的扩展函数
    /// </summary>
    public static class ArraySegmentExtensions
    {
        /// <summary>
        /// 再次分割一个数组片段
        /// </summary>
        public static ArraySegment<T> Slice<T>(this ArraySegment<T> segment, int index, int count)
        {
            if (segment.Array == null)
                throw new InvalidOperationException();

            if (index > segment.Count || count > (segment.Count - index))
            {
                throw new ArgumentOutOfRangeException();
            }

            return new ArraySegment<T>(segment.Array, segment.Offset + index, count);
        }


        /// <summary>
        /// 再次分割一个数组片段
        /// </summary>
        public static ArraySegment<T> Slice<T>(this ArraySegment<T> segment, int index)
        {
            if (segment.Array == null)
                throw new InvalidOperationException();

            if (index > segment.Count)
            {
                throw new ArgumentOutOfRangeException();
            }

            return new ArraySegment<T>(segment.Array, segment.Offset + index, segment.Count - index);
        }

        /// <summary>
        /// 将本数组片段的元素全部复制到另一个数组片段中，要求目标数组片段的长度必须大于等于本数组片段的长度
        /// </summary>
        public static void CopyTo<T>(this ArraySegment<T> segment, ArraySegment<T> destination)
        {
            if (segment.Array == null)
                throw new InvalidOperationException();

            if (destination.Array == null)
                throw new InvalidOperationException();

            if (segment.Count > destination.Count)
            {
                throw new InvalidOperationException("目标数组片段长度过短。");
            }

            Array.Copy(segment.Array, segment.Offset, destination.Array, destination.Offset, segment.Count);
        }

        /// <summary>
        /// 将本数组片段的元素全部复制到另一个数组中，要求目标数组片段的长度必须 大于等于 本数组片段的长度 + <paramref name="destinationIndex"/>
        /// </summary>
        public static void CopyTo<T>(this ArraySegment<T> segment, T[] destination, int destinationIndex)
        {
            if (segment.Array == null)
                throw new InvalidOperationException();

            Array.Copy(segment.Array, segment.Offset, destination, destinationIndex, segment.Count);
        }

        /// <summary>
        /// 将本数组片段的元素全部复制到另一个数组中，要求目标数组片段的长度必须大于等于本数组片段的长度
        /// </summary>
        public static void CopyTo<T>(this ArraySegment<T> segment, T[] destination)
        {
            segment.CopyTo(destination, 0);
        }
    }
#endif
}
