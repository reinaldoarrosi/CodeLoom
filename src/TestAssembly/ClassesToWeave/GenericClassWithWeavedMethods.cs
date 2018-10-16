using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestAssembly.ClassesToWeave
{
    public class GenericClassWithWeavedMethods<T1, T2>
    {
        public T1 NormalGenericMethod(T1 a, T2 b)
        {
            return a;
        }
    }
}
