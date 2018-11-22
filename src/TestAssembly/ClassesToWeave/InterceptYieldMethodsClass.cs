using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestAssembly.Aspects;
using TestAssemblyReference;

namespace TestAssembly.ClassesToWeave
{
    public class InterceptYieldMethodsClass
    {
        public IEnumerable<int> ReturnOriginalIntEnumerable()
        {
            yield return 1;
        }

        public IEnumerable<int> ReturnInterceptedIntEnumerable()
        {
            yield return 1;
        }

        public IEnumerable<SimpleClass> ReturnOriginalSimpleClassEnumerable()
        {
            yield return new SimpleClass(1);
        }

        public IEnumerable<SimpleClass> ReturnInterceptedSimpleClassEnumerable()
        {
            yield return new SimpleClass(1);
        }

        public IEnumerable<T> ReturnOriginalTEnumerable<T>()
        {
            yield return default(T);
        }

        public IEnumerable<T> ReturnInterceptedTEnumerable<T>()
        {
            yield return default(T);
        }

        public IEnumerable<T> ReturnEmptyEnumerable<T>()
        {
            yield break;
        }

        public IEnumerable<T> ReplaceEmptyEnumerableWithANonEmptyEnumerable<T>()
        {
            yield break;
        }
    }
}
