using System.Collections.Generic;
using static BehaviourTree.AbortType;

namespace BehaviourTree.Composites
{
    public abstract class Composite : Node
    {
        protected readonly List<Node> Children;
        public bool HasLowPriorityChild;
        
        protected Composite(List<Node> children) : this(None, children) {}
        protected Composite(AbortType abortType, List<Node> children)
        {
            AbortType = abortType;
            Children = children;
        }

        public abstract bool CheckIfRunning(float deltaTime);
        
        public AbortType AbortType { get; }
    }
}