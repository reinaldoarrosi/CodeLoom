using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeLoom.Contexts
{
    public class Arguments : IEnumerable<object>
    {
        public static readonly Arguments Empty = new Arguments(new object[0]);

        private object[] _values;

        public Arguments(object[] values)
        {
            _values = values;
        }

        public object GetArgument(int index)
        {
            return _values[index];
        }

        public IEnumerator<object> GetEnumerator()
        {
            return _values.AsEnumerable().GetEnumerator();
        }

        public void SetArgument(int index, object value)
        {
            _values[index] = value;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
