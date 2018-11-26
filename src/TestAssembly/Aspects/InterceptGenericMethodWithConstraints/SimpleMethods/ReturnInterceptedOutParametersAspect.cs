using CodeLoom.Aspects;
using CodeLoom.Contexts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using TestAssemblyReference;

namespace TestAssembly.Aspects.InterceptGenericMethodWithConstraints.SimpleMethods
{
    public class ReturnInterceptedOutParametersAspect : IInterceptMethodAspect
    {
        public void OnMethodInvoked(MethodContext context)
        {
            context.Proceed();

            context.Arguments.SetArgument(0, new InheritsFromSimpleClass(2));
            context.Arguments.SetArgument(1, new SimpleClass(2));
            context.Arguments.SetArgument(2, new [] { new InheritsFromSimpleClass(1), new InheritsFromSimpleClass(2) });
            context.Arguments.SetArgument(3, new [] { new SimpleClass(1), new SimpleClass(2) });
            context.Arguments.SetArgument(4, new List<InheritsFromSimpleClass> { new InheritsFromSimpleClass(1), new InheritsFromSimpleClass(2) });
            context.Arguments.SetArgument(5, new List<SimpleClass> { new SimpleClass(1), new SimpleClass(2) });
        }
    }
}