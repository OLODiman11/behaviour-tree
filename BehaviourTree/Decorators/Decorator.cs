using System;
using BehaviourTree.Blackboards;

namespace BehaviourTree.Decorators
{
    public abstract class Decorator : Node
    {
        protected readonly Node Child;
        protected readonly IBlackboard Blackboard;
        
        protected Decorator(IBlackboard blackboard, Node child)
        {
            Blackboard = blackboard;
            Child = child;
        }
    }
}