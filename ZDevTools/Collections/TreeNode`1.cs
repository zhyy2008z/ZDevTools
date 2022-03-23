using System;
using System.Collections.Generic;

namespace ZDevTools.Collections
{
    /// <summary>
    /// 最基本的节点结构
    /// </summary>
    /// <typeparam name="T">节点类型</typeparam>
    public class TreeNode<T>
        where T : TreeNode<T>
    {
        #region 属性
        /// <summary>
        /// 孩子节点
        /// </summary>
        public IReadOnlyList<T> Children { get; internal set; } = new List<T>();

        /// <summary>
        /// 父节点
        /// </summary>
        public T Parent { get; internal set; }

        /// <summary>
        /// 获取当前节点在父节点中的位置，-1代表没有父节点
        /// </summary>
        public int Index
        {
            get
            {
                if (Parent == null)
                    return -1;
                else
                    return ((List<T>)Parent.Children).IndexOf((T)this);
            }
        }
        #endregion

        #region 线性化
        /// <summary>
        /// 将当前节点及子节点线性化为列表
        /// </summary>
        /// <returns></returns>
        public List<T> AllToList()
        {
            var list = new List<T>();
            linear((T)this, list);
            return list;
        }
        static void linear(T node, List<T> list)
        {
            list.Add(node);

            foreach (var item in node.Children)
            {
                linear(item, list);
            }
        }

        /// <summary>
        /// 获取指定节点所有的祖先（按照亲疏排序）
        /// </summary>
        /// <param name="includeSelf">是否将当前节点包含在内</param>
        public List<T> AncestorToList(bool includeSelf = false)
        {
            List<T> result = new List<T>();
            if (includeSelf) result.Add((T)this);
            var parent = this.Parent;
            while (parent != null)
            {
                result.Add(parent);
                parent = parent.Parent;
            }
            return result;
        }

        /// <summary>
        /// 将所有子节点线性化为列表
        /// </summary>
        /// <returns></returns>
        public List<T> SubToList()
        {
            var list = new List<T>();

            linearSub((T)this, list);

            return list;
        }
        static void linearSub(T node, List<T> list)
        {
            foreach (var item in node.Children)
            {
                list.Add(item);
                linearSub(item, list);
            }
        }
        #endregion

        #region 查找
        /// <summary>
        /// 在当前节点及其所有子节点中查找所有符合断言的节点
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public List<T> FindAll(Func<T, bool> predicate)
        {
            var list = new List<T>();
            Linear((T)this, list, predicate);
            return list;
        }

        internal static void Linear(T node, List<T> list, Func<T, bool> predicate)
        {
            if (predicate(node))
                list.Add(node);

            foreach (var item in node.Children)
            {
                Linear(item, list, predicate);
            }
        }

        /// <summary>
        /// 查找第一个发现的可以通过断言的节点
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public T Find(Func<T, bool> predicate)
        {
            return find((T)this, predicate);
        }
        static T find(T node, Func<T, bool> predicate)
        {
            if (predicate(node))
                return node;
            else
                foreach (var child in node.Children)
                {
                    var result = find(child, predicate);
                    if (result != null)
                        return result;
                }
            return null;
        }
        #endregion

        #region 查找祖先
        /// <summary>
        /// 获取离当前节点最近的可以通过指定断言的祖先
        /// </summary>
        public T FindAncestor(Func<T, bool> predicate, bool includeSelf = false)
        {
            if (includeSelf && predicate((T)this)) return (T)this;
            var parent = this.Parent;
            while (parent != null)
            {
                if (predicate(parent)) return parent;
                parent = parent.Parent;
            }
            return null;
        }

        /// <summary>
        /// 获取所有通过指定断言的祖先（按照亲疏排序）
        /// </summary>
        public List<T> FindAllAncestors(Func<T, bool> predicate, bool includeSelf = false)
        {
            List<T> result = new List<T>();
            if (includeSelf && predicate((T)this)) result.Add((T)this);
            var parent = this.Parent;
            while (parent != null)
            {
                if (predicate(parent))
                    result.Add(parent);
                parent = parent.Parent;
            }
            return result;
        }
        #endregion

        #region 查找后代
        /// <summary>
        /// 在后代节点中寻找第一个通过断言的节点
        /// </summary>
        public T FindDescendant(Func<T, bool> predicate)
        {
            foreach (var childNode in this.Children)
            {
                var result = childNode.Find(predicate);
                if (result != null) return result;
            }
            return null;
        }

        /// <summary>
        /// 在后代节点中寻找所有能够通过断言的节点
        /// </summary>
        public List<T> FindAllDescendant(Func<T, bool> predicate)
        {
            var result = new List<T>();
            foreach (var childNode in this.Children)
                Linear(childNode, result, predicate);
            return result;
        }
        #endregion

        #region 判断
        /// <summary>
        /// 当前节点及其子节点是否包含能够通过断言的节点
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public bool Contains(Func<T, bool> predicate)
        {
            return contains((T)this, predicate);
        }
        static bool contains(T node, Func<T, bool> predicate)
        {
            if (predicate(node))
                return true;
            else
                foreach (var child in node.Children)
                    if (contains(child, predicate))
                        return true;
            return false;
        }
        #endregion

        #region 判断祖先
        /// <summary>
        /// 当前节点是否存在指定的祖先
        /// </summary>
        public bool ContainsAncestor(Func<T, bool> predicate, bool includeSelf = false)
        {
            if (includeSelf && predicate((T)this)) return true;
            var parent = this.Parent;
            while (parent != null)
            {
                if (predicate(parent)) return true;
                parent = parent.Parent;
            }
            return false;
        }
        #endregion

        #region 判断后代
        /// <summary>
        /// 是否存在通过指定断言的后代节点
        /// </summary>
        public bool ContainsDescendant(Func<T, bool> predicate)
        {
            foreach (var childNode in this.Children)
                if (childNode.Contains(predicate)) return true;
            return false;
        }
        #endregion
    }
}
