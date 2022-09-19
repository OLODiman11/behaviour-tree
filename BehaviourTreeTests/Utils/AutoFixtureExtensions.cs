using System.Collections.Generic;
using System.Linq;
using AutoFixture;
using AutoFixture.Kernel;
using BehaviourTree;
using BehaviourTree.Composites;
using BehaviourTree.Decorators;
using BehaviourTree.Tasks;
using NSubstitute;
using NUnit.Framework;

namespace BehaviourTreeTests.Utils
{
    public static class AutoFixtureExtensions
    {
        public static Node CreateNode(this IFixture fixture, NodeState state)
        {
            var node = fixture.Create<Node>();
            node.Evaluate(Arg.Any<float>()).Returns(state);
            return node;
        }

        public static T CreateComposite<T>(this IFixture fixture, List<Node> children) where T : Composite
        {
            var specimenBuilder = new ParameterNameSpecimenBuilder<List<Node>>(nameof(children), children);
            return fixture.CreateWithArgs<T>(specimenBuilder);
        }
        
        public static T CreateComposite<T>(this IFixture fixture, bool reevaluateEveryFrame, List<Node> children) where T : Composite
        {
            var specimenBuilder1 = new ParameterNameSpecimenBuilder<bool>(nameof(reevaluateEveryFrame), reevaluateEveryFrame);
            var specimenBuilder2 = new ParameterNameSpecimenBuilder<List<Node>>(nameof(children), children);
            return fixture.CreateWithArgs<T>(specimenBuilder1, specimenBuilder2);
        }
        
        public static T Create<T, T1>(this IFixture fixture, string argName, T1 value)
        {
            var specimenBuilder = new ParameterNameSpecimenBuilder<T1>(argName, value);
            return fixture.CreateWithArgs<T>(specimenBuilder);
        }
        
        public static T Create<T, T1, T2>(this IFixture fixture, string argName1, T1 value1, string argName2, T2 value2)
        {
            var specimenBuilder1 = new ParameterNameSpecimenBuilder<T1>(argName1, value1);
            var specimenBuilder2 = new ParameterNameSpecimenBuilder<T2>(argName2, value2);
            return fixture.CreateWithArgs<T>(specimenBuilder1, specimenBuilder2);
        }
        
        public static T Create<T, T1, T2, T3>(this IFixture fixture, string argName1, T1 value1, string argName2, T2 value2, string argName3, T3 value3)
        {
            var specimenBuilder1 = new ParameterNameSpecimenBuilder<T1>(argName1, value1);
            var specimenBuilder2 = new ParameterNameSpecimenBuilder<T2>(argName2, value2);
            var specimenBuilder3 = new ParameterNameSpecimenBuilder<T3>(argName3, value3);
            return fixture.CreateWithArgs<T>(specimenBuilder1, specimenBuilder2, specimenBuilder3);
        }
        
        private static T CreateWithArgs<T>(this IFixture fixture, params ISpecimenBuilder[] builders)
        {
            foreach (var builder in builders) fixture.Customizations.Add(builder);
            var composite = fixture.Create<T>();
            foreach (var builder in builders) fixture.Customizations.Remove(builder);
            return composite;
        }
    }
}