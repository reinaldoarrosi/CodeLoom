using CodeLoom.Aspects;
using CodeLoom.Contexts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace TestAssembly.Aspects.InterceptStaticMethod
{
    public class ReturnInterceptedIntValueAspect : IInterceptMethodAspect
    {
        public void OnMethodInvoked(MethodContext context)
        {
            context.Proceed();
            context.SetReturnValue(2);
        }
    }
}
