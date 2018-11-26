using CodeLoom.Aspects;
using CodeLoom.Contexts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using TestAssemblyReference;

namespace TestAssembly.Aspects.InterceptGenericMethod.GenericMethodsWithRepeatedGenericParameter
{
    public class GenericWithRepeatedGenericParameterReturnInterceptedOutParametersAspect : IInterceptMethodAspect
    {
        public void OnMethodInvoked(MethodContext context)
        {
            context.Proceed();

            context.Arguments.SetArgument(0, 2);
            context.Arguments.SetArgument(1, new SimpleStruct(2));
            context.Arguments.SetArgument(2, new SimpleClass(2));
            context.Arguments.SetArgument(3, new int[] { 1, 2 });
            context.Arguments.SetArgument(4, new[] { new SimpleStruct(1), new SimpleStruct(2) });
            context.Arguments.SetArgument(5, new[] { new SimpleClass(1), new SimpleClass(2) });
            context.Arguments.SetArgument(6, new List<int> { 1, 2 });
            context.Arguments.SetArgument(7, new List<SimpleStruct> { new SimpleStruct(1), new SimpleStruct(2) });
            context.Arguments.SetArgument(8, new List<SimpleClass> { new SimpleClass(1), new SimpleClass(2) });
        }
    }
}