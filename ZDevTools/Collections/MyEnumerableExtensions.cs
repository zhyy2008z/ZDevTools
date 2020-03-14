using System;
using System.Collections.Generic;
using System.Linq;

namespace ZDevTools.Collections
{
    public static class MyEnumerableExtensions
    {
        ///<summary>Finds the index of the first item matching an expression in an enumerable.</summary>
        ///<param name="items">The enumerable to search.</param>
        ///<param name="predicate">The expression to test the items against.</param>
        ///<returns>The index of the first matching item, or -1 if no items match.</returns>
        public static int FindIndex<T>(this IEnumerable<T> items, Predicate<T> predicate)
        {
            if (items == null) throw new ArgumentNullException("items");
            if (predicate == null) throw new ArgumentNullException("predicate");

            int retVal = 0;
            foreach (var item in items)
            {
                if (predicate(item)) return retVal;
                retVal++;
            }
            return -1;
        }

        public static int FindIndex<T>(this T[] array, Predicate<T> match) => Array.FindIndex(array, match);

        ///<summary>Finds the index of the first occurrence of an item in an enumerable.</summary>
        ///<param name="items">The enumerable to search.</param>
        ///<param name="item">The item to find.</param>
        ///<returns>The index of the first matching item, or -1 if the item was not found.</returns>
        public static int IndexOf<T>(this IEnumerable<T> items, T item) { return items.FindIndex(i => EqualityComparer<T>.Default.Equals(item, i)); }

        /// <summary>
        /// 查找数组的扩展方法
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="array"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static int IndexOf<T>(this T[] array, T value) { return Array.IndexOf(array, value); }

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


        /// <summary>
        /// 不允许重复匹配的搜索方法(从2,2,2,2,2中查找2,2，结果：0,2)
        /// </summary>
        /// <param name="items"></param>
        /// <param name="pattern"></param>
        /// <returns></returns>
        public static List<int> SearchAll<T>(this IReadOnlyList<T> items, IReadOnlyList<T> pattern)
            where T : IEquatable<T>
        {
            if (pattern.Count == 0)
                throw new ArgumentException("模式数组不能为空！");

            List<int> locations = new List<int>();
            var count = items.Count - pattern.Count + 1;
            for (int i = 0; i < count;)
            {
                if (isMatch(items, i, pattern))
                {
                    locations.Add(i);
                    i += pattern.Count;
                }
                else
                    i++;
            }
            return locations;
        }


        /// <summary>
        /// 允许重复匹配的搜索方法（从2,2,2,2,2中查找2,2，结果：0,1,2,3）
        /// </summary>
        /// <param name="items"></param>
        /// <param name="pattern"></param>
        /// <returns></returns>
        public static List<int> SearchAllr<T>(this IReadOnlyList<T> items, IReadOnlyList<T> pattern)
           where T : IEquatable<T>
        {
            List<int> locations = new List<int>();
            var count = items.Count - pattern.Count + 1;
            for (int i = 0; i < count; i++)
            {
                if (isMatch(items, i, pattern))
                    locations.Add(i);
            }
            return locations;
        }

        /// <summary>
        /// 找出第一个匹配项
        /// </summary>
        /// <param name="items"></param>
        /// <param name="pattern"></param>
        /// <returns></returns>
        public static int IndexOf<T>(this IReadOnlyList<T> items, IReadOnlyList<T> pattern)
            where T : IEquatable<T>
        {
            var count = items.Count - pattern.Count + 1;
            for (int i = 0; i < count; i++)
            {
                if (isMatch(items, i, pattern))
                    return i;
            }
            return -1;
        }


        /// <summary>
        /// 从后向前查找第一个匹配项
        /// </summary>
        /// <param name="items"></param>
        /// <param name="pattern"></param>
        /// <returns></returns>
        public static int LastIndexOf<T>(this IReadOnlyList<T> items, IReadOnlyList<T> pattern)
            where T : IEquatable<T>
        {
            var count = items.Count - pattern.Count + 1;
            for (int i = count - 1; i > -1; i--)
            {
                if (isMatch(items, i, pattern))
                    return i;
            }
            return -1;
        }

        static bool isMatch<T>(IReadOnlyList<T> items, int position, IReadOnlyList<T> pattern)
            where T : IEquatable<T>
        {
            for (int i = 0; i < pattern.Count; i++)
            {
                if (!pattern[i].Equals(items[position + i]))
                    return false;
            }
            return true;
        }




    }
}
