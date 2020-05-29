using System;
using System.Collections.Generic;
using System.Text;

namespace ZDevTools.Collections
{
    /// <summary>
    /// 针对列表结构的扩展方法
    /// </summary>
    public static class MyListExtensions
    {
        /// <summary>
        /// 不允许重复匹配的搜索方法(从2,2,2,2,2中查找2,2，结果：0,2)
        /// </summary>
        /// <param name="list"></param>
        /// <param name="pattern"></param>
        /// <returns></returns>
        public static List<int> SearchAll<T>(this IReadOnlyList<T> list, IReadOnlyList<T> pattern)
            where T : IEquatable<T>
        {
            if (list == null) throw new ArgumentNullException(nameof(list));
            if (pattern == null) throw new ArgumentNullException(nameof(pattern));
            if (pattern.Count == 0)
                throw new ArgumentException("模式数组不能为空！");

            List<int> locations = new List<int>();
            var count = list.Count - pattern.Count + 1;
            for (int i = 0; i < count;)
            {
                if (isMatch(list, i, pattern))
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
        /// <param name="list"></param>
        /// <param name="pattern"></param>
        /// <returns></returns>
        public static List<int> SearchAllr<T>(this IReadOnlyList<T> list, IReadOnlyList<T> pattern)
           where T : IEquatable<T>
        {
            if (list == null) throw new ArgumentNullException(nameof(list));
            if (pattern == null) throw new ArgumentNullException(nameof(pattern));
            if (pattern.Count == 0)
                throw new ArgumentException("模式数组不能为空！");

            List<int> locations = new List<int>();
            var count = list.Count - pattern.Count + 1;
            for (int i = 0; i < count; i++)
            {
                if (isMatch(list, i, pattern))
                    locations.Add(i);
            }

            return locations;
        }

        /// <summary>
        /// 找出第一个匹配项
        /// </summary>
        /// <param name="list"></param>
        /// <param name="pattern"></param>
        /// <returns></returns>
        public static int IndexOf<T>(this IReadOnlyList<T> list, IReadOnlyList<T> pattern)
            where T : IEquatable<T>
        {
            if (list == null) throw new ArgumentNullException(nameof(list));
            if (pattern == null) throw new ArgumentNullException(nameof(pattern));
            if (pattern.Count == 0)
                throw new ArgumentException("模式数组不能为空！");

            var count = list.Count - pattern.Count + 1;
            for (int i = 0; i < count; i++)
            {
                if (isMatch(list, i, pattern))
                    return i;
            }

            return -1;
        }

        /// <summary>
        /// 从后向前查找第一个匹配项
        /// </summary>
        /// <param name="list"></param>
        /// <param name="pattern"></param>
        /// <returns></returns>
        public static int LastIndexOf<T>(this IReadOnlyList<T> list, IReadOnlyList<T> pattern)
            where T : IEquatable<T>
        {
            if (list == null) throw new ArgumentNullException(nameof(list));
            if (pattern == null) throw new ArgumentNullException(nameof(pattern));
            if (pattern.Count == 0)
                throw new ArgumentException("模式数组不能为空！");

            var count = list.Count - pattern.Count + 1;
            for (int i = count - 1; i > -1; i--)
            {
                if (isMatch(list, i, pattern))
                    return i;
            }

            return -1;
        }

        static bool isMatch<T>(IReadOnlyList<T> list, int position, IReadOnlyList<T> pattern)
            where T : IEquatable<T>
        {
            for (int i = 0; i < pattern.Count; i++)
            {
                if (!pattern[i].Equals(list[position + i]))
                    return false;
            }
            return true;
        }
    }
}
