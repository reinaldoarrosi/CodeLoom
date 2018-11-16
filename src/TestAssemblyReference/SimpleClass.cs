using System;

namespace TestAssemblyReference
{
    public class SimpleClass
    {
        public SimpleClass()
        { }

        public SimpleClass(int value)
        {
            Value = value;
        }

        public int Value { get; set; }

        public override string ToString()
        {
            return $"SimpleClass={Value}";
        }
    }
}
