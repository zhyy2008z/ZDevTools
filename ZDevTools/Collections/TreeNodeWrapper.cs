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
    {
        bool _longterm;
        List<TTreeNode> _nodes;
        Dictionary<TKey, TTreeNode> _flattenNodes;

        /// <summary>
        /// 根据提供的节点创建节点临时包装
        /// </summary>
        /// <param name="nodes"></param>
        public TreeNodeWrapper(IEnumerable<TTreeNode> nodes) : this(nodes, false) { }

        /// <summary>
        /// 根据提供的节点创建节点包装
        /// </summary>
        /// <param name="nodes"></param>
        /// <param name="longterm">是否长期包装，如果是长期包装则会采取以空间换时间的算法</param>
        public TreeNodeWrapper(IEnumerable<TTreeNode> nodes, bool longterm)
        {
            _longterm = longterm;

            if (longterm)
            {
                _flattenNodes = new Dictionary<TKey, TTreeNode>();
                _nodes = new List<TTreeNode>();
                foreach (var node in nodes)
                {
                    _nodes.Add(node);
                    foreach (var n in node.AllToList())
                        _flattenNodes.Add(n.Id, n);
                }
            }
            else
            {
                _nodes = nodes.ToList();
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
                return _nodes.SelectMany(node => node.AllToList());
            }
        }
        #endregion

        #region 判断
        /// <summary>
        /// 包装中是否包含指定Id的节点
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
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
                return _nodes.SelectMany(node => node.FindAll(predicate));
            }
        }
        #endregion
    }
}
