using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZDevTools.Collections
{
    /// <summary>
    /// 树节点扩展函数
    /// </summary>
    public static class TreeNodeExtensions
    {
        #region 判断祖先
        /// <summary>
        /// 当前节点是否存在指定的祖先
        /// </summary>
        public static bool ContainsAncestor<TTreeNode, TKey>(this TTreeNode node, TKey ancestorKey, bool includeSelf = false)
            where TTreeNode : TreeNode<TTreeNode, TKey>
            where TKey : IEquatable<TKey>
        {
            if (includeSelf && node.Id.Equals(ancestorKey)) return true;
            var parent = node.Parent;
            while (parent != null)
            {
                if (parent.Id.Equals(ancestorKey)) return true;
                parent = parent.Parent;
            }
            return false;
        }

        /// <summary>
        /// 当前节点是否存在指定的祖先
        /// </summary>
        public static bool ContainsAncestor<TTreeNode, TKey>(this TTreeNode node, TTreeNode ancestorNode, bool includeSelf = false)
            where TTreeNode : TreeNode<TTreeNode, TKey>
            where TKey : IEquatable<TKey> => node.ContainsAncestor(ancestorNode.Id, includeSelf);

        /// <summary>
        /// 当前节点是否存在指定的祖先
        /// </summary>
        public static bool ContainsAncestor<T>(this T node, Func<T, bool> predicate, bool includeSelf = false)
            where T : TreeNode<T>
        {
            if (includeSelf && predicate(node)) return true;
            var parent = node.Parent;
            while (parent != null)
            {
                if (predicate(parent)) return true;
                parent = parent.Parent;
            }
            return false;
        }
        #endregion

        #region 查找祖先
        /// <summary>
        /// 查找具有指定id的当前节点的父节点
        /// </summary>
        public static TTreeNode FindAncestor<TTreeNode, TKey>(this TTreeNode node, TKey ancestorKey, bool includeSelf = false)
            where TTreeNode : TreeNode<TTreeNode, TKey>
            where TKey : IEquatable<TKey>
        {
            if (includeSelf && node.Id.Equals(ancestorKey)) return node;
            var parent = node.Parent;
            while (parent != null)
            {
                if (parent.Id.Equals(ancestorKey)) return parent;
                parent = parent.Parent;
            }
            return null;
        }

        /// <summary>
        /// 获取离当前节点最近的可以通过指定断言的祖先
        /// </summary>
        public static T FindAncestor<T>(this T node, Func<T, bool> predicate, bool includeSelf = false)
            where T : TreeNode<T>
        {
            if (includeSelf && predicate(node)) return node;
            var parent = node.Parent;
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
        public static List<T> FindAllAncestors<T>(this T node, Func<T, bool> predicate, bool includeSelf = false)
            where T : TreeNode<T>
        {
            List<T> result = new List<T>();
            if (includeSelf && predicate(node)) result.Add(node);
            var parent = node.Parent;
            while (parent != null)
            {
                if (predicate(parent))
                    result.Add(parent);
                parent = parent.Parent;
            }
            return result;
        }
        #endregion

        #region 祖先线性化
        /// <summary>
        /// 获取指定节点所有的祖先（按照亲疏排序）
        /// </summary>
        /// <param name="includeSelf">是否将当前节点包含在内</param>
        /// <param name="node">当前节点</param>
        public static List<T> AncestorsToList<T>(this T node, bool includeSelf = false)
            where T : TreeNode<T>
        {
            List<T> result = new List<T>();
            if (includeSelf) result.Add(node);
            var parent = node.Parent;
            while (parent != null)
            {
                result.Add(parent);
                parent = parent.Parent;
            }
            return result;
        }
        #endregion

        #region 判断后代
        /// <summary>
        /// 是否存在指定后代Key的节点
        /// </summary>
        public static bool ContainsDescendant<TTreeNode, TKey>(this TTreeNode node, TKey descendantKey)
            where TTreeNode : TreeNode<TTreeNode, TKey>
            where TKey : IEquatable<TKey>
        {
            foreach (var childNode in node.Children)
                if (childNode.Contains(descendantKey)) return true;
            return false;
        }

        /// <summary>
        /// 是否存在指定后代节点
        /// </summary>
        public static bool ContainsDescendant<TTreeNode, TKey>(this TTreeNode node, TTreeNode descendantNode)
            where TTreeNode : TreeNode<TTreeNode, TKey>
            where TKey : IEquatable<TKey> => node.ContainsDescendant(descendantNode.Id);

        /// <summary>
        /// 是否存在通过指定断言的后代节点
        /// </summary>
        public static bool ContainsDescendant<T>(this T node, Func<T, bool> predicate)
            where T : TreeNode<T>
        {
            foreach (var childNode in node.Children)
                if (childNode.Contains(predicate)) return true;
            return false;
        }
        #endregion

        #region 查找后代
        /// <summary>
        /// 在后代节点中寻找具有指定Key的节点
        /// </summary>
        public static TTreeNode FindDescendant<TTreeNode, TKey>(this TTreeNode node, TKey descendantKey)
            where TTreeNode : TreeNode<TTreeNode, TKey>
            where TKey : IEquatable<TKey>
        {
            foreach (var childNode in node.Children)
            {
                var result = childNode.Find(descendantKey);
                if (result != null) return result;
            }
            return null;
        }

        /// <summary>
        /// 在后代节点中寻找具有指定Key的节点
        /// </summary>
        public static T FindDescendant<T>(this T node, Func<T, bool> predicate)
            where T : TreeNode<T>
        {
            foreach (var childNode in node.Children)
            {
                var result = childNode.Find(predicate);
                if (result != null) return result;
            }
            return null;
        }

        /// <summary>
        /// 在后代节点中寻找所有能够通过断言的节点
        /// </summary>
        public static List<T> FindAllDescendant<T>(this T node, Func<T, bool> predicate)
            where T : TreeNode<T>
        {
            var result = new List<T>();
            foreach (var childNode in node.Children)
                linear(childNode, result, predicate);
            return result;
        }

        static void linear<T>(T node, List<T> list, Func<T, bool> predicate)
            where T : TreeNode<T>
        {
            if (predicate(node))
                list.Add(node);
            foreach (var item in node.Children)
                linear(item, list, predicate);
        }
        #endregion        
    }
}
