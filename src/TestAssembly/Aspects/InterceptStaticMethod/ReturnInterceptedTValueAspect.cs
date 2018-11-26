using CodeLoom.Aspects;
using CodeLoom.Contexts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using TestAssemblyReference;

namespace TestAssembly.Aspects.InterceptStaticMethod
{
    public class ReturnInterceptedTValueAspect : IInterceptMethodAspect
    {
        public void OnMethodInvoked(MethodContext context)
        {
            context.Proceed();
            context.SetReturnValue(new SimpleClass(2));
        }
    }
}
