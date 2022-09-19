using System.Collections.Generic;
using static BehaviourTree.NodeState;

namespace BehaviourTree.Composites
{
    public class Parallel : Composite
    {
        public Parallel(List<Node> children) : base(children) {}

        public override NodeState Evaluate(float deltaTime = 0)
        {
            var returnState = Success;
            foreach (var child in Children)
            {
                var childState = child.Evaluate(deltaTime);
                if (childState != Success) returnState = childState;
            }
            return returnState;
        }
    }
}