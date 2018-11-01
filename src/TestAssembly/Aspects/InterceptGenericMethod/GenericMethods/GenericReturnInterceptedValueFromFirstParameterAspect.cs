using CodeLoom.Aspects;
using CodeLoom.Contexts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace TestAssembly.Aspects.InterceptGenericMethod.GenericMethods
{
    public class GenericReturnInterceptedValueFromFirstParameterAspect : InterceptMethodAspect
    {
        public override void OnMethodInvoked(MethodContext context)
        {
            context.Arguments.SetArgument(0, 2);
            context.Proceed();
        }
    }
}