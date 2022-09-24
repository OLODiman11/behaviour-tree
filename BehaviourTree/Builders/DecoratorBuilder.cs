using System;
using System.Collections.Generic;
using System.Linq;
using BehaviourTree.Blackboards;
using BehaviourTree.Decorators;

namespace BehaviourTree.Builders
{
    public class DecoratorBuilder<T> : NestedBuilder where T : Decorator
    {
        private readonly List<object> _args;

        public DecoratorBuilder(params object[] args) 
            : this(null, null, args) {}
        public DecoratorBuilder(IBlackboard blackboard, params object[] args) 
            : this(null, blackboard, args) {}
        public DecoratorBuilder(NestedBuilder parent, IBlackboard blackboard, params object[] args) 
            : base(parent, blackboard, 1)
        {
            _args = new List<object>();
            _args.Add(blackboard);
            _args.AddRange(args);
        }

        public override Node Build()
        {
            _args.Add(Children.Select(child => child.Build()).ToList()[0]);
            return (Node) Activator.CreateInstance(typeof(T), _args.ToArray());
        }
    }
}