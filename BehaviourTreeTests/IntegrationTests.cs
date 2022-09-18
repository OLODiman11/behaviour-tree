using System.CodeDom;
using System.Collections.Generic;
using BehaviourTree;
using BehaviourTree.Composites;
using NSubstitute;
using NUnit.Framework;
using static BehaviourTree.NodeState;
using static BehaviourTreeTests.Utils.HelperFunctions;

namespace BehaviourTreeTests
{
    [TestFixture]
    public class IntegrationTests
    {
        [Test]
        public void Test1()
        {
            var tree = CreateSequenceRootTree(0, 0);
            var lowerPriority = New<Node>();
            lowerPriority.Evaluate().Returns(Success);
            var higherPriority = NewNode(Running);
            var root = New<Sequence>(
                New<Selector>(NewNode(Success)),
                New<Selector>(AbortType.LowerPriority, lowerPriority),
                New<Selector>(higherPriority)
            );

            root.Evaluate();
            lowerPriority.Evaluate().Returns(Running);
            root.Evaluate();

            lowerPriority.Received(2).Evaluate();
            higherPriority.Received(1).Evaluate();
        }
        
        [Test]
        public void Test2()
        {
            var lowerPriority = New<Node>();
            lowerPriority.Evaluate().Returns(Failure);
            var higherPriority = NewNode(Running);
            var root = New<Selector>(
                New<Sequence>(NewNode(Failure)),
                New<Sequence>(AbortType.LowerPriority, lowerPriority),
                New<Sequence>(higherPriority)
            );

            root.Evaluate();
            lowerPriority.Evaluate().Returns(Running);
            root.Evaluate();

            lowerPriority.Received(2).Evaluate();
            higherPriority.Received(1).Evaluate();
        }

        private Node[] CreateSequenceRootTree(int lowerPriorityChild, int runningChild)
        {
            var tree = new Node[13];
            
            tree[12] = New<Sequence>(NewNode(Failure));
            tree[11] = New<Sequence>(NewNode(Failure));
            tree[10] = New<Sequence>(NewNode(Failure));
            tree[9] = New<Selector>(tree[10], tree[11], tree[12]);
            
            tree[8] = New<Sequence>(NewNode(Failure));
            tree[7] = New<Sequence>(NewNode(Failure));
            tree[6] = New<Sequence>(NewNode(Failure));
            tree[5] = New<Selector>(tree[6], tree[7], tree[8]);
            
            tree[4] = New<Sequence>(NewNode(Failure));
            tree[3] = New<Sequence>(NewNode(Failure));
            tree[2] = New<Sequence>(NewNode(Failure));
            tree[1] = New<Selector>(tree[2], tree[3], tree[4]);

            tree[0] = New<Sequence>(tree[1], tree[5], tree[9]);

            var lowerPriority = New<Node>();
            lowerPriority.Evaluate().Returns(Failure);
            tree[lowerPriorityChild] = New<Sequence>(lowerPriority);
            
            return tree;
        } 
    }
}