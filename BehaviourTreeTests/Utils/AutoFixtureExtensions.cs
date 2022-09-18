using System.Collections.Generic;
using AutoFixture;
using AutoFixture.Kernel;
using BehaviourTree;
using BehaviourTree.Composites;
using NSubstitute;

namespace BehaviourTreeTests.Utils
{
    public static class AutoFixtureExtensions
    {
        public static Node CreateNode(this IFixture fixture, NodeState state)
        {
            var node = fixture.Create<Node>();
            node.Evaluate().Returns(state);
            return node;
        }

        public static T CreateComposite<T>(this IFixture fixture, AbortType abortType) where T : Composite
        {
            var specimenBuilder = new ParameterNameSpecimenBuilder<AbortType>(nameof(abortType), abortType);
            return fixture.CreateNode<T>(specimenBuilder);
        }
        
        public static T CreateComposite<T>(this IFixture fixture, List<Node> children) where T : Composite
        {
            var specimenBuilder = new ParameterNameSpecimenBuilder<List<Node>>(nameof(children), children);
            return fixture.CreateNode<T>(specimenBuilder);
        }
        
        public static T CreateComposite<T>(this IFixture fixture, AbortType abortType, List<Node> children) where T : Composite
        {
            var specimenBuilder1 = new ParameterNameSpecimenBuilder<AbortType>(nameof(abortType), abortType);
            var specimenBuilder2 = new ParameterNameSpecimenBuilder<List<Node>>(nameof(children), children);
            return fixture.CreateNode<T>(specimenBuilder1, specimenBuilder2);
        }

        public static Composite CreateComposite(this IFixture fixture, AbortType abortType)
        {
            var specimenBuilder = new ParameterNameSpecimenBuilder<AbortType>(nameof(abortType), abortType);
            fixture.Customizations.Add(specimenBuilder);
            var composite = fixture.Create<Composite>();
            fixture.Customizations.Remove(specimenBuilder);
            return composite;
        }

        
        private static T CreateNode<T>(this IFixture fixture, params ISpecimenBuilder[] builders) where T : Node
        {
            foreach (var builder in builders) fixture.Customizations.Add(builder);
            var composite = fixture.Create<T>();
            foreach (var builder in builders) fixture.Customizations.Remove(builder);
            return composite;
        }
    }
}