using System;
using System.Collections.Generic;
using System.Linq;
using AutoFixture;
using AutoFixture.AutoNSubstitute;
using BehaviourTree;
using BehaviourTree.Composites;
using static BehaviourTree.NodeState;

namespace BehaviourTreeTests.Utils
{
    public static class HelperFunctions
    {
        public static IFixture Fixture { get; } = NewFixture;

        public static List<Node> NewSuccessNodesList => NewListOfConstantStateNodes(Success);
        public static List<Node> NewFailureNodesList => NewListOfConstantStateNodes(Failure);
        public static Node NewFailureNode => NewConstantStateNode(Failure);
        public static Node NewSuccessNode => NewConstantStateNode(Success);
        public static Node NewRunningNode => NewConstantStateNode(Running);
        public static IFixture NewFixture
        {
            get
            {
                var fixture = new Fixture(new GreedyEngineParts());
                fixture.Customize(new AutoNSubstituteCustomization());
                return fixture;
            }
        }

        public static Node NewConstantStateNode(NodeState state) => Fixture.CreateNode(state);
            
        public static List<Node> NewListOfConstantStateNodes(NodeState state)
        {
            var list = new List<Node>();
            var count = new Random().Next(3, 5);
            for (var i = 0; i < count; i++) 
                list.Add(NewConstantStateNode(state));

            return list;
        }

        public static T New<T>() => Fixture.Create<T>();

        public static T New<T>(params Node[] children) where T : Composite 
            => Fixture.CreateComposite<T>(children.ToList());

        public static Node NewNode(NodeState state) => NewConstantStateNode(state);

        public static float NextFloat(this Random random, int min, int max) 
            => (float)random.NextDouble() * random.Next(min, max);

        public static List<T> With<T>(this List<T> list, T item)
        {
            list.Add(item);
            return list;
        }
        
        public static List<T> With<T>(this List<T> list, List<T> items)
        {
            list.AddRange(items);
            return list;
        }
    }
}