using System;
using System.Collections.Generic;
using System.Linq;

namespace ZDevTools.Collections
{
    /// <summary>
    /// 可枚举类型扩展方法
    /// </summary>
    public static class MyEnumerableExtensions
    {
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
                case IList<T> list:
                    return list.LastIndexOf(item);
                default:
                    return items.FindLastIndex(i => EqualityComparer<T>.Default.Equals(item, i));
            }
        }

        /// <summary>
        /// 获取一个序列中最小值的索引及最小值本身
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="items"></param>
        /// <param name="initValue"></param>
        /// <param name="minValue"></param>
        /// <returns></returns>
        public static int MinValueIndex<T>(this IEnumerable<T> items, T initValue, out T minValue)
            where T : IComparable<T>
        {
            if (items == null) throw new ArgumentNullException(nameof(items));

            int result = -1;
            int i = 0;
            minValue = initValue;
            foreach (var item in items)
            {
                if (minValue.CompareTo(item) > 0)
                {
                    result = i;
                    minValue = item;
                }
                i++;
            }
            return result;
        }

        /// <summary>
        /// 获取一个序列中最大值的索引及最大值本身
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="items"></param>
        /// <param name="initValue"></param>
        /// <param name="maxValue"></param>
        /// <returns></returns>
        public static int MaxValueIndex<T>(this IEnumerable<T> items, T initValue, out T maxValue)
                        where T : IComparable<T>
        {
            if (items == null) throw new ArgumentNullException(nameof(items));

            int result = -1;
            int i = 0;
            maxValue = initValue;
            foreach (var item in items)
            {
                if (maxValue.CompareTo(item) < 0)
                {
                    maxValue = item;
                    result = i;
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
        /// <param name="items"></param>
        /// <returns></returns>
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
        /// <param name="items"></param>
        /// <returns></returns>
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
    }
}
