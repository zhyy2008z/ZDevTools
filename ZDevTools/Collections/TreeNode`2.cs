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
        where TKey : IEquatable<TKey>
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

        /// <summary>
        /// 该节点所属树引用
        /// </summary>
        public Tree<TTreeNode, TKey> Tree { get; internal set; }
        #endregion

        #region 解析
        /// <summary>
        /// 该方法用于解析节点列表，解析后组织为节点树结构，并获得唯一根节点（忽略返回的平化节点字典）
        /// </summary>
        /// <param name="nodes">treeNode可枚举对象</param>
        /// <remarks>您必须保证nodes中有且仅有一个根节点，否则会报错。节点排序：后入先出，因此，nodes中排序靠后的节点会最先出现在节点树中</remarks>
        public static TTreeNode Parse(IEnumerable<TTreeNode> nodes) => Parse(null, nodes, out _);

        /// <summary>
        /// 该方法用于解析节点列表，解析后组织为节点树结构，并获得唯一根节点
        /// </summary>
        /// <param name="nodes">treeNode可枚举对象</param>
        /// <param name="flattenNodes">平化的节点字典</param>
        /// <param name="tree">所属Tree</param>
        /// <remarks>您必须保证nodes中有且仅有一个根节点，否则会报错。
        /// 如果整理出错，请不要再次使用这些节点，因为节点状态已更改并且无法保证处于未附加状态中。
        /// </remarks>
        public static TTreeNode Parse(Tree<TTreeNode, TKey> tree, IEnumerable<TTreeNode> nodes, out Dictionary<TKey, TTreeNode> flattenNodes)
        {
            flattenNodes = nodes.ToDictionary(node => node.Id);
            var treeNodes = nodes.Reverse().ToList(); //逆转顺序后可以让nodes节点的形成层级后，同级节点的相对顺序保持不变

            //整理为树
            for (int i = treeNodes.Count - 1; i > -1; i--)
            {
                var current = treeNodes[i];
                //检查条件，不允许已附加到树的节点参与解析
                if (current.Tree != null)
                    throw new TreeNodeException<TTreeNode, TKey>("同一个节点不能同时被多个树引用！", current);
                current.Tree = tree;
                TTreeNode parent;
                flattenNodes.TryGetValue(current.ParentId, out parent);
                if (parent != null && !current.ParentId.Equals(current.Id)) //有父节点且父节点不是自己，这才能判定为根节点，根节点的Parent属性值是null
                {
                    current.Parent = parent;
                    ((IList<TTreeNode>)parent.Children).Add(current);
                    treeNodes.Remove(current);
                }
            }

            if (treeNodes.Count > 1)
                throw new TreeNodeException<TTreeNode, TKey>("整理失败，发现多个节点疑似根节点！");

            return treeNodes.Count == 1 ? treeNodes[0] : null;
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
        static bool contains(TTreeNode node, TKey id)
        {
            if (node.Id.Equals(id))
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
            if (includeSelf && this.Id.Equals(ancestorKey)) return true;
            var parent = this.Parent;
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
        static TTreeNode find(TTreeNode node, TKey id)
        {
            if (node.Id.Equals(id))
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
            if (includeSelf && this.Id.Equals(ancestorKey)) return (TTreeNode)this;
            var parent = this.Parent;
            while (parent != null)
            {
                if (parent.Id.Equals(ancestorKey)) return parent;
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
