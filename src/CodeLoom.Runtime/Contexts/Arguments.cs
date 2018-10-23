using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeLoom.Contexts
{
    public class Arguments
    {
        private string[] _names;
        private Type[] _types;
        private object[] _values;

        public Arguments(string[] names, Type[] types, object[] values)
        {
            _names = names;
            _types = types;
            _values = values;
        }

        public string GetArgumentName(int index)
        {
            return _names[index];
        }

        public int GetArgumentIndex(string name)
        {
            for (int i = 0; i < _names.Length; i++)
            {
                if (_names[i] == name) return i;
            }

            return -1;
        }

        public Type GetArgumentType(int index)
        {
            return _types[index];
        }
        public Type GetArgumentType(string name)
        {
            return GetArgumentType(GetArgumentIndex(name));
        }

        public object GetArgumentValue(int index)
        {
            return _values[index];
        }
        public object GetArgumentValue(string name)
        {
            return GetArgumentValue(GetArgumentIndex(name));
        }

        public void SetArgumentValue(int index, object value)
        {
            _values[index] = value;
        }
        public void SetArgumentValue(string name, object value)
        {
            _values[GetArgumentIndex(name)] = value;
        }
    }
}
