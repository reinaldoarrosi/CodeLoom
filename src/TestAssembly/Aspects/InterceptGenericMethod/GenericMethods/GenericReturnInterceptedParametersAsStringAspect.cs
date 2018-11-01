using CodeLoom.Aspects;
using CodeLoom.Contexts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using TestAssemblyReference;

namespace TestAssembly.Aspects.InterceptGenericMethod.GenericMethods
{
    public class GenericReturnInterceptedParametersAsStringAspect : InterceptMethodAspect
    {
        public override void OnMethodInvoked(MethodContext context)
        {
            context.Arguments.SetArgument(0, 2);
            context.Arguments.SetArgument(1, "ab");
            context.Arguments.SetArgument(2, new SimpleStruct(2));
            context.Arguments.SetArgument(3, new SimpleClass(2));

            context.Proceed();
        }
    }
}