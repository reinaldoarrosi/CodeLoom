using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;

namespace TestAssembly
{
    public class TestClass
    {
        private List<int> b = new List<int>();
        private int d;

        public TestClass()
        {
            d = 0;
        }

        public TestClass(int a)
        { }

        public TestClass(int a, int c)
            : this(a)
        { }
    }
}
