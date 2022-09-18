using System;

namespace BehaviourTree
{
    [Flags]
    public enum AbortType
    {
        None = 0,
        Self = 1,
        LowerPriority = 2,
        Both = Self | LowerPriority
    }
}