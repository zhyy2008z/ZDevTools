using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZDevTools.Collections
{
    /// <summary>
    /// TreeNode结构异常类，用于结构算法出现无法处理情况时抛出
    /// </summary>
    /// <typeparam name="TTreeNode">TreeNode类型</typeparam>
    /// <typeparam name="TKey">键类型</typeparam>
    [Serializable]
    public class TreeNodeException<TTreeNode, TKey> : Exception
        where TTreeNode : TreeNode<TTreeNode, TKey>
    {
        /// <summary>
        /// 树节点
        /// </summary>
        public TTreeNode Node { get; }

        /// <summary>
        /// 初始化一个异常类
        /// </summary>
        public TreeNodeException() { }
        /// <summary>
        /// 初始化一个异常类
        /// </summary>
        public TreeNodeException(string message) : base(message) { }
        /// <summary>
        /// 初始化一个异常类
        /// </summary>
        public TreeNodeException(string message, TTreeNode node) : base(message + "节点Id:" + node.Id) { Node = node; }
        /// <summary>
        /// 初始化一个异常类
        /// </summary>
        public TreeNodeException(string message, Exception inner) : base(message, inner) { }
        /// <summary>
        /// 初始化一个异常类
        /// </summary>
        public TreeNodeException(string message, Exception inner, TTreeNode node) : base(message + "节点Id:" + node.Id, inner) { Node = node; }
        /// <summary>
        /// 初始化一个异常类
        /// </summary>
        protected TreeNodeException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
