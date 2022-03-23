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
                    new MyMenu(){Id=1,ParentId=0,Name="根"},
                    new MyMenu(){Id=2,ParentId=1,Name="一级菜单"},
                    new MyMenu(){Id=3,ParentId=1,Name="二号一级菜单"},
                    new MyMenu(){Id=4,ParentId=2,Name="二级菜单"},
                    new MyMenu(){Id=5,ParentId=2,Name="二号二级菜单" },
            });
        }



        [Fact]
        public void Constructor_NormalInput_TreeNode()
        {
            var tree = getTree();

            Assert.NotNull(tree);

            Assert.True(tree.Nodes.Count > 0);

            Assert.True(tree.Nodes[0].Name == "根");

            Assert.True(tree.Nodes[0].Children[0].Parent == tree.Nodes[0]);
        }

        [Fact]
        public void Contains_Id_Boolean()
        {
            var tree = getTree();
            Assert.True(tree.Contains(3));
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

            var findNode = tree.Find(4);
            Assert.NotNull(findNode);
            Assert.True(findNode.Name == "二级菜单" && tree.Find(findNode.ParentId) == findNode.Parent);
        }

        [Fact]
        public void Find_Predicate_MyMenu()
        {
            var tree = getTree();

            var findNode = tree.Find(node => node.Id == 4);
            Assert.NotNull(findNode);
            Assert.True(findNode.Name == "二级菜单" && tree.Find(findNode.ParentId) == findNode.Parent);
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

            var node = new MyMenu() { Id = 5, ParentId = 5 };
            Assert.Throws<TreeNodeException<MyMenu, int>>(() => { tree.AttachNode(node); });

            node.Id = 6;
            //这里测试配置正确的下级节点能否在树中生存良好（在程序集以外理论上不能创建这样的节点，必须经过算法整理才能创建层级节点）
            var subNode = new MyMenu { Id = 7, ParentId = 6, Name = "待测试", Parent = node };

            ((List<MyMenu>)node.Children).Add(subNode);

            tree.AttachNode(node);

            Assert.NotNull(node.Parent);
            Assert.True(node.Parent == tree.Find(node.ParentId));
            Assert.True(subNode.Parent.Children[0] == subNode);
            Assert.NotNull(tree.Find(node.Id));

            //尝试重复添加
            Assert.Throws<TreeNodeException<MyMenu, int>>(() => { tree.AttachNode(node); });

            //尝试错误添加，id重复
            Assert.Throws<TreeNodeException<MyMenu, int>>(() => { tree.AttachNode(new MyMenu() { Id = 4, ParentId = 3, Name = "id重复" }); });

            //尝试错误添加，父节点不存在重复
            tree.AttachNode(new MyMenu() { Id = 9, ParentId = 8, Name = "不存在父节点" });

            //尝试错误添加，混合以上两点
            Assert.Throws<TreeNodeException<MyMenu, int>>(() => { tree.AttachNode(new MyMenu() { Id = 4, ParentId = 8, Name = "id重复的不存在父节点" }); });

            //附加另一颗树的节点
            var tree2 = getTree();
            Assert.Throws<TreeNodeException<MyMenu, int>>(() =>
            {
                tree.AttachNode(tree2.Nodes.First());
            });
        }

        [Fact]
        public void AttachNode_NodeIndex()
        {
            var tree = getTree();

            var node = new MyMenu() { Id = 6, ParentId = 2 };

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


            //分离其它树节点
            var tree2 = getTree();
            Assert.Throws<TreeNodeException<MyMenu, int>>(() => tree.DetachNode(tree2.Find(4)));

            //分离节点并测试
            var node = tree.Find(2);
            var oldParent = node.Parent;

            //分离前的节点ParentId应该不可以赋值
            Assert.Throws<TreeNodeException<MyMenu, int>>(() => node.ParentId = 6);

            tree.DetachNode(node);

            //分离后的节点Parent应为null,当前节点及子节点Tree应为null
            Assert.Null(node.Parent);

            foreach (var n in node.AllToList())
                Assert.Null(node.Tree);

            var oldParentId = node.ParentId;
            node.ParentId = 6; //分离后的节点ParentId应该可以赋值
            node.ParentId = oldParentId;

            //节点应该可以附加回去
            tree.AttachNode(node);

            //附加后的节点Parent应不为null,当前节点及子节点Tree应不为null
            Assert.NotNull(node.Parent);

            foreach (var n in node.AllToList())
                Assert.NotNull(node.Tree);

            //附加后应该可以找回
            Assert.NotNull(tree.Find(2));

            //附加后的父节点应该与附加前一致
            Assert.True(node.Parent == oldParent);
        }

        [Fact]
        public void MoveNode_Node()
        {
            var tree = getTree();

            //移动根节点
            Assert.Throws<TreeNodeException<MyMenu, int>>(() => tree.MoveNode(tree.Nodes[0], tree.Nodes[0].Children[0]));

            //移动其它树节点
            var tree2 = getTree();
            Assert.Throws<TreeNodeException<MyMenu, int>>(() => tree.MoveNode(tree2.Find(4), tree.Nodes[0]));
            Assert.Throws<TreeNodeException<MyMenu, int>>(() => tree.MoveNode(tree.Find(4), tree2.Nodes[0]));
            Assert.Throws<TreeNodeException<MyMenu, int>>(() => tree.MoveNode(tree2.Find(4), tree2.Nodes[0]));

            //移动节点并测试
            var node = tree.Find(4);

            //移动前的节点ParentId应该不可以赋值
            Assert.Throws<TreeNodeException<MyMenu, int>>(() => node.ParentId = 6);

            tree.MoveNode(node, tree.Find(3));

            //移动后的节点ParentId应该不可以赋值
            Assert.Throws<TreeNodeException<MyMenu, int>>(() => node.ParentId = 6);

            //移动后的节点应该可以找回
            Assert.NotNull(tree.Find(4));

            //移动后的节点及其子节点Tree不能为null
            foreach (var n in node.AllToList())
                Assert.NotNull(node.Tree);

            //移动后的父节点应该与移动到目标节点一致
            Assert.True(node.Parent == tree.Find(3));

            //原父节点下应该没有当前节点
            Assert.False(tree.Find(2).Contains(4));

            //现在的父节点下应该有当前节点
            Assert.True(tree.Find(3).Contains(4));

            tree.MoveNode(tree.Find(4), tree.Find(2), 1);

            Assert.Equal(1, tree.Find(4).Index);

            tree.AttachNode(new MyMenu() { Id = 6, ParentId = 2, Name = "第六个元素" }, 1);

            Assert.Equal(1, tree.Find(6).Index);

            tree.AttachNode(new MyMenu() { Id = 7, ParentId = 3, Name = "第七个元素" });

            tree.MoveNode(tree.Find(7), tree.Find(2), 2);

            Assert.Equal(2, tree.Find(7).Index);

            tree.MoveNode(tree.Find(5), tree.Find(2), 2);

            Assert.Equal(1, tree.Find(5).Index);
        }

        [Fact]
        public void MoveNode_NodeIndex()
        {
            var tree = getTree();
            var node = tree.Find(3);
            //把2添加到1下面的序号2 上
            tree.MoveNode(node, tree.Find(2), 1);
            Assert.True(node.Index == 1);
            Assert.True(node.Parent == tree.Find(2));
            Assert.True(tree.Find(2).Children.Count == 3);
            Assert.True(node.ParentId == 2);
            //移动后的节点ParentId应该不可以赋值
            Assert.Throws<TreeNodeException<MyMenu, int>>(() => node.ParentId = 5);

        }

        [Fact]
        public void NodeWrapper_Distinct()
        {
            var tree = getTree();

            var wrapper = tree.CreateWrapper(new MyMenu[] { tree.Nodes[0].Children.First(), tree.Nodes[0].Children.Last() }, true);

            Assert.Equal(2, wrapper.Nodes.Count);


            wrapper = tree.CreateWrapper(new MyMenu[] {
                tree.Nodes[0].Children.First(),
                tree.Nodes[0].Children.Last(),
                tree.Nodes[0].Children.First().Children.First(),
                tree.Nodes[0].Children.First().Children.Last(),
                tree.Nodes[0]
            }, true);
            Assert.Equal(1, wrapper.Nodes.Count);

        }

        [Fact]
        public void AncestorToList()
        {
            var tree = getTree();

            var ancestors = tree.Nodes[0].Children.First().Children.First().AncestorToList();

            Assert.True(ancestors[0] == tree.Nodes[0].Children.First() && ancestors[1] == tree.Nodes[0]);

            ancestors = tree.Nodes[0].Children.First().Children.First().AncestorToList(true);
            Assert.True(ancestors[0] == tree.Nodes[0].Children.First().Children.First() && ancestors[1] == tree.Nodes[0].Children.First() && ancestors[2] == tree.Nodes[0]);
        }

        [Fact]
        public void FindAncestor()
        {
            var tree = getTree();
            var last = tree.Nodes[0].Children.First().Children.First();
            Assert.Equal(last.FindAncestor(2), tree.Nodes[0].Children[0]);

            Assert.Null(last.FindAncestor(4));

            Assert.NotNull(last.FindAncestor(4, true));

            Assert.Equal(4, last.FindAncestor(menu => menu.Name.Contains("菜单"), true).Id);
        }

        [Fact]
        public void FindDescendant()
        {
            var tree = getTree();
            Assert.Null(tree.Nodes[0].FindDescendant(1));
            Assert.NotNull(tree.Nodes[0].Find(1));
        }

        [Fact]
        public void NodeWrapperTree_ContainsFind()
        {
            var tree = getTree();

            var wrapper = tree.CreateWrapper(new MyMenu[] { tree.Nodes[0].Children.First(), tree.Nodes[0].Children.Last() }, true);

            Assert.True(wrapper.ContainsDescendant(4));

            Assert.NotNull(wrapper.FindDescendant(4));

            Assert.False(tree.ContainsDescendant(1));

            Assert.True(tree.ContainsDescendant(3));


            Assert.Null(tree.FindDescendant(1));

            Assert.NotNull(tree.FindDescendant(4));

        }

    }
}
