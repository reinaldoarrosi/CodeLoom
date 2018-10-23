using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestAssemblyReference;

namespace TestAssembly.ClassesToWeave
{
    public class ParentClass
    {
        public int ParentValueProperty { get; set; }

        public SimpleClass ParentRefProperty { get; set; }

        public int[] ParentValueArrayProperty { get; set; }

        public SimpleClass[] ParentRefArrayProperty { get; set; }

        public List<int> ParentValueGenericProperty { get; set; }

        public List<SimpleClass> ParentRefGenericProperty { get; set; }

        public SimpleClass this[double a, SimpleClass b, double[] c, SimpleClass d, List<double> e, List<SimpleClass> f, ParentClass g]
        {
            get { return new SimpleClass(); }
            set { }
        }
    }

    public class ClassWithInterceptedProperties //: ParentClass
    {
        public int ValueProperty { get; set; }

        public SimpleClass RefProperty { get; set; }

        public int[] ValueArrayProperty { get; set; }

        public SimpleClass[] RefArrayProperty { get; set; }

        public List<int> ValueGenericProperty { get; set; }

        public List<SimpleClass> RefGenericProperty { get; set; }

        public int this[int a, SimpleClass b, int[] c, SimpleClass d, List<int> e, List<SimpleClass> f]
        {
            get { return 0; }
            set { }
        }

        public SimpleClass this[double a, SimpleClass b, double[] c, SimpleClass d, List<double> e, List<SimpleClass> f]
        {
            get { return new SimpleClass(); }
            set { }
        }
    }
}
