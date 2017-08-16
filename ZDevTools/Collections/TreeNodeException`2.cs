using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZDevTools.Collections
{
    [Serializable]
    public class TreeNodeException<TTreeNode, TKey> : Exception
        where TTreeNode : TreeNode<TTreeNode, TKey>
    {

        public TTreeNode Node { get; }

        public TreeNodeException() { }

        public TreeNodeException(string message) : base(message) { }

        public TreeNodeException(string message, TTreeNode node) : base(message + "节点Id:" + node.Id) { Node = node; }

        public TreeNodeException(string message, Exception inner) : base(message, inner) { }

        public TreeNodeException(string message, Exception inner, TTreeNode node) : base(message + "节点Id:" + node.Id, inner) { Node = node; }

        protected TreeNodeException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
