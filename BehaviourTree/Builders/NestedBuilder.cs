using System.Collections.Generic;
using BehaviourTree.Blackboards;
using BehaviourTree.Composites;
using BehaviourTree.Decorators;
using BehaviourTree.Tasks;

namespace BehaviourTree.Builders
{
    public abstract class NestedBuilder : IBuilder
    {
        protected readonly List<IBuilder> Children;
        private readonly IBlackboard _blackboard;
        private readonly NestedBuilder _parent;
        private readonly int _count;

        protected NestedBuilder(NestedBuilder parent, IBlackboard blackboard, int count)
        {
            _parent = parent;
            _blackboard = blackboard;
            _count = count;
            Children = new List<IBuilder>();
        }

        public static CompositeBuilder<T> Root<T>(int count, IBlackboard blackboard, params object[] args) where T : Composite 
            => new CompositeBuilder<T>(blackboard, count, args);

        public NestedBuilder Selector(int count, bool reevaluateEveryFrame = false) =>
            Composite<Selector>(count, reevaluateEveryFrame);
        public NestedBuilder Sequence(int count, bool reevaluateEveryFrame = false) =>
            Composite<Sequence>(count, reevaluateEveryFrame);
        public NestedBuilder Parallel(int count) =>
            Composite<Parallel>(count);
        
        public NestedBuilder Composite<TComposite>(int count, params object[] args) where TComposite : Composite
        {
            var builder = new CompositeBuilder<TComposite>(this, _blackboard, count, args);
            Children.Add(builder);
            return builder;
        }
        
        public NestedBuilder Task<TTask>(params object[] args) where TTask : Task
        {
            var builder = new TaskBuilder<TTask>(_blackboard, args);
            Children.Add(builder);
            return ReturnParent();
        }

        public NestedBuilder Decorator<TDecorator>(params object[] args) where TDecorator : Decorator
        {
            var builder = new DecoratorBuilder<TDecorator>(this, _blackboard, args);
            Children.Add(builder);
            return builder;
        }

        private NestedBuilder ReturnParent()
            => Children.Count < _count || _parent == null ? this : _parent.ReturnParent();
        public abstract Node Build();
    }
}