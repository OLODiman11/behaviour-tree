using System.Collections;
using System.Collections.Generic;
using BehaviourTree;
using BehaviourTree.Blackboards;
using BehaviourTree.Builders;
using BehaviourTree.Composites;
using BehaviourTree.Decorators;
using BehaviourTree.Tasks;
using BehaviourTreeTests.Utils;
using FluentAssertions;
using NUnit.Framework;
using static BehaviourTree.Builders.NestedBuilder;

namespace BehaviourTreeTests.Builders
{
    [TestFixture]
    public class CompositeBuilderTests
    {
        private class BuildsGivenTypeData : IEnumerable
        {
            public IEnumerator GetEnumerator()
            {
                yield return new Sequence(new List<Node>());
                yield return new Selector(new List<Node>());
                yield return new Parallel(new List<Node>());
            }
        }
        
        private class PassesArgumentsData : IEnumerable
        {
            public IEnumerator GetEnumerator()
            {
                yield return new Sequence(true, new List<Node>());
                yield return new Selector(true, new List<Node>());
            }
        }
        
        private class AddsTaskData : IEnumerable
        {
            public IEnumerator GetEnumerator()
            {
                yield return new Sequence(new List<Node>{new Wait(15f), new Wait(45f)});
                yield return new Selector(new List<Node>{new Wait(15f), new Wait(45f)});
                yield return new Parallel(new List<Node>{new Wait(15f), new Wait(45f)});
            }
        }

        [TestCaseSource(typeof(BuildsGivenTypeData))]
        public void BuildsGivenType<T>(T expected) where T : Composite
        {
            var builder = new CompositeBuilder<T>(0);

            var composite = builder.Build();

            composite.Matches(expected).Should().BeTrue();
        }
        
        [TestCaseSource(typeof(PassesArgumentsData))]
        public void PassesArguments<T>(T expected) where T : Composite
        {
            var builder = new CompositeBuilder<T>(0, true);

            var composite = builder.Build();

            composite.Matches(expected).Should().BeTrue();
        }
        
        
        [TestCaseSource(typeof(AddsTaskData))]
        public void AddsTask<T>(T expected) where T : Composite
        {
            var builder = new CompositeBuilder<T>(2);
            builder.Task<Wait>(15f);
            builder.Task<Wait>(45f);
            
            var composite = builder.Build();

            composite.Matches(expected).Should().BeTrue();
        }
        
        private class EbanutiiTestData : IEnumerable
        {
            public IEnumerator GetEnumerator()
            {
                var blackboard = new Blackboard();
                blackboard.Register<float>("Float");
                blackboard.Set("Float", 15f);

                var composite = new Selector(new List<Node>
                {
                    new Sequence(true, new List<Node>
                    {
                        new CheckBool(blackboard, "dada", new Selector(new List<Node>
                        {
                            new Wait(blackboard, 32f),
                            new Wait(blackboard, 43f)
                        })),
                        new Sequence(new List<Node>
                        {
                            new Wait(blackboard, 32f)
                        }),
                        new Wait(blackboard, 43f)
                    }),
                    new Parallel(new List<Node>
                    {
                        new Wait(blackboard, 32f),
                        new Wait(blackboard, 43f)
                    }),
                    new Sequence(new List<Node>
                    {
                        new Wait(blackboard, 32f)
                    })
                });

                yield return composite;
            }
        }
        
        [TestCaseSource(typeof(EbanutiiTestData))]
        public void EbanutiiTest<T>(T expected)
        {
            var blackboard = new Blackboard();
            blackboard.Register<float>("Float");
            blackboard.Set("Float", 15f);
            var composite = 
                Root<Selector>(3, blackboard)
                    .Composite<Sequence>(3, true)
                        .Decorator<CheckBool>("dada").Composite<Selector>(2)
                            .Task<Wait>(32f)
                            .Task<Wait>(43f)
                        .Composite<Sequence>(1)
                            .Task<Wait>(32f)
                        .Task<Wait>(43f)
                    .Composite<Parallel>(2)
                        .Task<Wait>(32f)
                        .Task<Wait>(43f)
                    .Composite<Sequence>(1)
                        .Task<Wait>(32f)
                .Build();

            composite.Matches(expected).Should().BeTrue();
        }
        
        [TestCaseSource(typeof(EbanutiiTestData))]
        public void EbanutiiTest2<T>(T expected)
        {
            var blackboard = new Blackboard();
            blackboard.Register<float>("Float");
            blackboard.Set("Float", 15f);
            var composite = 
                Root<Selector>(3, blackboard)
                    .Sequence(3, true)
                        .Decorator<CheckBool>("dada").Selector(2)
                            .Task<Wait>(32f)
                            .Task<Wait>(43f)
                        .Sequence(1)
                            .Task<Wait>(32f)
                        .Task<Wait>(43f)
                    .Parallel(2)
                        .Task<Wait>(32f)
                        .Task<Wait>(43f)
                    .Sequence(1)
                        .Task<Wait>(32f)
                .Build();

            composite.Matches(expected).Should().BeTrue();
        }
    }
}