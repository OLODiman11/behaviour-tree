using System.Collections.Generic;
using static BehaviourTree.NodeState;

namespace BehaviourTree.Composites
{
    public class Selector : Composite
    {
        private int _currentIndex;
        
        public Selector(List<Node> children) : base(children) {}
        public Selector(bool reevaluateEveryFrame, List<Node> children) : base(reevaluateEveryFrame, children) {}

        public override NodeState Evaluate(float deltaTime = 0)
        {
            if (ReevaluateEveryFrame) _currentIndex = 0;
            while (_currentIndex < Children.Count)
            {
                var childState = Children[_currentIndex].Evaluate(deltaTime);
                if (childState == Success)
                {
                    _currentIndex = 0;
                    return Success;
                }
                if(childState == Running) return Running;

                _currentIndex++;
            }
            _currentIndex = 0;
            
            return Failure;
        }
    }
}