using System;
using BehaviourTree.Blackboards;
using static BehaviourTree.NodeState;

namespace BehaviourTree.Decorators
{
    public class CheckBool : Decorator
    {
        private readonly string _condition;

        public CheckBool(string condition, Node child) : base(null, child) => _condition = condition;
        public CheckBool(IBlackboard blackboard, string condition, Node child) : base(blackboard, child) => _condition = condition;

        public override NodeState Evaluate(float deltaTime = 0) 
            => Blackboard.Get<bool>(_condition) ? Success : Child.Evaluate(deltaTime);
    }
}