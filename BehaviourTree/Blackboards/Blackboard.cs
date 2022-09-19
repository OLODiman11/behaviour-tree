using System.Collections.Generic;

namespace BehaviourTree.Blackboards
{
    public class Blackboard : IBlackboard
    {
        private readonly Dictionary<string, object> _dictionary;

        public Blackboard() => _dictionary = new Dictionary<string, object>();

        public T Get<T>(string name) => (T)_dictionary[name];

        public void Set<T>(string name, T value) => _dictionary[name] = value;

        public void Register<T>(string name) => _dictionary.Add(name, default(T));
    }
}