using System;
using Xunit;
using Moq;
using ZDevTools.Collections;
using System.Linq;
using System.Collections.Generic;


namespace ZDevTools.Test.Collections
{

    public class UnitTest_Tree
    {
        Tree<MyMenu, int> getTree()
        {
            return new Tree<MyMenu, int>(new MyMenu[] {
                    new MyMenu(){ Id=0,ParentId=0,Name="根"},
                    new MyMenu(){Id=1,ParentId=0,Name="一级菜单"},
                    new MyMenu(){Id=2,ParentId=0,Name="二号一级菜单"},
                    new MyMenu(){Id=3,ParentId=1,Name="二级菜单"},
                    new MyMenu(){Id=4,ParentId=1,Name="二号二级菜单" },
            });
        }



        [Fact]
        public void Constructor_NormalInput_TreeNode()
        {
            var tree = getTree();

            Assert.NotNull(tree);

            Assert.NotNull(tree.Root);

            Assert.True(tree.Root.Children.Count > 0);

            Assert.True(tree.Root.Name == "根");

            Assert.True(tree.Root.Children[0].Parent == tree.Root);
        }

        [Fact]
        public void Contains_Id_Boolean()
        {
            var tree = getTree();
            Assert.True(tree.Contains(2));
        }

        [Fact]
        public void Contains_Predicate_Boolean()
        {
            var tree = getTree();
            Assert.True(tree.Contains(mm => mm.Name == "一级菜单"));
        }

        [Fact]
        public void Find_Id_MyMenu()
        {
            var tree = getTree();

            var findNode = tree.Find(3);
            Assert.NotNull(findNode);
            Assert.True(findNode.Name == "二级菜单" && tree.Find(findNode.ParentId) == findNode.Parent);
        }

        [Fact]
        public void Find_Predicate_MyMenu()
        {
            var tree = getTree();

            var findNode = tree.Find(node => node.Id == 3);
            Assert.NotNull(findNode);
            Assert.True(findNode.Name == "二级菜单" && tree.Find(findNode.ParentId) == findNode.Parent);
        }

        [Fact]
        public void FindAll_IntegerArray_MyMenus()
        {
            var tree = getTree();

            var nodes = tree.FindAll(new int[] { 0, 1, 3 }).ToArray();

            Assert.True(nodes.Length == 3);
            Assert.True(nodes[0].Name == "根" && nodes[1].Name == "一级菜单");
        }

        [Fact]
        public void FindAll_Predicate_MyMenus()
        {
            var tree = getTree();

            var nodes = tree.FindAll(mm => mm.Name.Contains("菜单"));

            Assert.True(nodes.Count() == 4);
        }

        [Fact]
        public void AttachNode_Node()
        {
            var tree = getTree();

            var node = new MyMenu() { Id = 4, ParentId = 4 };
            Assert.Throws<TreeNodeException<MyMenu, int>>(() => { tree.AttachNode(node); });

            node.Id = 5;
            //这里测试配置正确的下级节点能否在树中生存良好（在程序集以外理论上不能创建这样的节点，必须经过算法整理才能创建层级节点）
            var subNode = new MyMenu { Id = 6, ParentId = 5, Name = "待测试", Parent = node };

            ((List<MyMenu>)node.Children).Add(subNode);

            tree.AttachNode(node);

            Assert.NotNull(node.Parent);
            Assert.True(node.Parent == tree.Find(node.ParentId));
            Assert.True(subNode.Parent.Children[0] == subNode);
            Assert.NotNull(tree.Find(node.Id));

            //尝试重复添加
            Assert.Throws<TreeNodeException<MyMenu, int>>(() => { tree.AttachNode(node); });

            //尝试错误添加，id重复
            Assert.Throws<TreeNodeException<MyMenu, int>>(() => { tree.AttachNode(new MyMenu() { Id = 3, ParentId = 2, Name = "id重复" }); });

            //尝试错误添加，父节点不存在重复
            Assert.Throws<TreeNodeException<MyMenu, int>>(() => { tree.AttachNode(new MyMenu() { Id = 8, ParentId = 7, Name = "不存在父节点" }); });

            //尝试错误添加，混合以上两点
            Assert.Throws<TreeNodeException<MyMenu, int>>(() => { tree.AttachNode(new MyMenu() { Id = 3, ParentId = 7, Name = "id重复的不存在父节点" }); });

            //附加另一颗树的节点
            var tree2 = getTree();
            Assert.Throws<TreeNodeException<MyMenu, int>>(() =>
            {
                tree.AttachNode(tree2.Root);
            });


        }

        [Fact]
        public void AttachNode_NodeIndex()
        {
            var tree = getTree();

            var node = new MyMenu() { Id = 5, ParentId = 1 };

            const int index = 1;
            tree.AttachNode(node, index);

            Assert.True(node.Index == index);

            Assert.True(node.Parent == tree.Find(node.ParentId));

            Assert.True(node.Parent.Children[index] == node);
        }

        [Fact]
        public void DetachNode_Node()
        {
            var tree = getTree();

            //分离根节点
            Assert.Throws<TreeNodeException<MyMenu, int>>(() => tree.DetachNode(tree.Root));

            //分离其它树节点
            var tree2 = getTree();
            Assert.Throws<TreeNodeException<MyMenu, int>>(() => tree.DetachNode(tree2.Find(3)));

            //分离节点并测试
            var node = tree.Find(1);
            var oldParent = node.Parent;

            //分离前的节点ParentId应该不可以赋值
            Assert.Throws<TreeNodeException<MyMenu, int>>(() => node.ParentId = 5);

            tree.DetachNode(node);

            //分离后的节点Parent应为null,当前节点及子节点Tree应为null
            Assert.Null(node.Parent);

            foreach (var n in node.AllToList())
                Assert.Null(node.Tree);

            var oldParentId = node.ParentId;
            node.ParentId = 5; //分离后的节点ParentId应该可以赋值
            node.ParentId = oldParentId;

            //节点应该可以附加回去
            tree.AttachNode(node);

            //附加后的节点Parent应不为null,当前节点及子节点Tree应不为null
            Assert.NotNull(node.Parent);

            foreach (var n in node.AllToList())
                Assert.NotNull(node.Tree);

            //附加后应该可以找回
            Assert.NotNull(tree.Find(1));
            
            //附加后的父节点应该与附加前一致
            Assert.True(node.Parent == oldParent);
        }

        [Fact]
        public void MoveNode_Node()
        {
            var tree = getTree();

            //移动根节点
            Assert.Throws<TreeNodeException<MyMenu, int>>(() => tree.MoveNode(tree.Root, tree.Root));

            //移动其它树节点
            var tree2 = getTree();
            Assert.Throws<TreeNodeException<MyMenu, int>>(() => tree.MoveNode(tree2.Find(3), tree.Root));
            Assert.Throws<TreeNodeException<MyMenu, int>>(() => tree.MoveNode(tree.Find(3), tree2.Root));
            Assert.Throws<TreeNodeException<MyMenu, int>>(() => tree.MoveNode(tree2.Find(3), tree2.Root));

            //移动节点并测试
            var node = tree.Find(3);

            //移动前的节点ParentId应该不可以赋值
            Assert.Throws<TreeNodeException<MyMenu, int>>(() => node.ParentId = 5);

            tree.MoveNode(node, tree.Find(2));

            //移动后的节点ParentId应该不可以赋值
            Assert.Throws<TreeNodeException<MyMenu, int>>(() => node.ParentId = 5);

            //移动后的节点应该可以找回
            Assert.NotNull(tree.Find(3));

            //移动后的节点及其子节点Tree不能为null
            foreach (var n in node.AllToList())
                Assert.NotNull(node.Tree);

            //移动后的父节点应该与移动到目标节点一致
            Assert.True(node.Parent == tree.Find(2));

            //原父节点下应该没有当前节点
            Assert.False(tree.Find(1).Contains(3));

            //现在的父节点下应该有当前节点
            Assert.True(tree.Find(2).Contains(3));
        }

        [Fact]
        public void MoveNode_NodeIndex()
        {
            var tree = getTree();
            var node = tree.Find(2);
            //把2添加到1下面的序号2 上
            tree.MoveNode(node, tree.Find(1), 1);
            Assert.True(node.Index == 1);
            Assert.True(node.Parent == tree.Find(1));
            Assert.True(tree.Find(1).Children.Count == 3);
            Assert.True(node.ParentId == 1);
            //移动后的节点ParentId应该不可以赋值
            Assert.Throws<TreeNodeException<MyMenu, int>>(() => node.ParentId = 5);

        }
    }
}
