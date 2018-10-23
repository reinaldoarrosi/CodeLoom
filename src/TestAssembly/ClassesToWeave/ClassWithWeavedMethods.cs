using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestAssemblyReference;

namespace TestAssembly.ClassesToWeave
{
    public class ClassWithWeavedMethods
    {
        public int NormalMethod(int a, int b)
        {
            return a + b;
        }
    }
}
