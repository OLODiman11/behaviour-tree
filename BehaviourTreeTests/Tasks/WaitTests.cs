using AutoFixture;
using BehaviourTree;
using BehaviourTree.Builders;
using BehaviourTree.Tasks;
using BehaviourTreeTests.Utils;
using NUnit.Framework;
using FluentAssertions;
using static BehaviourTree.NodeState;

namespace BehaviourTreeTests.Tasks
{
    [TestFixture]
    public class WaitTests
    {
        private static readonly IFixture Fixture = HelperFunctions.Fixture;

        private Wait _wait;
        
        [SetUp]
        public void SetUp()
        {
            var time = 14.5f;
            _wait = Fixture.Create<Wait, float>(nameof(time), time);
        }
        
        [Test]
        public void ReturnsRunningIfTimeNotPassed()
        {
            _wait.Evaluate(14f).Should().Be(Running);
        }
        
        [Test]
        public void AlwaysReturnsRunningIfTimeNotGiven()
        {
            var wait = new Wait();
            
            wait.Evaluate(float.MaxValue).Should().Be(Running);
        }
        
        [Test]
        public void ReturnsSuccessIfTimePassed()
        {
            _wait.Evaluate(15f).Should().Be(Success);
        }
        
        [Test]
        public void AccumulatesDeltaTime()
        {
            _wait.Evaluate(5f).Should().Be(Running);
            _wait.Evaluate(4f).Should().Be(Running);
            _wait.Evaluate(5.6f).Should().Be(Success);
        }

        [Test]
        public void RestartsOnSuccess()
        {
            _wait.Evaluate(15f).Should().Be(Success);
            _wait.Evaluate(10f).Should().Be(Running);
        }
    }
}