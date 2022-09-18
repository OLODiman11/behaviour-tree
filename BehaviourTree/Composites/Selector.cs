using System.Collections.Generic;
using static BehaviourTree.AbortType;
using static BehaviourTree.NodeState;

namespace BehaviourTree.Composites
{
    public class Selector : Composite
    {
        private int _currentIndex;
        
        public Selector(List<Node> children) : base(children)
        {
        }

        public Selector(AbortType abortType, List<Node> children) : base(abortType, children)
        {
        }

        public override NodeState Evaluate(float deltaTime = 0)
        {
            if (CheckIfRunning(deltaTime)) return Running;

            if (AbortType.HasFlag(Self))
            {
                for (var i = 0; i < _currentIndex; i++)
                {
                    var child = Children[i];
                    var childState = child.Evaluate(deltaTime);
                    if (childState == Running)
                    {
                        _currentIndex = i;
                        return Running;
                    }
                }
            }

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

        public override bool CheckIfRunning(float deltaTime)
        {
            if (!HasLowPriorityChild) return false;
            
            for (var i = 0; i < _currentIndex; i++)
            {
                var child = Children[i];
                if (child is Composite composite)
                    if (composite.AbortType.HasFlag(LowerPriority))
                    {
                        var compositeState = composite.Evaluate(deltaTime);
                        if (compositeState == Running)
                        {
                            _currentIndex = i;
                            return true;
                        }
                    }
                    else if (composite.HasLowPriorityChild)
                    {
                        if (composite.CheckIfRunning(deltaTime))
                        {
                            _currentIndex = i;
                            return true;
                        }
                    }
            }
            
            

            return false;
        }
    }
}