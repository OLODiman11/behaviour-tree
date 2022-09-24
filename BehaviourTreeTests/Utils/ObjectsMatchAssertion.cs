using System.Collections;
using System.Reflection;

namespace BehaviourTreeTests.Utils
{
    public static class ObjectsMatchAssertion
    {
        public static bool Matches(this object @this, object other)
        {
            if (@this == null && other == null) return true;
            if (@this == null || other == null) return false;
            if (@this == other) return true;

            var thisType = @this.GetType();
            if (thisType != other.GetType()) return false;
            if (thisType.IsValueType) return @this.Equals(other);
            
            var fieldInfos = thisType.GetFields(
                BindingFlags.Public
                | BindingFlags.NonPublic
                | BindingFlags.Static
                | BindingFlags.Instance);

            foreach (var fieldInfo in fieldInfos)
            {
                var thisValue = fieldInfo.GetValue(@this);
                var otherValue = fieldInfo.GetValue(other);
                if (!thisValue.Matches(otherValue)) return false;
            }

            if (!typeof(IEnumerable).IsAssignableFrom(thisType)) return true;

            var thisEnumerator = ((IEnumerable) @this).GetEnumerator();
            var otherEnumerator = ((IEnumerable) other).GetEnumerator();
            var thisMoveNext = thisEnumerator.MoveNext();
            var otherMoveNext = otherEnumerator.MoveNext();
            while (thisMoveNext && otherMoveNext)
            {
                if (!thisEnumerator.Current.Matches(otherEnumerator.Current)) return false;
                thisMoveNext = thisEnumerator.MoveNext();
                otherMoveNext = otherEnumerator.MoveNext();
            }

            if (thisMoveNext != otherMoveNext) return false;

            return true;
        }
    }
}