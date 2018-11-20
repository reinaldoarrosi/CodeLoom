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
    public class ReturnInterceptedValueListAspect : InterceptAsyncMethodAspect
    {
        public async override Task OnMethodInvoked(AsyncMethodContext context)
        {
            await context.Proceed();

            var ret = (List<int>)context.ReturnValue;
            ret.Insert(0, 2);
            context.SetReturnValue(ret);
        }
    }
}