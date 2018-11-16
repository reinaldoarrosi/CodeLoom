using System;

namespace TestAssemblyReference
{
    public class InheritsFromSimpleClass : SimpleClass
    {
        public InheritsFromSimpleClass(int value)
            : base(value)
        { }

        public override string ToString()
        {
            return $"InheritsFromSimpleClass={Value}";
        }
    }
}
