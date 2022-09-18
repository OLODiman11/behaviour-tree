using System;
using System.Collections.Generic;
using AutoFixture;
using BehaviourTree;
using BehaviourTree.Composites;
using BehaviourTreeTests.Utils;
using NSubstitute;
using FluentAssertions;
using NUnit.Framework;
using static BehaviourTree.NodeState;
using static BehaviourTreeTests.Utils.HelperFunctions;

namespace BehaviourTreeTests.Composites
{
    [TestFixture]
    public class SelectorTests
    {
        private static readonly IFixture Fixture = HelperFunctions.Fixture;
        
        [TestFixture]
        public class EvaluateMethod
        {
            
            [Test]
            public void EvaluatesChildrenLeftToRightOneByOne()
            {
                var children = NewFailureNodesList;
                var rightOrder = new Action(() => {});
                foreach (var child in children) 
                    rightOrder += () => child.Evaluate();
                var sequence = Fixture.CreateComposite<Selector>(children);
                
                sequence.Evaluate();
                
                Received.InOrder(rightOrder);
            }
            
            [TestCase(Failure, TestName = "OnFailure")]
            [TestCase(Success, TestName = "OnSuccess")]
            public void StartsOverFromTheFirstChild(NodeState state)
            {
                var children = new List<Node>()
                    .With(NewFailureNodesList)
                    .With(NewConstantStateNode(state));
                var sequence = Fixture.CreateComposite<Selector>(children);
            
                sequence.Evaluate();
                sequence.Evaluate();
            
                foreach (var child in children) child.Received(2).Evaluate();
            }
            
            [TestCase(Success, TestName = "SuccessChild")]
            [TestCase(Running, TestName = "RunningChild")]
            public void DoesNotEvaluateChildrenPast(NodeState state)
            {
                var afterFailure = NewFailureNodesList;
                var sequence = Fixture.CreateComposite<Selector>(new List<Node>()
                    .With(NewFailureNode)
                    .With(NewConstantStateNode(state))
                    .With(afterFailure));
                
                sequence.Evaluate();
            
                foreach (var child in afterFailure) 
                    child.DidNotReceive().Evaluate();
            }
            
            [TestCase(Success, TestName = "ToSuccessChild")]
            [TestCase(Failure, TestName = "ToFailureChild")]
            [TestCase(Running, TestName = "ToRunningChild")]
            public void PassesDeltaTime(NodeState state)
            {
                var random = new Random();
                var deltaTime = random.NextFloat(0, int.MaxValue);
                var child = NewConstantStateNode(state);
                var sequence = Fixture.CreateComposite<Selector>(new List<Node>{child});
            
                sequence.Evaluate(deltaTime);
            
                child.Received().Evaluate(deltaTime);
            }
            
            [TestCase(Success, TestName = "SuccessOnFirstSuccessChild")]
            [TestCase(Failure, TestName = "FailureOnAllChildrenFailed")]
            [TestCase(Running, TestName = "RunningOnFirstRunningChild")]
            public void Returns(NodeState state)
            {
                var sequence = Fixture.CreateComposite<Selector>(new List<Node>()
                    .With(NewFailureNode)
                    .With(NewConstantStateNode(state))
                    .With(NewFailureNode));
            
                var sequenceState = sequence.Evaluate();
            
                sequenceState.Should().Be(state);
            }
        }
        
        [TestFixture]
        public class AbortTypeProperty
        {
            private Node _higherPriority;
            private Node _lowerPriority;
            
            [SetUp]
            public void SetUp()
            {
                _lowerPriority = NewRunningNode;
                _higherPriority = NewFailureNode;
            }
                
            [TestFixture]
            public class None : AbortTypeProperty
            {
                [Test]
                public void DoesNotSwitchToHigherPriorityChild()
                {
                    var sequence = Fixture.CreateComposite<Selector>(AbortType.None, new List<Node>()
                        .With(_higherPriority)
                        .With(_lowerPriority));
            
                    sequence.Evaluate();
                    _higherPriority.Evaluate().Returns(Running);
                    sequence.Evaluate();
            
                    _lowerPriority.Received(2).Evaluate();
                    _higherPriority.Received(1).Evaluate();
                }
            
                [Test]
                public void DoesNotSwitchToItself()
                {
                    var sutSequence = Fixture.CreateComposite<Selector>(AbortType.None, new List<Node>().With(_higherPriority));
                    var rootSequence = Fixture.CreateComposite<Selector>(new List<Node>()
                        .With(sutSequence)
                        .With(_lowerPriority));
            
                    rootSequence.Evaluate();
                    _higherPriority.Evaluate().Returns(Running);
                    rootSequence.Evaluate();
            
                    _lowerPriority.Received(2).Evaluate();
                    _higherPriority.Received(1).Evaluate();
                }
            }
            
            [TestFixture]
            public class Self : AbortTypeProperty
            {
                [Test]
                public void SwitchesToHigherPriorityChild()
                {
                    var sequence = Fixture.CreateComposite<Selector>(AbortType.Self, new List<Node>()
                        .With(_higherPriority)
                        .With(_lowerPriority));
            
                    sequence.Evaluate();
                    _higherPriority.Evaluate().Returns(Running);
                    sequence.Evaluate();
            
                    _lowerPriority.Received(1).Evaluate();
                    _higherPriority.Received(2).Evaluate();
                }
            
                [Test]
                public void DoesNotSwitchToItself()
                {
                    var sutSequence = Fixture.CreateComposite<Selector>(AbortType.Self, new List<Node>().With(_higherPriority));
                    var rootSequence = Fixture.CreateComposite<Selector>(new List<Node>()
                        .With(sutSequence)
                        .With(_lowerPriority));
            
                    rootSequence.Evaluate();
                    _higherPriority.Evaluate().Returns(Running);
                    rootSequence.Evaluate();
            
                    _lowerPriority.Received(2).Evaluate();
                    _higherPriority.Received(1).Evaluate();
                }
            }
            
            [TestFixture]
            public class LowerPriority : AbortTypeProperty
            {
                [Test]
                public void DoesNotSwitchToHigherPriorityChild()
                {
                    var sequence = Fixture.CreateComposite<Selector>(AbortType.LowerPriority, new List<Node>()
                        .With(_higherPriority)
                        .With(_lowerPriority));
            
                    sequence.Evaluate();
                    _higherPriority.Evaluate().Returns(Running);
                    sequence.Evaluate();
            
                    _lowerPriority.Received(2).Evaluate();
                    _higherPriority.Received(1).Evaluate();
                }
            
                [Test]
                public void SwitchesToItself()
                {
                    var sutSequence =
                        Fixture.CreateComposite<Selector>(AbortType.LowerPriority, new List<Node>().With(_higherPriority));
                    var rootSequence = Fixture.CreateComposite<Selector>(new List<Node>()
                        .With(sutSequence)
                        .With(_lowerPriority));
            
                    rootSequence.Evaluate();
                    _higherPriority.Evaluate().Returns(Running);
                    rootSequence.Evaluate();
            
                    _lowerPriority.Received(1).Evaluate();
                    _higherPriority.Received(2).Evaluate();
                }
            }
            
            [TestFixture]
            public class Both : AbortTypeProperty
            {
                [Test]
                public void SwitchesToHigherPriorityChild()
                {
                    var sequence = Fixture.CreateComposite<Selector>(AbortType.Both, new List<Node>()
                        .With(_higherPriority)
                        .With(_lowerPriority));
            
                    sequence.Evaluate();
                    _higherPriority.Evaluate().Returns(Running);
                    sequence.Evaluate();
            
                    _lowerPriority.Received(1).Evaluate();
                    _higherPriority.Received(2).Evaluate();
                }
            
                [Test]
                public void SwitchesToItself()
                {
                    var sutSequence = Fixture.CreateComposite<Selector>(AbortType.Both, new List<Node>().With(_higherPriority));
                    var rootSequence = Fixture.CreateComposite<Selector>(new List<Node>()
                        .With(sutSequence)
                        .With(_lowerPriority));
            
                    rootSequence.Evaluate();
                    _higherPriority.Evaluate().Returns(Running);
                    rootSequence.Evaluate();
            
                    _lowerPriority.Received(1).Evaluate();
                    _higherPriority.Received(2).Evaluate();
                }
            }
        }
    }
}