using CodeLoom.Aspects;
using CodeLoom.Contexts;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using TestAssemblyReference;

namespace TestAssembly.Aspects.InterceptGenericMethodWithConstraints.GenericMethodsWithRepeatedGenericParameter
{
    public class GenericWithRepeatedGenericParameterReturnInterceptedOutParametersAspect : InterceptMethodAspect
    {
        public override void OnMethodInvoked(MethodContext context)
        {
            context.Proceed();

            context.Arguments.SetArgument(0, new InheritsFromSimpleClass(2));
            context.Arguments.SetArgument(1, new[] { "ab" });
            context.Arguments.SetArgument(2, new[] { 2 });
            context.Arguments.SetArgument(3, new[] { new InheritsFromSimpleClass(1), new InheritsFromSimpleClass(2) });
            context.Arguments.SetArgument(4, new[] { new[] { "ab" } });
            context.Arguments.SetArgument(5, new[] { new[] { 2 } });
            context.Arguments.SetArgument(6, new List<InheritsFromSimpleClass> { new InheritsFromSimpleClass(1), new InheritsFromSimpleClass(2) });
            context.Arguments.SetArgument(7, new List<IEnumerable> { new[] { "ab" } });
            context.Arguments.SetArgument(8, new List<int[]> { new[] { 2 } });
        }
    }
}