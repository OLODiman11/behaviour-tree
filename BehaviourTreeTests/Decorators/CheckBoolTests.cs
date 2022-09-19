using AutoFixture;
using BehaviourTree;
using BehaviourTree.Blackboards;
using BehaviourTree.Decorators;
using BehaviourTreeTests.Utils;
using NSubstitute;
using FluentAssertions;
using NUnit.Framework;
using static BehaviourTree.NodeState;
using static BehaviourTreeTests.Utils.HelperFunctions;

namespace BehaviourTreeTests.Decorators
{
    [TestFixture]
    public class CheckBoolTests
    {
        private static readonly IFixture Fixture = HelperFunctions.Fixture;
        private const string Condition = nameof(Condition);
        
        [Test]
        public void ReturnsSuccessIfTrue()
        {
            var blackboard = Fixture.Create<IBlackboard>();
            blackboard.Get<bool>(Condition).Returns(true);
            var decorator = Fixture.Create<CheckBool, string, IBlackboard>(
                "condition",Condition,
                nameof(blackboard), blackboard);
            
            var state = decorator.Evaluate();

            state.Should().Be(Success);
        }
        
        [TestCase(Running)]
        [TestCase(Success)]
        [TestCase(Failure)]
        public void ReturnsChildStateIfFalse(NodeState state)
        {
            var child = NewNode(state);
            var blackboard = Fixture.Create<IBlackboard>();
            blackboard.Get<bool>(Condition).Returns(false);
            var decorator = Fixture.Create<CheckBool, string, Node, IBlackboard>(
                "condition",Condition,
                nameof(child), child,
                nameof(blackboard), blackboard);

            decorator.Evaluate().Should().Be(state);
        }
        
        [TestCase(Running)]
        [TestCase(Success)]
        [TestCase(Failure)]
        public void PassesDeltaTime(NodeState state)
        {
            var deltaTime = 15.4f;
            var child = NewNode(state);
            var blackboard = Fixture.Create<IBlackboard>();
            blackboard.Get<bool>(Condition).Returns(false);
            var decorator = Fixture.Create<CheckBool, string, Node, IBlackboard>(
                "condition", Condition,
                nameof(child), child,
                nameof(blackboard), blackboard);

            decorator.Evaluate(deltaTime);

            child.Received().Evaluate(deltaTime);
        }
    }
}