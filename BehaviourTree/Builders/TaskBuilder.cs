using System;
using BehaviourTree.Blackboards;
using BehaviourTree.Tasks;

namespace BehaviourTree.Builders
{
    public class TaskBuilder<T> : NodeBuilder where T : Task
    {
        private readonly object[] _args;
        
        public TaskBuilder(IBlackboard blackboard, params object[] args)
        {
            _args = new object[args.Length + 1];
            _args[0] = blackboard;
            for (var i = 0; i < args.Length; i++) _args[i + 1] = args[i];
        }

        public override Node Build() => (Node) Activator.CreateInstance(typeof(T), _args);
    }
}