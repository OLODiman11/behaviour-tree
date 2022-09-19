using BehaviourTree.Blackboards;

namespace BehaviourTree.Tasks
{
    public abstract class Task : Node
    {
        protected readonly IBlackboard Blackboard;

        protected Task(IBlackboard blackboard) => Blackboard = blackboard;
    }
}