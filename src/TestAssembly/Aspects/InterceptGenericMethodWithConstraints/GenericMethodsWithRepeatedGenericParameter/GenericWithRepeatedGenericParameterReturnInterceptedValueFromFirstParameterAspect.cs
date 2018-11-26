using CodeLoom.Aspects;
using CodeLoom.Contexts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using TestAssemblyReference;

namespace TestAssembly.Aspects.InterceptGenericMethodWithConstraints.GenericMethodsWithRepeatedGenericParameter
{
    public class GenericWithRepeatedGenericParameterReturnInterceptedValueFromFirstParameterAspect : IInterceptMethodAspect
    {
        public void OnMethodInvoked(MethodContext context)
        {
            context.Arguments.SetArgument(0, new InheritsFromSimpleClass(2));
            context.Proceed();
        }
    }
}