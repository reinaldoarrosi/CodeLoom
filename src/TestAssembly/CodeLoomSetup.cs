using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CodeLoom.Aspects;
using TestAssembly.Aspects;

[assembly:CodeLoom.CodeLoomSetup(typeof(TestAssembly.CodeLoomSetup))]

namespace TestAssembly
{
    public class CodeLoomSetup : CodeLoom.CodeLoomSetup
    {
        public override IEnumerable<ImplementInterfaceAspect> GetAspects(Type type)
        {
            if (type == typeof(ClassesToWeave.ClassWithWeavedProperties))
                yield return new AddPropertiesToClassAspect();

            if (type.FullName == typeof(ClassesToWeave.GenericClassWithWeavedProperties<,>).FullName)
                yield return new AddPropertiesToClassAspect();

            if (type == typeof(ClassesToWeave.ClassWithWeavedMethods))
                yield return new AddMethodsToClassAspect();

            if (type == typeof(ClassesToWeave.GenericClassWithWeavedMethods<,>))
                yield return new AddMethodsToClassAspect();
        }
    }
}
