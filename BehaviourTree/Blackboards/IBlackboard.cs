namespace BehaviourTree.Blackboards
{
    public interface IBlackboard
    {
        T Get<T>(string name);

        void Set<T>(string name, T value);

        void Register<T>(string name);
    }
}