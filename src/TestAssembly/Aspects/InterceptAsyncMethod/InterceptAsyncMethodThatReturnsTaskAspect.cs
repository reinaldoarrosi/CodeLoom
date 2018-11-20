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
    public class InterceptAsyncMethodThatReturnsTaskAspect : InterceptAsyncMethodAspect
    {
        public static int COUNTER_BEFORE = 0;
        public static int COUNTER_AFTER = 0;

        public static void ResetCounters()
        {
            COUNTER_BEFORE = 0;
            COUNTER_AFTER = 0;
        }

        public async override Task OnMethodInvoked(AsyncMethodContext context)
        {
            COUNTER_BEFORE++;
            await context.Proceed();
            COUNTER_AFTER++;
        }
    }
}
