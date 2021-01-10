using System;
using System.Collections.Generic;
using System.Linq;

namespace ZDevTools.Collections
{
    /// <summary>
    /// 可用于查找与操作的节点结构
    /// </summary>
    /// <typeparam name="TTreeNode">节点泛型参数</typeparam>
    /// <typeparam name="TKey">节点Id泛型参数</typeparam>
    public class TreeNode<TTreeNode, TKey> : TreeNode<TTreeNode>
        where TTreeNode : TreeNode<TTreeNode, TKey>
    {
        #region 属性
        TKey _id;
        /// <summary>
        /// 节点 Id
        /// </summary>
        public TKey Id
        {
            get { return _id; }
            set
            {
                if (Tree != null || Children.Count > 0)
                    throw new TreeNodeException<TTreeNode, TKey>("不能更改附加在树中的或者有子节点的节点的Id！", (TTreeNode)this);
                _id = value;
            }
        }

        TKey _parentId;
        /// <summary>
        /// 父节点 Id
        /// </summary>
        public TKey ParentId
        {
            get { return _parentId; }
            set
            {
                if (Tree != null || Parent != null)
                    throw new TreeNodeException<TTreeNode, TKey>("不能更改已附加在树中的或者具有父节点引用的节点的父节点Id！", (TTreeNode)this);
                _parentId = value;
            }
        }

        IEqualityComparer<TKey> _comparer;

        Tree<TTreeNode, TKey> _tree;
        /// <summary>
        /// 该节点所属树引用
        /// </summary>
        public Tree<TTreeNode, TKey> Tree
        {
            get => _tree;
            internal set
            {
                _tree = value;
                if (_tree != null)
                    _comparer = _tree.Comparer;
            }
        }
        #endregion

        #region 判断
        /// <summary>
        /// 当前节点及子节点中是否包含具有指定Id的节点
        /// </summary>
        public bool Contains(TKey id)
        {
            return contains((TTreeNode)this, id);
        }
        bool contains(TTreeNode node, TKey id)
        {
            if (_comparer.Equals(node.Id, id))
                return true;
            else
                foreach (var child in node.Children)
                    if (contains(child, id))
                        return true;
            return false;
        }

        /// <summary>
        /// 当前节点及子节点中是否包含具有指定的节点
        /// </summary>
        public bool Contains(TTreeNode node) => Contains(node.Id);

        #endregion

        #region 判断祖先
        /// <summary>
        /// 当前节点是否存在指定的祖先
        /// </summary>
        public bool ContainsAncestor(TKey ancestorKey, bool includeSelf = false)
        {
            if (includeSelf && _comparer.Equals(this.Id, ancestorKey)) return true;
            var parent = this.Parent;
            while (parent != null)
            {
                if (_comparer.Equals(parent.Id, ancestorKey)) return true;
                parent = parent.Parent;
            }
            return false;
        }

        /// <summary>
        /// 当前节点是否存在指定的祖先
        /// </summary>
        public bool ContainsAncestor(TTreeNode ancestorNode, bool includeSelf = false) => this.ContainsAncestor(ancestorNode.Id, includeSelf);
        #endregion

        #region 判断后代
        /// <summary>
        /// 是否存在指定后代Key的节点
        /// </summary>
        public bool ContainsDescendant(TKey descendantKey)
        {
            foreach (var childNode in this.Children)
                if (childNode.Contains(descendantKey)) return true;
            return false;
        }

        /// <summary>
        /// 是否存在指定后代节点
        /// </summary>
        public bool ContainsDescendant(TTreeNode descendantNode) => this.ContainsDescendant(descendantNode.Id);
        #endregion

        #region 查找
        /// <summary>
        /// 在当前节点及子节点中查找具有指定Id的节点
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public TTreeNode Find(TKey id)
        {
            return find((TTreeNode)this, id);
        }
        TTreeNode find(TTreeNode node, TKey id)
        {
            if (_comparer.Equals(node.Id, id))
                return node;
            else
                foreach (var child in node.Children)
                {
                    var result = find(child, id);
                    if (result != null)
                        return result;
                }
            return null;
        }
        #endregion

        #region 查找祖先
        /// <summary>
        /// 查找具有指定id的当前节点的父节点
        /// </summary>
        public TTreeNode FindAncestor(TKey ancestorKey, bool includeSelf = false)
        {
            if (includeSelf && _comparer.Equals(this.Id, ancestorKey)) return (TTreeNode)this;
            var parent = this.Parent;
            while (parent != null)
            {
                if (_comparer.Equals(parent.Id, ancestorKey)) return parent;
                parent = parent.Parent;
            }
            return null;
        }
        #endregion

        #region 查找后代
        /// <summary>
        /// 在后代节点中寻找具有指定Key的节点
        /// </summary>
        public TTreeNode FindDescendant(TKey descendantKey)
        {
            foreach (var childNode in this.Children)
            {
                var result = childNode.Find(descendantKey);
                if (result != null) return result;
            }
            return null;
        }
        #endregion
    }
}
