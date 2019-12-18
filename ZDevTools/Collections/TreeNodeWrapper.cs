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
        bool _longterm;
        List<TTreeNode> _nodes;
        Dictionary<TKey, TTreeNode> _flattenNodes;

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
            _longterm = longterm;
            _nodes = nodes.ToList();

            if (distinct)
            {
                //合并节点
                for (int i = _nodes.Count - 1; i > 0; i--)
                {
                    var thisNode = _nodes[i];
                    for (int j = i - 1; j > -1; j--)
                    {
                        var thatNode = _nodes[j];
                        if (thatNode.Find(thisNode.Id) != null) //当前节点在其它节点中
                        {
                            _nodes.RemoveAt(i);
                            break;
                        }
                        else if (thisNode.Find(thatNode.Id) != null) //其它节点在当前节点中
                        {
                            _nodes.RemoveAt(j);
                            i--;
                        }
                    }
                }
            }

            if (longterm)
            {
                _flattenNodes = new Dictionary<TKey, TTreeNode>();
                foreach (var node in _nodes)
                {
                    foreach (var n in node.AllToList())
                        _flattenNodes[n.Id] = n;
                }
            }
        }

        /// <summary>
        /// 被包装的节点
        /// </summary>
        public IReadOnlyList<TTreeNode> Nodes { get { return _nodes; } }

        #region 线性化
        /// <summary>
        /// 枚举包装中的所有节点（包括子节点）
        /// </summary>
        /// <returns></returns>
        public IEnumerable<TTreeNode> AsEnumerable()
        {
            if (_longterm)
                return _flattenNodes.Values;
            else
            {
                return _nodes.SelectMany(node => node.AllToList()).Distinct();
            }
        }
        #endregion

        #region 判断
        /// <summary>
        /// 包装中是否包含指定Id的节点
        /// </summary>
        public bool Contains(TKey id)
        {
            if (_longterm)
            {
                return _flattenNodes.ContainsKey(id);
            }
            else
            {
                foreach (var node in _nodes)
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
        public bool Contains(TTreeNode node)
        {
            if (_longterm)
            {
                return _flattenNodes.ContainsKey(node.Id);
            }
            else
            {
                foreach (var n in _nodes)
                {
                    if (n.Contains(node))
                        return true;
                }
                return false;
            }
        }

        /// <summary>
        /// 包装中是否包含能够通过断言的节点
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public bool Contains(Func<TTreeNode, bool> predicate)
        {
            if (_longterm)
            {
                return _flattenNodes.Values.Any(predicate);
            }
            else
            {
                foreach (var node in _nodes)
                {
                    if (node.Contains(predicate))
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
            if (this._longterm)
            {
                _flattenNodes.TryGetValue(id, out var result);
                return result;
            }
            else
            {
                return _nodes.Select(node => node.Find(id)).FirstOrDefault();
            }
        }

        /// <summary>
        /// 查找包装中符合断言的第一个节点
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public TTreeNode Find(Func<TTreeNode, bool> predicate)
        {
            if (_longterm)
            {
                return _flattenNodes.Values.FirstOrDefault(predicate);
            }
            else
            {
                return _nodes.Select(node => node.Find(predicate)).FirstOrDefault();
            }
        }

        /// <summary>
        /// 查找包装中符合断言的所有节点
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public IEnumerable<TTreeNode> FindAll(Func<TTreeNode, bool> predicate)
        {
            if (this._longterm)
            {
                return _flattenNodes.Values.Where(predicate);
            }
            else
            {
                return _nodes.SelectMany(node => node.FindAll(predicate)).Distinct();
            }
        }
        #endregion
    }
}
