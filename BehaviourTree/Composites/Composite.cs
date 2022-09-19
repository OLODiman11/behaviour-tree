using System.Collections.Generic;

namespace BehaviourTree.Composites
{
    public abstract class Composite : Node
    {
        protected readonly List<Node> Children;
        protected readonly bool ReevaluateEveryFrame;

        protected Composite(List<Node> children) : this(false, children) {}
        protected Composite(bool reevaluateEveryFrame, List<Node> children)
        {
            Children = children;
            ReevaluateEveryFrame = reevaluateEveryFrame;
        }
    }
}