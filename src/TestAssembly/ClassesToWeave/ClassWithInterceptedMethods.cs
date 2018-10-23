using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestAssemblyReference;

namespace TestAssembly.ClassesToWeave
{
    public class ParentMethodsClass
    {
        public void ParentMethodReturnVoid() { return; }

        public int ParentMethodReturnInt() { return 0; }

        public SimpleClass ParentMethodReturnSimpleClass() { return new SimpleClass(); }

        public int[] ParentMethodReturnIntArray() { return new int[0]; }

        public SimpleClass[] ParentMethodReturnSimpleClassArray() { return new SimpleClass[0]; }

        public List<int> ParentMethodReturnIntList() { return new List<int>(); }

        public List<SimpleClass> ParentMethodReturnSimpleClassList() { return new List<SimpleClass>(); }
    }

    public class ClassWithInterceptedMethods : ParentMethodsClass
    {
        public void MethodReturnVoid() { return; }

        public int MethodReturnInt() { return 0; }

        public SimpleClass MethodReturnSimpleClass() { return new SimpleClass(); }

        public int[] MethodReturnIntArray() { return new int[0]; }

        public SimpleClass[] MethodReturnSimpleClassArray() { return new SimpleClass[0]; }

        public List<int> MethodReturnIntList() { return new List<int>(); }

        public List<SimpleClass> MethodReturnSimpleClassList() { return new List<SimpleClass>(); }
    }
}
