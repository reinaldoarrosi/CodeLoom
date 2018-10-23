using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestAssemblyReference;

namespace TestAssembly.ClassesToWeave
{
    public class ParentClassStatic
    {
        public static int ParentValueProperty { get; set; }

        public static SimpleClass ParentRefProperty { get; set; }

        public static int[] ParentValueArrayProperty { get; set; }

        public static SimpleClass[] ParentRefArrayProperty { get; set; }

        public static List<int> ParentValueGenericProperty { get; set; }

        public static List<SimpleClass> ParentRefGenericProperty { get; set; }
    }

    public class ClassWithInterceptedStaticProperties : ParentClassStatic
    {
        public static int ValueProperty { get; set; }

        public static SimpleClass RefProperty { get; set; }

        public static int[] ValueArrayProperty { get; set; }

        public static SimpleClass[] RefArrayProperty { get; set; }

        public static List<int> ValueGenericProperty { get; set; }

        public static List<SimpleClass> RefGenericProperty { get; set; }
    }
}
