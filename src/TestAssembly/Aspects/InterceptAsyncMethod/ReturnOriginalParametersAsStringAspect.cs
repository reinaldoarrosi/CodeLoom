using CodeLoom.Aspects;
using CodeLoom.Contexts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace TestAssembly.Aspects.InterceptAsyncMethod
{
    public class ReturnOriginalParametersAsStringAspect : InterceptAsyncMethodAspect
    {
        public async override Task OnMethodInvoked(AsyncMethodContext context)
        {
            await context.Proceed();
        }
    }
}