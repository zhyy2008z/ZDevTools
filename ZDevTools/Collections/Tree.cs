using System;
using System.Collections.Generic;
using System.Linq;

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
    {
        readonly Dictionary<TKey, TTreeNode> FlattenNodes;

        /// <summary>
        /// 初始化一颗树
        /// </summary>
        /// <param name="nodes">一组节点</param>
        public Tree(IEnumerable<TTreeNode> nodes) : this(nodes, null) { }

        /// <summary>
        /// 初始化一颗树
        /// </summary>
        /// <param name="nodes">一组节点</param>
        /// <param name="comparer">Key比较器</param>
        public Tree(IEnumerable<TTreeNode> nodes, IEqualityComparer<TKey> comparer)
        {
            this.Comparer = comparer ?? EqualityComparer<TKey>.Default;
            Nodes = parse(nodes, out FlattenNodes);
        }

        /// <summary>
        /// 获取Key比较器
        /// </summary>
        public IEqualityComparer<TKey> Comparer { get; }

        /// <summary>
        /// 所有第一级节点
        /// </summary>
        public IReadOnlyList<TTreeNode> Nodes { get; }

        #region 解析
        /// <summary>
        /// 该方法用于解析节点列表，解析后组织为节点树结构，并获得唯一根节点
        /// </summary>
        /// <param name="nodes">treeNode可枚举对象</param>
        /// <param name="flattenNodes">平化的节点字典</param>
        List<TTreeNode> parse(IEnumerable<TTreeNode> nodes, out Dictionary<TKey, TTreeNode> flattenNodes)
        {
            var treeNodes = nodes.ToList();//每轮枚举的TreeNode可能不是同一个对象，因此要提前变为列表，把对象固定下来再用
            flattenNodes = treeNodes.ToDictionary(node => node.Id, Comparer);
            //逆转顺序后可以让nodes节点的形成层级后，同级节点的相对顺序保持不变
            treeNodes.Reverse();

            //整理为树
            for (int i = treeNodes.Count - 1; i > -1; i--)
            {
                var current = treeNodes[i];
                //检查条件，不允许已附加到树的节点参与解析
                if (current.Tree != null)
                    throw new TreeNodeException<TTreeNode, TKey>("同一个节点不能同时被多个树引用！", current);
                current.Tree = this;
                flattenNodes.TryGetValue(current.ParentId, out TTreeNode parent);
                if (parent != null && !Comparer.Equals(current.ParentId, current.Id)) //有父节点且父节点不是自己，这才能判定为非根节点，根节点的Parent属性值是null
                {
                    current.Parent = parent;
                    ((IList<TTreeNode>)parent.Children).Add(current);
                    treeNodes.Remove(current);
                }
            }

            return treeNodes;
        }
        #endregion

        #region 创建包装器

        /// <summary>
        /// 根据提供的节点创建节点不去重临时包装(要求所有节点必须来自同一颗树)
        /// </summary>
        /// <param name="nodes">来自同一颗树的节点</param>
        public TreeNodeWrapper<TTreeNode, TKey> CreateWrapper(IEnumerable<TTreeNode> nodes) => CreateWrapper(nodes, false, false);

        /// <summary>
        /// 根据提供的节点创建节点临时包装(要求所有节点必须来自同一颗树)
        /// </summary>
        /// <param name="nodes">来自同一颗树的节点</param>
        /// <param name="distinct">是否需要去重</param>
        public TreeNodeWrapper<TTreeNode, TKey> CreateWrapper(IEnumerable<TTreeNode> nodes, bool distinct) => CreateWrapper(nodes, distinct, false);

        /// <summary>
        /// 根据提供的节点创建节点包装(要求所有节点必须来自同一颗树)
        /// </summary>
        /// <param name="nodes">来自同一颗树的节点</param>
        /// <param name="distinct">是否需要去重</param>
        /// <param name="longterm">是否长期包装，如果是长期包装则会采取以空间换时间的算法</param>
        public TreeNodeWrapper<TTreeNode, TKey> CreateWrapper(IEnumerable<TTreeNode> nodes, bool distinct, bool longterm) => new TreeNodeWrapper<TTreeNode, TKey>(this, nodes, distinct, longterm);

        #endregion

        #region 判断
        /// <summary>
        /// 在树中是否存在 Id 为 id 的节点
        /// </summary>
        public bool Contains(TKey id)
        {
            return FlattenNodes.ContainsKey(id);
        }

        /// <summary>
        /// 在树中是否存在指定节点
        /// </summary>
        public bool Contains(TTreeNode node)
        {
            return FlattenNodes.ContainsKey(node.Id);
        }

        /// <summary>
        /// 在树中是否存在满足predicate的节点
        /// </summary>
        public bool Contains(Func<TTreeNode, bool> predicate)
        {
            return FlattenNodes.Values.Any(predicate);
        }
        #endregion

        #region 判断后代
        /// <summary>
        /// 除树根外是否存在 Id 为 id 的节点
        /// </summary>
        public bool ContainsDescendant(TKey id)
        {
            return FlattenNodes.TryGetValue(id, out var node) && node.Parent != null;
        }

        /// <summary>
        /// 除树根外是否存在指定节点
        /// </summary>
        public bool ContainsDescendant(TTreeNode node)
        {
            return FlattenNodes.TryGetValue(node.Id, out var n) && n.Parent != null;
        }

        /// <summary>
        /// 除树根外是否存在满足predicate的节点
        /// </summary>
        public bool ContainsDescendant(Func<TTreeNode, bool> predicate)
        {
            return FlattenNodes.Values.Any(node => node.Parent != null && predicate(node));
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
            FlattenNodes.TryGetValue(id, out var result);
            return result;
        }

        /// <summary>
        /// 查找第一个满足 <paramref name="predicate"/> 的节点
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public TTreeNode Find(Func<TTreeNode, bool> predicate)
        {
            return FlattenNodes.Values.FirstOrDefault(predicate);
        }


        /// <summary>
        /// 返回一组满足 <paramref name="predicate"/> 的节点
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public IEnumerable<TTreeNode> FindAll(Func<TTreeNode, bool> predicate)
        {
            return FlattenNodes.Values.Where(predicate);
        }

        #endregion

        #region 查找后代

        /// <summary>
        /// 从除根节点外的节点中查找 Id 为 id 的节点
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public TTreeNode FindDescendant(TKey id)
        {
            if (FlattenNodes.TryGetValue(id, out var node) && node.Parent != null)
                return node;
            else
                return null;
        }

        /// <summary>
        /// 从除根节点外的节点中查找第一个满足 <paramref name="predicate"/> 的节点
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public TTreeNode FindDescendant(Func<TTreeNode, bool> predicate)
        {
            return FlattenNodes.Values.FirstOrDefault(node => node.Parent != null && predicate(node));
        }


        /// <summary>
        /// 从除根节点外的节点中返回一组满足 <paramref name="predicate"/> 的节点
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public IEnumerable<TTreeNode> FindAllDescendants(Func<TTreeNode, bool> predicate)
        {
            return FlattenNodes.Values.Where(node => node.Parent != null && predicate(node));
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
        /// <param name="node">目标节点</param>
        /// <param name="index">添加到索引位置，-1代表最后一个元素</param>
        /// <remarks>该方法会确定是否可以完成操作再进行操作，不会破坏节点与树的状态</remarks>
        public void AttachNode(TTreeNode node, int index)
        {
            //这里的node仅考虑全新的node节点和已经整理为树结构的节点（因为只有内部算法才可以整理为树，因此我们可以假设这颗节点树是没有错误的），
            var flattenNodes = node.AllToList();
            var checkInvalidNode = flattenNodes.FirstOrDefault(n => n.Tree != null || Contains(n.Id));
            if (checkInvalidNode != null)
                throw new TreeNodeException<TTreeNode, TKey>("检查到一个节点属于其它树或者在当前树中已存在与该节点Id相同的节点！", checkInvalidNode);

            var parentNode = Find(node.ParentId);
            List<TTreeNode> nodes = (List<TTreeNode>)(parentNode == null ? Nodes : parentNode.Children);

            node.Parent = parentNode;
            node.Tree = this;

            //检查全部完成，后面的在可管控的范围内理论上不会出现异常
            foreach (var n in flattenNodes)
                FlattenNodes.Add(n.Id, n);

            if (index > -1 && index <= nodes.Count) //在可插入范围之间
                nodes.Insert(index, node);
            else
                nodes.Add(node);
        }

        /// <summary>
        /// 从树中分离一个节点及其子节点
        /// </summary>
        /// <param name="node"></param>
        /// <remarks>该方法会确定是否可以完成操作再进行操作，不会破坏节点与树的状态</remarks>
        public void DetachNode(TTreeNode node)
        {
            if (node.Tree != this)
                throw new TreeNodeException<TTreeNode, TKey>("无法分离不属于本树的节点！", node);

            //是本树的节点，在管控范围之内，可以安全地去附加所有节点

            //第一步，从树中移除该节点并删除与父节点的关联
            var nodes = (List<TTreeNode>)(node.Parent == null ? Nodes : node.Parent.Children);

            nodes.Remove(node);
            node.Parent = null;

            //第二步，移除当前节点与其所有子节点在当前树中的缓存以及与当前树的关联
            foreach (var n in node.AllToList())
            {
                n.Tree = null;
                FlattenNodes.Remove(n.Id);
            }
        }

        /// <summary>
        /// 将节点及其子节点移动到指定的父节点之下
        /// </summary>
        /// <param name="node">源节点</param>
        /// <param name="toParentNode">指定新的父级节点，若为null则为根节点</param>
        /// <remarks>该方法会确定是否可以完成操作再进行操作，不会破坏节点与树的状态</remarks>
        public void MoveNode(TTreeNode node, TTreeNode toParentNode)
        {
            MoveNode(node, toParentNode, -1);
        }

        /// <summary>
        /// 将节点及其子节点移动到指定的父节点之下的指定索引位置
        /// </summary>
        /// <param name="node">源节点</param>
        /// <param name="toParentNode">指定新的父级节点，若为null则为根节点</param>
        /// <param name="index">如果指定的索引不存在，将把节点安放在尾部</param>
        /// <remarks>该方法会确定是否可以完成操作再进行操作，不会破坏节点与树的状态</remarks>
        public void MoveNode(TTreeNode node, TTreeNode toParentNode, int index)
        {
            if (node == null)
                throw new ArgumentNullException(nameof(node));

            if (node.Tree != this || toParentNode != null && toParentNode.Tree != this)
                throw new TreeNodeException<TTreeNode, TKey>("只能将节点在本树中移动，不能跨树移动，您可以先从其他树分离，然后附加到本树后再尝试跨树移动！", node);

            if (toParentNode != null && node.Contains(toParentNode))
                throw new TreeNodeException<TTreeNode, TKey>("不能将父节点移动到其子节点下！", node);

            //已经确定节点均在本树中，属于管理范围内，无需特殊检查

            int sourceIndex;
            List<TTreeNode> nodes;

            //第一步，从父节点移除
            if (node.Parent == null) //源节点是根节点
            {
                sourceIndex = Nodes.IndexOf(node);
                nodes = (List<TTreeNode>)Nodes;
            }
            else
            {
                sourceIndex = node.Index;
                nodes = (List<TTreeNode>)node.Parent.Children;
            }

            nodes.RemoveAt(sourceIndex);

            if (node.Parent == toParentNode && index > sourceIndex) //源节点就是目标节点的直接子节点，且插入位置在原位置之后
                index--;

            nodes = (List<TTreeNode>)(toParentNode == null ? Nodes : toParentNode.Children);

            //第二步，挂载到指定节点指定位置之下
            if (index > -1 && index <= nodes.Count) //在可插入范围之间
                nodes.Insert(index, node);
            else
                nodes.Add(node);

            //第三步，更新父节点引用
            node.Tree = null;
            node.Parent = null;

            if (toParentNode == null)
                node.ParentId = default;
            else
                node.ParentId = toParentNode.Id;

            node.Parent = toParentNode;
            node.Tree = this;
            //由于是移动节点，故无需操作树节点缓存，整个操作中没有节点Id被改变。
        }
        #endregion

        #region 线性化
        /// <summary>
        /// 所有节点线性化
        /// </summary>
        /// <returns></returns>
        public IEnumerable<TTreeNode> AllAsEnumerable()
        {
            return FlattenNodes.Values;
        }

        /// <summary>
        /// 所有子节点线性化
        /// </summary>
        /// <returns></returns>
        public IEnumerable<TTreeNode> SubAsEnumerable()
        {
            return FlattenNodes.Values.Except(Nodes);
        }
        #endregion

        #region 排序
        /// <summary>
        /// 使用稳定排序，排序树中所有节点
        /// </summary>
        /// <param name="comparison"></param>
        public void StableSort(Comparison<TTreeNode> comparison) => stableSort((IList<TTreeNode>)this.Nodes, comparison);

        /// <summary>
        /// 使用稳定排序算法对列表及其子节点进行排序
        /// </summary>
        /// <param name="nodes">节点列表</param>
        /// <param name="comparison">比较算法</param>
        static void stableSort(IList<TTreeNode> nodes, Comparison<TTreeNode> comparison)
        {
            //先使用插入排序对本层级列表进行排序

            if (nodes == null)
                throw new ArgumentNullException(nameof(nodes));
            if (comparison == null)
                throw new ArgumentNullException(nameof(comparison));

            int count = nodes.Count;
            for (int j = 1; j < count; j++)
            {
                TTreeNode key = nodes[j];

                int i = j - 1;
                for (; i >= 0 && comparison(nodes[i], key) > 0; i--)
                {
                    nodes[i + 1] = nodes[i];
                }
                nodes[i + 1] = key;
            }

            foreach (var node in nodes)
                stableSort((IList<TTreeNode>)node.Children, comparison);
        }
        #endregion
    }
}
