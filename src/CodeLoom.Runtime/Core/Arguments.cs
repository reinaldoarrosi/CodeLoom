using System;
using System.Collections;
using System.Collections.Generic;

namespace CodeLoom.Core
{
    public class Arguments : IReadOnlyDictionary<string, Argument>, IReadOnlyList<Argument>
    {
        private Dictionary<string, int> _keyToIndexMap = new Dictionary<string, int>();
        private IList<Argument> _arguments = new List<Argument>();

        public Arguments(Argument[] arguments)
        {
            if (arguments != null)
            {
                foreach (var arg in arguments)
                {
                    Add(arg);
                }
            }
        }

        public Argument this[string key]
        {
            get
            {
                return GetFromKey(key);
            }
        }

        public Argument this[int index]
        {
            get
            {
                return _arguments[index];
            }
        }

        public IEnumerable<string> Keys
        {
            get
            {
                return _keyToIndexMap.Keys;
            }
        }

        public IEnumerable<Argument> Values
        {
            get
            {
                return _arguments;
            }
        }

        public int Count
        {
            get
            {
                return _arguments.Count;
            }
        }

        public void Add(Argument argument)
        {
            if (argument == null)
                throw new ArgumentNullException();

            if (_keyToIndexMap.ContainsKey(argument.Name))
                throw new ArgumentException($"Argument {argument.Name} already exists.");

            _arguments.Add(argument);
            _keyToIndexMap.Add(argument.Name, _arguments.Count - 1);
        }

        public void ChangeArgumentValue(string name, object newValue)
        {
            var argument = this[name];
            argument.Value = newValue;
        }

        public void ChangeArgumentValue(int index, object newValue)
        {
            var argument = this[index];
            argument.Value = newValue;
        }

        public bool ContainsKey(string key)
        {
            return _keyToIndexMap.ContainsKey(key);
        }

        public bool TryGetValue(string key, out Argument value)
        {
            try
            {
                value = GetFromKey(key);
                return true;
            }
            catch
            {
                value = null;
                return false;
            }
        }

        public IEnumerator<Argument> GetEnumerator()
        {
            return _arguments.GetEnumerator();
        }

        IEnumerator<KeyValuePair<string, Argument>> IEnumerable<KeyValuePair<string, Argument>>.GetEnumerator()
        {
            foreach (var item in _keyToIndexMap)
            {
                yield return new KeyValuePair<string, Argument>(item.Key, _arguments[item.Value]);
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _keyToIndexMap.GetEnumerator();
        }

        private Argument GetFromKey(string key)
        {
            var index = _keyToIndexMap[key];
            return _arguments[index];
        }
    }
}
