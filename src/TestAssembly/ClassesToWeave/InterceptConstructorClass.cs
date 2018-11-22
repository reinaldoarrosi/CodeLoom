using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestAssembly.Aspects;
using TestAssemblyReference;

namespace TestAssembly.ClassesToWeave
{
    public class InterceptConstructorClass<T>
    {
        public class InnerClass<T2>
        {
            public InnerClass(T a, T2 b, T[] c, T2[] d, List<T> e, List<T2> f)
            {
                TValue = a;
                T2Value = b;
                TArray = c;
                T2Array = d;
                TList = e;
                T2List = f;
            }

            public T TValue { get; set; }
            public T2 T2Value { get; set; }
            public T[] TArray { get; set; }
            public T2[] T2Array { get; set; }
            public List<T> TList { get; set; }
            public List<T2> T2List { get; set; }
        }

        public InterceptConstructorClass(int a)
        {
            IntValue = a;
        }

        public InterceptConstructorClass(SimpleClass a)
        {
            SimpleClassValue = a;
        }

        public InterceptConstructorClass(int a, SimpleClass b, T c, int[] d, SimpleClass[] e, T[] f, List<int> g, List<SimpleClass> h, List<T> i)
        {
            IntValue = a;
            IntArray = d;
            IntList = g;

            SimpleClassValue = b;
            SimpleClassArray = e;
            SimpleClassList = h;

            TValue = c;
            TArray = f;
            TList = i;
        }

        public int IntValue { get; set; }
        public int[] IntArray { get; set; }
        public List<int> IntList { get; set; }

        public SimpleClass SimpleClassValue { get; set; }
        public SimpleClass[] SimpleClassArray { get; set; }
        public List<SimpleClass> SimpleClassList { get; set; }

        public T TValue { get; set; }
        public T[] TArray { get; set; }
        public List<T> TList { get; set; }
    }
}
