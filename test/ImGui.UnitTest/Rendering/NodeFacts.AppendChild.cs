﻿using System;
using ImGui.Common.Primitive;
using ImGui.Rendering;
using Xunit;


namespace ImGui.UnitTest.Rendering
{
    public partial class NodeFacts
    {
        public class AppendChild
        {
            [Fact]
            public void AppendAPlainNodeToAPlainNode()
            {
                Node plainNode1 = new Node(1);
                Node plainNode2 = new Node(2);

                plainNode1.AppendChild(plainNode2);

                Assert.Same(plainNode1, plainNode2.Parent);
                Assert.Contains(plainNode1, plainNode2);
            }

            [Fact]
            public void AppendAPlainNodeToALayoutEntryNode_NotAllowed()
            {
                Action action = () =>
                {
                    Node plainNode = new Node(1);
                    Node entryNode = new Node(2);
                    entryNode.AttachLayoutEntry(new Size(100, 100));

                    entryNode.AppendChild(plainNode);
                };

                Assert.Throws<LayoutException>(action);
            }

            [Fact]
            public void AppendAPlainNodeToALayoutGroupNode_NotAllowed()
            {
                Action action = () =>
                {
                    Node plainNode = new Node(1);
                    Node groupNode = new Node(2);
                    groupNode.AttachLayoutGroup(true);

                    groupNode.AppendChild(plainNode);
                };

                Assert.Throws<LayoutException>(action);
            }


            [Fact]
            public void AppendALayoutEntryNodeToAPlainNode_NotAllowed()
            {
                Action action = () =>
                {
                    Node entryNode = new Node(1);
                    entryNode.AttachLayoutEntry(new Size(100, 100));
                    Node plainNode = new Node(2);

                    plainNode.AppendChild(entryNode);
                };

                Assert.Throws<LayoutException>(action);
            }

            [Fact]
            public void AppendALayoutEntryNodeToALayoutEntryNode_NotAllowed()
            {
                Action action = () =>
                {
                    Node entryNode1 = new Node(1);
                    entryNode1.AttachLayoutEntry(new Size(100, 100));
                    Node entryNode2 = new Node(2);
                    entryNode2.AttachLayoutEntry(new Size(100, 100));

                    entryNode1.AppendChild(entryNode2);
                };

                Assert.Throws<LayoutException>(action);
            }

            [Fact]
            public void AppendALayoutEntryNodeToALayoutGroupNode()
            {
                Node entryNode = new Node(1);
                entryNode.AttachLayoutEntry(new Size(100, 100));
                Node groupNode = new Node(2);
                groupNode.AttachLayoutGroup(true);

                groupNode.AppendChild(entryNode);

                Assert.Same(groupNode, entryNode.Parent);
                Assert.Contains(entryNode, groupNode);
            }

            [Fact]
            public void AppendALayoutGroupNodeToAPlainNode_NotAllowed()
            {
                Action action = () =>
                {
                    Node groupNode = new Node(1);
                    groupNode.AttachLayoutGroup(true);
                    Node plainNode = new Node(2);

                    plainNode.AppendChild(groupNode);
                };

                Assert.Throws<LayoutException>(action);
            }

            [Fact]
            public void AppendALayoutGroupNodeToALayoutEntryNode_NotAllowed()
            {
                Action action = () =>
                {
                    Node groupNode = new Node(1);
                    groupNode.AttachLayoutGroup(true);
                    Node entryNode = new Node(2);
                    entryNode.AttachLayoutEntry(new Size(100, 100));

                    entryNode.AppendChild(groupNode);
                };

                Assert.Throws<LayoutException>(action);
            }

            [Fact]
            public void AppendALayoutGroupNodeToALayoutGroupNode()
            {
                Node groupNode1 = new Node(1);
                groupNode1.AttachLayoutGroup(true);
                Node groupNode2 = new Node(2);
                groupNode2.AttachLayoutGroup(true);

                groupNode1.AppendChild(groupNode2);

                Assert.Equal(groupNode1, groupNode2.Parent);
                Assert.Contains(groupNode2, groupNode1);
            }
        }
    }
}