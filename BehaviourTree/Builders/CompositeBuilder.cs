using System;
using System.Collections.Generic;
using System.Linq;
using BehaviourTree.Blackboards;
using BehaviourTree.Composites;

namespace BehaviourTree.Builders
{
    public class CompositeBuilder<T> : NestedBuilder where T : Composite
    {
        private readonly List<object> _args;
        
        public CompositeBuilder(int count, params object[] args) 
            : this(null, null, count, args) {}
        public CompositeBuilder(IBlackboard blackboard, int count, params object[] args) 
            : this(null, blackboard, count, args) {}
        public CompositeBuilder(NestedBuilder parent, IBlackboard blackboard, int count, params object[] args) 
            : base(parent, blackboard, count) => _args = args.ToList();

        public override Node Build()
        {
            _args.Add(Children.Select(child => child.Build()).ToList());
            return (Node) Activator.CreateInstance(typeof(T), _args.ToArray());
        }
    }
}