using System;
using System.Runtime.Remoting.Messaging;

namespace BehaviourTree
{
    public abstract class Node
    {
        public abstract NodeState Evaluate(float deltaTime = 0f);
    }
}