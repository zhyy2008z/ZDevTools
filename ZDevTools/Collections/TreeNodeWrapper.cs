using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ZDevTools.Collections
{
    /// <summary>
    /// 将一组节点临时组合为一个团体，支持在这个团体中查找指定节点
    /// </summary>
    /// <typeparam name="TTreeNode"></typeparam>
    /// <typeparam name="TKey"></typeparam>
    public class TreeNodeWrapper<TTreeNode, TKey>
        where TTreeNode : TreeNode<TTreeNode, TKey>
        where TKey : IEquatable<TKey>
    {
        readonly bool Longterm;
        readonly List<TTreeNode> InnerNodes;
        readonly Dictionary<TKey, TTreeNode> FlattenNodes;

        /// <summary>
        /// 根据提供的节点创建节点临时包装(要求所有节点必须来自同一颗树)
        /// </summary>
        /// <param name="nodes">来自同一颗树的节点</param>
        public TreeNodeWrapper(IEnumerable<TTreeNode> nodes) : this(nodes, false, false) { }

        /// <summary>
        /// 根据提供的节点创建节点临时包装(要求所有节点必须来自同一颗树)
        /// </summary>
        /// <param name="nodes">来自同一颗树的节点</param>
        /// <param name="distinct">是否需要去重</param>
        public TreeNodeWrapper(IEnumerable<TTreeNode> nodes, bool distinct) : this(nodes, distinct, false) { }

        /// <summary>
        /// 根据提供的节点创建节点包装(要求所有节点必须来自同一颗树)
        /// </summary>
        /// <param name="nodes">来自同一颗树的节点</param>
        /// <param name="distinct">是否需要去重</param>
        /// <param name="longterm">是否长期包装，如果是长期包装则会采取以空间换时间的算法</param>
        public TreeNodeWrapper(IEnumerable<TTreeNode> nodes, bool distinct, bool longterm)
        {
            Longterm = longterm;
            InnerNodes = nodes.ToList();

            if (distinct)
            {
                //合并节点
                for (int i = InnerNodes.Count - 1; i > 0; i--)
                {
                    var thisNode = InnerNodes[i];
                    for (int j = i - 1; j > -1; j--)
                    {
                        var thatNode = InnerNodes[j];
                        if (thatNode.Find(thisNode.Id) != null) //当前节点在其它节点中
                        {
                            InnerNodes.RemoveAt(i);
                            break;
                        }
                        else if (thisNode.Find(thatNode.Id) != null) //其它节点在当前节点中
                        {
                            InnerNodes.RemoveAt(j);
                            i--;
                        }
                    }
                }
            }

            if (longterm)
            {
                FlattenNodes = new Dictionary<TKey, TTreeNode>();
                foreach (var node in InnerNodes)
                {
                    foreach (var n in node.AllToList())
                        FlattenNodes[n.Id] = n;
                }
            }
        }

        /// <summary>
        /// 被包装的节点
        /// </summary>
        public IReadOnlyList<TTreeNode> Nodes => InnerNodes;

        #region 线性化
        /// <summary>
        /// 枚举包装中的所有节点（也包含后代节点）
        /// </summary>
        /// <returns></returns>
        public IEnumerable<TTreeNode> AllAsEnumerable()
        {
            if (Longterm)
                return FlattenNodes.Values;
            else
                return InnerNodes.SelectMany(node => node.AllToList());
        }

        /// <summary>
        /// 枚举包装中的后代节点（不包括直接被包装的节点，仅包含它们的后代节点）
        /// </summary>
        /// <returns></returns>
        public IEnumerable<TTreeNode> SubAsEnumerable()
        {
            if (Longterm)
                return FlattenNodes.Values.Where(node => !Nodes.Contains(node));
            else
                return InnerNodes.SelectMany(node => node.SubToList());
        }
        #endregion

        #region 判断
        /// <summary>
        /// 包装中是否包含指定Id的节点
        /// </summary>
        public bool Contains(TKey id)
        {
            if (Longterm)
            {
                return FlattenNodes.ContainsKey(id);
            }
            else
            {
                foreach (var node in InnerNodes)
                {
                    if (node.Contains(id))
                        return true;
                }
                return false;
            }
        }

        /// <summary>
        /// 包装中是否包含指定的节点
        /// </summary>
        public bool Contains(TTreeNode node) => Contains(node.Id);

        /// <summary>
        /// 包装中是否包含能够通过断言的节点
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public bool Contains(Func<TTreeNode, bool> predicate)
        {
            if (Longterm)
            {
                return FlattenNodes.Values.Any(predicate);
            }
            else
            {
                foreach (var node in InnerNodes)
                {
                    if (node.Contains(predicate))
                        return true;
                }
                return false;
            }
        }
        #endregion

        #region 判断后代
        /// <summary>
        /// 包装中是否包含指定Id的后代节点
        /// </summary>
        public bool ContainsDescendant(TKey id)
        {
            if (Longterm)
            {
                return !InnerNodes.Any(node => node.Id.Equals(id)) && FlattenNodes.ContainsKey(id);
            }
            else
            {
                foreach (var node in InnerNodes)
                {
                    if (node.ContainsDescendant(id))
                        return true;
                }
                return false;
            }
        }

        /// <summary>
        /// 包装中是否包含指定的后代节点
        /// </summary>
        public bool ContainsDescendant(TTreeNode node) => ContainsDescendant(node.Id);


        /// <summary>
        /// 包装中是否包含能够通过断言的后代节点
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public bool ContainsDescendant(Func<TTreeNode, bool> predicate)
        {
            if (Longterm)
            {
                return FlattenNodes.Values.Any(node => !InnerNodes.Any(n => n.Id.Equals(node.Id)) && predicate(node));
            }
            else
            {
                foreach (var node in InnerNodes)
                {
                    if (node.ContainsDescendant(predicate))
                        return true;
                }
                return false;
            }
        }
        #endregion

        #region 查找
        /// <summary>
        /// 查找包装中具有指定Id的节点
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public TTreeNode Find(TKey id)
        {
            if (this.Longterm)
            {
                FlattenNodes.TryGetValue(id, out var result);
                return result;
            }
            else
            {
                foreach (var node in InnerNodes)
                {
                    var result = node.Find(id);
                    if (result != null) return result;
                }
                return null;
            }
        }

        /// <summary>
        /// 查找包装中符合断言的第一个节点
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public TTreeNode Find(Func<TTreeNode, bool> predicate)
        {
            if (Longterm)
            {
                return FlattenNodes.Values.FirstOrDefault(predicate);
            }
            else
            {
                foreach (var node in InnerNodes)
                {
                    var result = node.Find(predicate);
                    if (result != null) return result;
                }
                return null;
            }
        }

        /// <summary>
        /// 查找包装中符合断言的所有节点
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public List<TTreeNode> FindAll(Func<TTreeNode, bool> predicate)
        {
            if (this.Longterm)
            {
                return FlattenNodes.Values.Where(predicate).ToList();
            }
            else
            {
                var result = new List<TTreeNode>();
                foreach (var node in InnerNodes)
                    TreeNode<TTreeNode>.Linear(node, result, predicate);
                return result;
            }
        }
        #endregion

        #region 查找后代
        /// <summary>
        /// 查找包装中具有指定Id的后代节点
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public TTreeNode FindDescendant(TKey id)
        {
            if (this.Longterm)
            {
                if (!InnerNodes.Any(node => node.Id.Equals(id)) && FlattenNodes.TryGetValue(id, out var result))
                    return result;
                else
                    return null;
            }
            else
            {
                foreach (var node in InnerNodes)
                {
                    var result = node.FindDescendant(id);
                    if (result != null) return result;
                }
                return null;
            }
        }

        /// <summary>
        /// 查找包装中符合断言的第一个后代节点
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public TTreeNode FindDescendant(Func<TTreeNode, bool> predicate)
        {
            if (Longterm)
            {
                return FlattenNodes.Values.FirstOrDefault(node => !InnerNodes.Any(n => n.Id.Equals(node.Id)) && predicate(node));
            }
            else
            {
                foreach (var node in InnerNodes)
                {
                    var result = node.FindDescendant(predicate);
                    if (result != null) return result;
                }
                return null;
            }
        }

        /// <summary>
        /// 查找包装中符合断言的所有后代节点
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public List<TTreeNode> FindAllDescendants(Func<TTreeNode, bool> predicate)
        {
            if (this.Longterm)
            {
                return FlattenNodes.Values.Where(node => !InnerNodes.Any(n => n.Id.Equals(node.Id)) && predicate(node)).ToList();
            }
            else
            {
                var result = new List<TTreeNode>();
                foreach (var node in InnerNodes)
                    foreach (var child in node.Children)
                        TreeNode<TTreeNode>.Linear(child, result, predicate);
                return result;
            }
        }
        #endregion
    }
}
