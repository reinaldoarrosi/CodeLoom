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
    public class ReturnInterceptedGenericValueListAspect : IInterceptAsyncMethodAspect
    {
        public async Task OnMethodInvoked(AsyncMethodContext context)
        {
            await context.Proceed();

            var value = (List<string>)context.ReturnValue;
            value.Add("b");
        }
    }
}