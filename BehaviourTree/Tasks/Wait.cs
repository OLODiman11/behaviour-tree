using System;
using BehaviourTree.Blackboards;
using static BehaviourTree.NodeState;

namespace BehaviourTree.Tasks
{
    public class Wait : Task
    {
        private readonly float _time;
        private float _elapsed;
        
        public Wait(float time = -1) : base(null) => _time = time;
        public Wait(IBlackboard blackboard, float time = -1) : base(blackboard) => _time = time;

        public override NodeState Evaluate(float deltaTime = 0)
        {
            if (_time < 0) return Running;
            
            _elapsed += deltaTime;
            if (_elapsed < _time) return Running;
            
            _elapsed = 0;
            return Success;

        }
    }
}