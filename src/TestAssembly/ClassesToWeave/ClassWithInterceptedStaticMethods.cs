using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestAssemblyReference;

namespace TestAssembly.ClassesToWeave
{
    public class ParentMethodsClassStatic
    {
        public static void ParentMethodReturnVoid() { return; }

        public static int ParentMethodReturnInt() { return 0; }

        public static SimpleClass ParentMethodReturnSimpleClass() { return new SimpleClass(); }

        public static int[] ParentMethodReturnIntArray() { return new int[0]; }

        public static SimpleClass[] ParentMethodReturnSimpleClassArray() { return new SimpleClass[0]; }

        public static List<int> ParentMethodReturnIntList() { return new List<int>(); }

        public static List<SimpleClass> ParentMethodReturnSimpleClassList() { return new List<SimpleClass>(); }
    }

    public class ClassWithInterceptedStaticMethods : ParentMethodsClassStatic
    {
        public static void MethodReturnVoid() { return; }

        public static int MethodReturnInt() { return 0; }

        public static SimpleClass MethodReturnSimpleClass() { return new SimpleClass(); }

        public static int[] MethodReturnIntArray() { return new int[0]; }

        public static SimpleClass[] MethodReturnSimpleClassArray() { return new SimpleClass[0]; }

        public static List<int> MethodReturnIntList() { return new List<int>(); }

        public static List<SimpleClass> MethodReturnSimpleClassList() { return new List<SimpleClass>(); }
    }
}
