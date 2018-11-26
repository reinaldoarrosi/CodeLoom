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
    public class ReturnInterceptedT2AsyncAspect : IInterceptAsyncMethodAspect
    {
        public async Task OnMethodInvoked(AsyncMethodContext context)
        {
            await context.Proceed();
            context.SetReturnValue("b");
        }
    }
}
