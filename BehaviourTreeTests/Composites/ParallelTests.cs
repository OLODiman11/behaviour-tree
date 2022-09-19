using System;
using System.Collections.Generic;
using AutoFixture;
using BehaviourTree;
using BehaviourTree.Composites;
using BehaviourTreeTests.Utils;
using NSubstitute;
using NUnit.Framework;
using FluentAssertions;
using static BehaviourTree.NodeState;
using static BehaviourTreeTests.Utils.HelperFunctions;

namespace BehaviourTreeTests.Composites
{
    [TestFixture]
    public class ParallelTests
    {
        private static readonly IFixture Fixture = HelperFunctions.Fixture;
        
        [TestFixture]
        public class EvaluateMethod
        {
            
            [Test]
            public void EvaluatesAllChildrenOneTimeEveryFrame()
            {
                var children = new List<Node>()
                    .With(NewSuccessNode)
                    .With(NewRunningNode)
                    .With(NewFailureNode)
                    .With(NewSuccessNode)
                    .With(NewRunningNode)
                    .With(NewFailureNode);
                var parallel = Fixture.CreateComposite<Parallel>(children);
                
                parallel.Evaluate();
                parallel.Evaluate();

                foreach (var child in children) child.Received(2).Evaluate();
            }

            [TestCase(Success, TestName = "ToSuccessChild")]
            [TestCase(Failure, TestName = "ToFailureChild")]
            [TestCase(Running, TestName = "ToRunningChild")]
            public void PassesDeltaTime(NodeState state)
            {
                var random = new Random();
                var deltaTime = random.NextFloat(0, int.MaxValue);
                var child = NewConstantStateNode(state);
                var sequence = Fixture.CreateComposite<Parallel>(new List<Node>{child});

                sequence.Evaluate(deltaTime);

                child.Received().Evaluate(deltaTime);
            }
            
            [TestCase(Success, TestName = "SuccessIfAllChildrenSucceeded")]
            [TestCase(Failure, TestName = "FailureOnAnyFailedChild")]
            [TestCase(Running, TestName = "RunningOnAnyRunningChild")]
            public void Returns(NodeState state)
            {
                var sequence = Fixture.CreateComposite<Parallel>(new List<Node>()
                    .With(NewSuccessNodesList)
                    .With(NewConstantStateNode(state))
                    .With(NewSuccessNodesList));

                var sequenceState = sequence.Evaluate();

                sequenceState.Should().Be(state);
            }
        }
    }
}