using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestAssembly.ClassesToWeave
{
    public class GenericClassWithWeavedProperties<T1, T2>
    {
        public T1 GenericNormalProperty1 { get; set; }
        public T2 GenericNormalProperty2 { get; set; }
    }
}
