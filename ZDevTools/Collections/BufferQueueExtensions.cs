using System;
using System.Collections.Generic;
using System.Text;

namespace ZDevTools.Collections
{
    /// <summary>
    /// Buffer Queue 扩展函数
    /// </summary>
    public static class BufferQueueExtensions
    {
        #region Add
        /// <summary>
        /// 在队尾添加
        /// </summary>
        public static void Add<T>(this BufferQueue<T> queue, T item) => queue.Enqueue(item);

        //#if NETCOREAPP
        /// <summary>
        /// 在队尾添加一批元素
        /// </summary>
        public static void AddRange<T>(this BufferQueue<T> queue, ReadOnlySpan<T> span) => queue.Enqueue(span);
        //#else
        ///// <summary>
        ///// 在队尾添加一批元素
        ///// </summary>
        //public static void AddRange<T>(this BufferQueue<T> queue, ArraySegment<T> segment) => queue.Enqueue(segment);
        //#endif
        #endregion

    }
}
