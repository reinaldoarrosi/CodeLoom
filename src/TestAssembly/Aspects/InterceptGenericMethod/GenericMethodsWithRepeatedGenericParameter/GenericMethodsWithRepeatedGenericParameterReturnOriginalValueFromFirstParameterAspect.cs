using CodeLoom.Aspects;
using CodeLoom.Contexts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace TestAssembly.Aspects.InterceptGenericMethod.GenericMethodsWithRepeatedGenericParameter
{
    public class GenericMethodsWithRepeatedGenericParameterReturnOriginalValueFromFirstParameterAspect : InterceptMethodAspect
    {
        public override void OnMethodInvoked(MethodContext context)
        {
            context.Proceed();
        }
    }
}