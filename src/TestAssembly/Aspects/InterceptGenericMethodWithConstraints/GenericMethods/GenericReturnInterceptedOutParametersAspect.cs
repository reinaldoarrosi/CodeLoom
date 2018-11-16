using CodeLoom.Aspects;
using CodeLoom.Contexts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using TestAssemblyReference;

namespace TestAssembly.Aspects.InterceptGenericMethodWithConstraints.GenericMethods
{
    public class GenericReturnInterceptedOutParametersAspect : InterceptMethodAspect
    {
        public override void OnMethodInvoked(MethodContext context)
        {
            context.Proceed();

            context.Arguments.SetArgument(0, new InheritsFromSimpleClass(2));
            context.Arguments.SetArgument(1, new SimpleClass(2));
            context.Arguments.SetArgument(2, new SimpleStruct(2));
            context.Arguments.SetArgument(3, new SimpleClass(2));
            context.Arguments.SetArgument(4, new[] { new InheritsFromSimpleClass(1), new InheritsFromSimpleClass(2) });
            context.Arguments.SetArgument(5, new[] { new SimpleClass(1), new SimpleClass(2) });
            context.Arguments.SetArgument(6, new[] { new SimpleStruct(1), new SimpleStruct(2) });
            context.Arguments.SetArgument(7, new[] { new SimpleClass(1), new SimpleClass(2) } );
            context.Arguments.SetArgument(8, new List<InheritsFromSimpleClass> { new InheritsFromSimpleClass(1), new InheritsFromSimpleClass(2) });
            context.Arguments.SetArgument(9, new List<SimpleClass> { new SimpleClass(1), new SimpleClass(2) });
            context.Arguments.SetArgument(10, new List<SimpleStruct> { new SimpleStruct(1), new SimpleStruct(2) });
            context.Arguments.SetArgument(11, new List<SimpleClass> { new SimpleClass(1), new SimpleClass(2) });
        }
    }
}