using System;
using System.Collections.Generic;
using System.Linq;
#if NET7_0_OR_GREATER
using System.Numerics;
#endif

namespace ZDevTools.Collections
{
    /// <summary>
    /// 可枚举类型扩展方法
    /// </summary>
    public static class MyEnumerableExtensions
    {
        ///// <summary>
        ///// 索引枚举器
        ///// </summary>
        //public struct IndexedEnumerable<T>
        //{
        //    readonly IEnumerable<T> Values;

        //    /// <summary>
        //    /// 初始化枚举器
        //    /// </summary>
        //    /// <param name="values"></param>
        //    public IndexedEnumerable(IEnumerable<T> values) => this.Values = values;

        //    /// <summary>
        //    /// 获取迭代器
        //    /// </summary>
        //    public Enumerator GetEnumerator() => new Enumerator(Values.GetEnumerator());

        //    /// <summary>
        //    /// 迭代器
        //    /// </summary>
        //    public struct Enumerator
        //    {
        //        readonly IEnumerator<T> InnerEnumerator;
        //        int _index;

        //        /// <summary>
        //        /// 初始化迭代器
        //        /// </summary>
        //        public Enumerator(IEnumerator<T> enumerator)
        //        {
        //            this.InnerEnumerator = enumerator;
        //            _index = -1;
        //        }

        //        /// <summary>
        //        /// 迭代到下一个
        //        /// </summary>
        //        public bool MoveNext()
        //        {
        //            _index++;
        //            return InnerEnumerator.MoveNext();
        //        }

        //        /// <summary>
        //        /// 当前值
        //        /// </summary>
        //        public (int Index, T Item) Current => (_index, InnerEnumerator.Current);
        //    }
        //}

        ///// <summary>
        ///// 为序列增加索引字段以配合foreach增加索引值
        ///// </summary>
        //public static IndexedEnumerable<T> Indexed<T>(this IEnumerable<T> items)
        //{
        //    if (items == null) throw new ArgumentNullException(nameof(items));

        //    return new IndexedEnumerable<T>(items);
        //}

        ///<summary>Finds the index of the first item matching an expression in an enumerable.</summary>
        ///<param name="items">The enumerable to search.</param>
        ///<param name="predicate">The expression to test the items against.</param>
        ///<returns>The index of the first matching item, or -1 if no items match.</returns>
        public static int FindIndex<T>(this IEnumerable<T> items, Predicate<T> predicate)
        {
            if (items == null) throw new ArgumentNullException(nameof(items));
            if (predicate == null) throw new ArgumentNullException(nameof(predicate));

            switch (items)
            {
                case T[] arr:
                    return Array.FindIndex(arr, predicate);
                case List<T> list:
                    return list.FindIndex(predicate);
                default:
                    int i = 0;
                    foreach (var item in items)
                    {
                        if (predicate(item)) return i;
                        i++;
                    }
                    return -1;
            }
        }

        ///<summary>Finds the index of the first occurrence of an item in an enumerable.</summary>
        ///<param name="items">The enumerable to search.</param>
        ///<param name="item">The item to find.</param>
        ///<returns>The index of the first matching item, or -1 if the item was not found.</returns>
        public static int IndexOf<T>(this IEnumerable<T> items, T item)
        {
            if (items == null) throw new ArgumentNullException(nameof(items));

            switch (items)
            {
                case IList<T> list:
                    return list.IndexOf(item);
                default:
                    return items.FindIndex(i => EqualityComparer<T>.Default.Equals(item, i));
            }
        }

        ///<summary>Finds the last index of the first item matching an expression in an enumerable.</summary>
        ///<param name="items">The enumerable to search.</param>
        ///<param name="predicate">The expression to test the items against.</param>
        ///<returns>The index of the first matching item, or -1 if no items match.</returns>
        ///<remarks>
        /// 注意：不要将此方法用于无限元素序列
        /// </remarks>
        public static int FindLastIndex<T>(this IEnumerable<T> items, Predicate<T> predicate)
        {
            if (items == null) throw new ArgumentNullException(nameof(items));
            if (predicate == null) throw new ArgumentNullException(nameof(predicate));

            switch (items)
            {
                case T[] arr:
                    return Array.FindLastIndex(arr, predicate);
                case List<T> list:
                    return list.FindLastIndex(predicate);
                default:
                    var count = items.Count();
                    int i = 0;

                    foreach (var item in items.Reverse())
                    {
                        i++;
                        if (predicate(item)) return count - i;
                    }

                    return -1;
            }
        }

        ///<summary>Finds the last index of the first occurrence of an item in an enumerable.</summary>
        ///<param name="items">The enumerable to search.</param>
        ///<param name="item">The item to find.</param>
        ///<returns>The index of the first matching item, or -1 if the item was not found.</returns>
        ///<remarks>
        /// 注意：不要将此方法用于无限元素序列
        /// </remarks>
        public static int LastIndexOf<T>(this IEnumerable<T> items, T item)
        {
            if (items == null) throw new ArgumentNullException(nameof(items));
            switch (items)
            {
                case T[] arr:
                    return Array.LastIndexOf(arr, item);
                case List<T> list:
                    return list.LastIndexOf(item);
                default:
                    return items.FindLastIndex(i => EqualityComparer<T>.Default.Equals(item, i));
            }
        }

        /// <summary>
        /// 获取一个序列中最小值的索引及最小值本身
        /// </summary>
        public static int MinValueIndex<T>(this IEnumerable<T> items, out T minValue)
            where T : IComparable<T>
        {
            if (items == null) throw new ArgumentNullException(nameof(items));

            int result = -1;
            int i = 0;
            minValue = default;
            foreach (var item in items)
            {
                if (i == 0 || minValue.CompareTo(item) > 0)
                {
                    minValue = item;
                    result = i;
                }
                i++;
            }
            return result;
        }

        /// <summary>
        /// 获取一个序列中最大值的索引及最大值本身
        /// </summary>
        public static int MaxValueIndex<T>(this IEnumerable<T> items, out T maxValue)
                        where T : IComparable<T>
        {
            if (items == null) throw new ArgumentNullException(nameof(items));

            int result = -1;
            int i = 0;
            maxValue = default;
            foreach (var item in items)
            {
                if (i == 0 || maxValue.CompareTo(item) < 0)
                {
                    maxValue = item;
                    result = i;
                }
                i++;
            }

            return result;
        }

        /// <summary>
        /// 获取一个序列中最小值的索引及最小值本身
        /// </summary>
        public static int MinValueIndex<T>(this IEnumerable<T> items, Comparison<T> comparison, out T minValue)
        {
            if (items == null) throw new ArgumentNullException(nameof(items));
            if (comparison == null) throw new ArgumentNullException(nameof(comparison));

            int result = -1;
            int i = 0;
            minValue = default;
            foreach (var item in items)
            {
                if (i == 0)
                {
                    minValue = item;
                    result = 0;
                }
                else
                {
                    if (comparison(minValue, item) > 0)
                    {
                        minValue = item;
                        result = i;
                    }
                }
                i++;
            }
            return result;
        }

        /// <summary>
        /// 获取一个序列中最大值的索引及最大值本身
        /// </summary>
        public static int MaxValueIndex<T>(this IEnumerable<T> items, Comparison<T> comparison, out T maxValue)
        {
            if (items == null) throw new ArgumentNullException(nameof(items));
            if (comparison == null) throw new ArgumentNullException(nameof(comparison));

            int result = -1;
            int i = 0;
            maxValue = default;
            foreach (var item in items)
            {
                if (i == 0)
                {
                    maxValue = item;
                    result = 0;
                }
                else
                {
                    if (comparison(maxValue, item) < 0)
                    {
                        maxValue = item;
                        result = i;
                    }
                }
                i++;
            }

            return result;
        }

        /// <summary>
        /// 获取由区间 [<paramref name="startIndex"/>, <paramref name="endIndex"/>] 代表的一组序列
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="items"></param>
        /// <param name="startIndex"></param>
        /// <param name="endIndex"></param>
        /// <returns></returns>
        public static IEnumerable<T> Range<T>(this IEnumerable<T> items, int startIndex, int endIndex)
        {
            return items.Skip(startIndex).Take(endIndex - startIndex + 1);
        }

        /// <summary>
        /// 获取由区间 [<paramref name="startIndex"/>, <paramref name="endIndex"/>) 代表的一组序列
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="items"></param>
        /// <param name="startIndex"></param>
        /// <param name="endIndex"></param>
        /// <returns></returns>
        public static IEnumerable<T> IntervalL<T>(this IEnumerable<T> items, int startIndex, int endIndex)
        {
            return items.Skip(startIndex).Take(endIndex - startIndex);
        }

        /// <summary>
        /// 获取由区间 (<paramref name="startIndex"/>, <paramref name="endIndex"/>] 代表的一组序列
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="items"></param>
        /// <param name="startIndex"></param>
        /// <param name="endIndex"></param>
        /// <returns></returns>
        public static IEnumerable<T> IntervalR<T>(this IEnumerable<T> items, int startIndex, int endIndex)
        {
            return items.Skip(startIndex + 1).Take(endIndex - startIndex);
        }

        /// <summary>
        /// 获取由区间 (<paramref name="startIndex"/>, <paramref name="endIndex"/>) 代表的一组序列
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="items"></param>
        /// <param name="startIndex"></param>
        /// <param name="endIndex"></param>
        /// <returns></returns>
        public static IEnumerable<T> Interval<T>(this IEnumerable<T> items, int startIndex, int endIndex)
        {
            return items.Skip(startIndex + 1).Take(endIndex - startIndex - 1);
        }

        /// <summary>
        /// 返回序列的一阶差分序列
        /// </summary>
        public static IEnumerable<double> FirstDifference(this IEnumerable<double> items)
        {
            if (items == null) throw new ArgumentNullException(nameof(items));

            bool isFirst = true;
            double lastValue = default;

            foreach (var item in items)
            {
                if (isFirst)
                {
                    isFirst = false;
                }
                else
                {
                    yield return item - lastValue;
                }
                lastValue = item;
            }
        }

        /// <summary>
        /// 返回序列的一阶差分序列
        /// </summary>
        public static IEnumerable<float> FirstDifference(this IEnumerable<float> items)
        {
            if (items == null) throw new ArgumentNullException(nameof(items));

            bool isFirst = true;
            float lastValue = default;

            foreach (var item in items)
            {
                if (isFirst)
                {
                    isFirst = false;
                }
                else
                {
                    yield return item - lastValue;
                }
                lastValue = item;
            }
        }

        /// <summary>
        /// 返回序列的一阶差分序列
        /// </summary>
        public static IEnumerable<long> FirstDifference(this IEnumerable<long> items)
        {
            if (items == null) throw new ArgumentNullException(nameof(items));

            bool isFirst = true;
            long lastValue = default;

            foreach (var item in items)
            {
                if (isFirst)
                {
                    isFirst = false;
                }
                else
                {
                    yield return item - lastValue;
                }
                lastValue = item;
            }
        }

        /// <summary>
        /// 返回序列的一阶差分序列
        /// </summary>
        public static IEnumerable<int> FirstDifference(this IEnumerable<int> items)
        {
            if (items == null) throw new ArgumentNullException(nameof(items));

            bool isFirst = true;
            int lastValue = default;

            foreach (var item in items)
            {
                if (isFirst)
                {
                    isFirst = false;
                }
                else
                {
                    yield return item - lastValue;
                }
                lastValue = item;
            }
        }

        /// <summary>
        /// 返回序列的一阶差分序列
        /// </summary>
        public static IEnumerable<TimeSpan> FirstDifference(this IEnumerable<DateTime> items)
        {
            if (items == null) throw new ArgumentNullException(nameof(items));
            bool isFirst = true;
            DateTime lastValue = default;

            foreach (var item in items)
            {
                if (isFirst)
                {
                    isFirst = false;
                }
                else
                {
                    yield return item - lastValue;
                }
                lastValue = item;
            }
        }

#if NET7_0_OR_GREATER
        /// <summary>
        /// 返回序列的一阶差分序列
        /// </summary>
        public static IEnumerable<T> FirstDifference<T>(this IEnumerable<T> items) where T : ISubtractionOperators<T, T, T>
        {
            if (items == null) throw new ArgumentNullException(nameof(items));

            bool isFirst = true;
            T lastValue = default;

            foreach (var item in items)
            {
                if (isFirst)
                {
                    isFirst = false;
                }
                else
                {
                    yield return item - lastValue;
                }
                lastValue = item;
            }
        }
#endif

        /// <summary>
        /// 序列变为BufferQueue
        /// </summary>
        public static BufferQueue<T> ToBufferQueue<T>(this IEnumerable<T> source)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }
            return new BufferQueue<T>(source);
        }
    }
}
