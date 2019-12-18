using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Reflection;

namespace ZDevTools.Collections
{
    /// <summary>
    /// 树类型（相当于节点树管理器）
    /// </summary>
    /// <typeparam name="TTreeNode"></typeparam>
    /// <typeparam name="TKey"></typeparam>
    /// <remarks>非线程安全，请自行做好线程同步操作</remarks>
    public class Tree<TTreeNode, TKey>
        where TTreeNode : TreeNode<TTreeNode, TKey>
        where TKey : IEquatable<TKey>
    {
        Dictionary<TKey, TTreeNode> _flattenNodes;

        /// <summary>
        /// 初始化一颗树
        /// </summary>
        /// <param name="nodes">一组节点</param>
        public Tree(IEnumerable<TTreeNode> nodes)
        {
            Root = TreeNode<TTreeNode, TKey>.Parse(this, nodes, out _flattenNodes);
        }

        /// <summary>
        /// 根节点
        /// </summary>
        public TTreeNode Root { get; }

        /// <summary>
        /// 当前树是否是空的
        /// </summary>
        public bool IsEmpty { get => Root == null; }

        #region 判断
        /// <summary>
        /// 在树中是否存在 Id 为 id 的节点
        /// </summary>
        public bool Contains(TKey id)
        {
            return _flattenNodes.ContainsKey(id);
        }

        /// <summary>
        /// 在树中是否存在指定节点
        /// </summary>
        public bool Contains(TTreeNode node)
        {
            return _flattenNodes.ContainsKey(node.Id);
        }

        /// <summary>
        /// 在树中是否存在满足predicate的节点
        /// </summary>
        public bool Contains(Func<TTreeNode, bool> predicate)
        {
            return _flattenNodes.Values.Any(predicate);
        }
        #endregion

        #region 查找

        /// <summary>
        /// 查找 Id 为 id 的节点
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public TTreeNode Find(TKey id)
        {
            _flattenNodes.TryGetValue(id, out var result);
            return result;
        }

        /// <summary>
        /// 查找一个满足 <paramref name="predicate"/> 的节点
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public TTreeNode Find(Func<TTreeNode, bool> predicate)
        {
            return _flattenNodes.Values.FirstOrDefault(predicate);
        }

        /// <summary>
        /// 该方法返回的数量可能与ids数量不一致，因为其会抛弃没有对应node的id。
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public IEnumerable<TTreeNode> FindAll(IEnumerable<TKey> ids)
        {
            return ids.Select(id => Find(id)).Where(node => node != null);
        }

        /// <summary>
        /// 返回一组满足 <paramref name="predicate"/> 的节点
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public IEnumerable<TTreeNode> FindAll(Func<TTreeNode, bool> predicate)
        {
            return _flattenNodes.Values.Where(predicate);
        }

        #endregion

        #region 操作

        /// <summary>
        /// 向树中附加一个节点
        /// </summary>
        /// <param name="node"></param>
        /// <remarks>该方法会确定是否可以完成操作再进行操作，不会破坏节点与树的状态</remarks>
        public void AttachNode(TTreeNode node)
        {
            AttachNode(node, -1);
        }

        /// <summary>
        /// 在指定 <paramref name="index"/> 附加一个节点
        /// </summary>
        /// <param name="node"></param>
        /// <param name="index"></param>
        /// <remarks>该方法会确定是否可以完成操作再进行操作，不会破坏节点与树的状态</remarks>
        public void AttachNode(TTreeNode node, int index)
        {
            //这里的node仅考虑全新的node节点和已经整理为树结构的节点（因为只有内部算法才可以整理为树，因此我们可以假设这颗节点树是没有错误的），
            var flattenNodes = node.AllToList();
            var checkInvalidNode = flattenNodes.FirstOrDefault(n => n.Tree != null || Contains(n.Id));
            if (checkInvalidNode != null)
                throw new TreeNodeException<TTreeNode, TKey>("检查到一个节点属于其它树或者在当前树中已存在与该节点Id相同的节点！", checkInvalidNode);

            var parentNode = Find(node.ParentId);
            node.Parent = parentNode ?? throw new TreeNodeException<TTreeNode, TKey>("未在树中找到该节点的父节点，请检查！", node);
            node.Tree = this;

            //检查全部完成，后面的在可管控的范围内理论上不会出现异常
            foreach (var n in flattenNodes)
                _flattenNodes.Add(n.Id, n);

            if (index > -1 && index < ((List<TTreeNode>)parentNode.Children).Count) //在范围之间
                ((List<TTreeNode>)parentNode.Children).Insert(index, node);
            else
                ((List<TTreeNode>)parentNode.Children).Add(node);
        }

        /// <summary>
        /// 从树中分离一个节点（无法分离根节点）
        /// </summary>
        /// <param name="node"></param>
        /// <remarks>该方法会确定是否可以完成操作再进行操作，不会破坏节点与树的状态</remarks>
        public void DetachNode(TTreeNode node)
        {
            if (node == Root)
                throw new TreeNodeException<TTreeNode, TKey>("不能分离本树的根节点！", node);

            if (node.Tree != this)
                throw new TreeNodeException<TTreeNode, TKey>("无法分离不属于本树的节点！", node);

            //是本树的节点，在管控范围之内，可以安全地去附加所有节点

            //第一步，从树中移除该节点并删除与父节点的关联
            ((List<TTreeNode>)node.Parent.Children).Remove(node);
            node.Parent = null;
            //第二步，移除当前节点与其所有子节点在当前树中的缓存以及与当前树的关联
            foreach (var n in node.AllToList())
            {
                n.Tree = null;
                _flattenNodes.Remove(n.Id);
            }
        }

        /// <summary>
        /// 将节点移动到指定的父节点之下
        /// </summary>
        /// <param name="node"></param>
        /// <param name="toParentNode"></param>
        /// <remarks>该方法会确定是否可以完成操作再进行操作，不会破坏节点与树的状态</remarks>
        public void MoveNode(TTreeNode node, TTreeNode toParentNode)
        {
            MoveNode(node, toParentNode, -1);
        }

        /// <summary>
        /// 将节点移动到指定的父节点之下的指定索引位置
        /// </summary>
        /// <param name="node"></param>
        /// <param name="toParentNode"></param>
        /// <param name="index">如果指定的索引不存在，将把节点安放在尾部</param>
        /// <remarks>该方法会确定是否可以完成操作再进行操作，不会破坏节点与树的状态</remarks>
        public void MoveNode(TTreeNode node, TTreeNode toParentNode, int index)
        {
            if (node == Root)
                throw new TreeNodeException<TTreeNode, TKey>("不能移动本树的根节点！", node);

            if (node.Tree != this || toParentNode.Tree != this)
                throw new TreeNodeException<TTreeNode, TKey>("只能将节点在本树中移动，不能跨树移动，您可以先去附加然后再尝试跨树移动！", node);

            //已经确定节点均在本树中，属于管理范围内，无需特殊检查
            //第一步，从父节点移除
            ((List<TTreeNode>)node.Parent.Children).Remove(node);
            //第二步，挂载到指定节点指定位置之下
            if (index > -1 && index < ((List<TTreeNode>)toParentNode.Children).Count) //在范围之间
                ((List<TTreeNode>)toParentNode.Children).Insert(index, node);
            else
                ((List<TTreeNode>)toParentNode.Children).Add(node);
            //第三步，更新父节点引用
            node.Tree = null;
            node.Parent = null;
            node.ParentId = toParentNode.Id;
            node.Parent = toParentNode;
            node.Tree = this;
            //由于是移动节点，故无需操作树节点缓存，整个操作中没有节点Id被改变。
        }

        /// <summary>
        /// 使用稳定排序，排序树中所有节点
        /// </summary>
        /// <param name="comparison"></param>
        public void StableSort(Comparison<TTreeNode> comparison)
        {
            Root?.StableSort(comparison);
        }

        #endregion

        #region 线性化
        /// <summary>
        /// 所有节点线性化
        /// </summary>
        /// <returns></returns>
        public IEnumerable<TTreeNode> AllAsEnumerable()
        {
            return _flattenNodes.Values;
        }

        /// <summary>
        /// 所有子节点线性化
        /// </summary>
        /// <returns></returns>
        public IEnumerable<TTreeNode> SubAsEnumerable()
        {
            return _flattenNodes.Values.Where(node => node != Root);
        }
        #endregion
    }
}
