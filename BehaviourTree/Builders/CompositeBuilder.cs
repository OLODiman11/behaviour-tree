using System;
using System.Collections.Generic;
using System.Linq;
using BehaviourTree.Blackboards;
using BehaviourTree.Composites;
using BehaviourTree.Tasks;

namespace BehaviourTree.Builders
{
    public abstract class CompositeBuilder : NodeBuilder
    {
        protected readonly List<NodeBuilder> Children;
        private readonly IBlackboard _blackboard;
        private readonly CompositeBuilder _parent;

        protected CompositeBuilder(CompositeBuilder parent, IBlackboard blackboard)
        {
            _parent = parent;
            Children = new List<NodeBuilder>();
            _blackboard = blackboard;
        }

        public CompositeBuilder Selector() => Composite<Selector>();
        public CompositeBuilder Sequence() => Composite<Sequence>();
        public CompositeBuilder Parallel() => Composite<Parallel>();

        public CompositeBuilder<TComposite> Composite<TComposite>() where TComposite : Composite
        {
            var builder = new CompositeBuilder<TComposite>(this, _blackboard);
            Children.Add(builder);
            return builder;
        }
        
        public CompositeBuilder Task<TTask>(params object[] args) where TTask : Task
        {
            var builder = new TaskBuilder<TTask>(_blackboard, args);
            Children.Add(builder);
            return this;
        }

        public CompositeBuilder End() => _parent;
    }
    
    public class CompositeBuilder<T> : CompositeBuilder where T : Composite
    {
        public CompositeBuilder() : base(null, null) {}
        public CompositeBuilder(IBlackboard blackboard) : base(null, blackboard) {}
        public CompositeBuilder(CompositeBuilder parent, IBlackboard blackboard) : base(parent, blackboard) {}

        public override Node Build()
            => (Node)Activator.CreateInstance(typeof(T), Children.Select(child => child.Build()).ToList());
        
    }
}