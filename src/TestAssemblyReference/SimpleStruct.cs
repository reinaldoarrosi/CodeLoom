using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestAssemblyReference
{
    public struct SimpleStruct
    {
        public SimpleStruct(int value)
        {
            Value = value;
        }

        public int Value { get; set; }

        public override string ToString()
        {
            return $"SimpleStruct={Value}";
        }
    }
}
