using System.Collections.Generic;
using BehaviourTree;
using BehaviourTree.Composites;
using FluentAssertions;
using NSubstitute;
using NUnit.Framework;
using static BehaviourTree.AbortType;

namespace BehaviourTreeTests.Composites
{
    [TestFixture]
    public class CompositeTests
    {
        [TestFixture]
        public class Constructor
        {
            [Test]
            public void SetsAbortTypeToNoneIfNotSpecified()
            {
                var composite = Substitute.For<Composite>(new List<Node>());

                composite.AbortType.Should().Be(None);
            }
            
            [TestCase(None)]
            [TestCase(Self)]
            [TestCase(LowerPriority)]
            [TestCase(Both)]
            public void SetsAbortType(AbortType abortType)
            {
                var composite = Substitute.For<Composite>(abortType, new List<Node>());

                composite.AbortType.Should().Be(abortType);
            }

            [Test]
            public void SwitchesToLowerPriorityChild()
            {
                var tree = CreateTree(out var lowerPriority, out var higherPriority);

                tree[0].Evaluate();
                higherPriority.Evaluate().Returns(NodeState.Running);
                tree[0].Evaluate();
                tree[0].Evaluate();

                for (var i = 0; i < tree.Length; i++)
                {
                    if (i == 0 || i == 3) tree[i].Received(3).Evaluate();
                    else if (i == 1) tree[i].Received(2).Evaluate();
                    else tree[i].Received(1).Evaluate();
                }
            }

            private Node[] CreateTree(out Node lowerPriority, out Node higherPriority)
            {
                lowerPriority = NewComposite(NodeState.Running);
                higherPriority = NewComposite(NodeState.Success, LowerPriority);
                var nodes = new Node[13];
                
                nodes[2] = NewComposite(NodeState.Success);
                nodes[3] = higherPriority;
                nodes[4] = NewComposite(NodeState.Success);
                nodes[6] = NewComposite(NodeState.Success);
                nodes[7] = NewComposite(NodeState.Success);
                nodes[8] = NewComposite(NodeState.Success);
                nodes[10] = NewComposite(NodeState.Success);
                nodes[11] = lowerPriority;
                nodes[12] = NewComposite(NodeState.Success);

                nodes[1] = Substitute.For<Sequence>(None, new List<Node> {nodes[2], nodes[3], nodes[5]});
                nodes[5] = Substitute.For<Sequence>(None, new List<Node> {nodes[6], nodes[7], nodes[8]});
                nodes[9] = Substitute.For<Sequence>(None, new List<Node> {nodes[10], nodes[11], nodes[12]});

                nodes[0] = Substitute.For<Sequence>(None, new List<Node> {nodes[1], nodes[5], nodes[9]});

                return nodes;
            }
            
            private static Composite NewComposite(NodeState state, AbortType abortType = None)
            {
                var composite = Substitute.For<Composite>(abortType, new List<Node>());
                composite.Evaluate().Returns(state);
                return composite;
            }
        }
    }
}