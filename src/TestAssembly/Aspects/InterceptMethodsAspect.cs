using CodeLoom.Aspects;
using CodeLoom.Contexts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestAssembly.Aspects
{
    public class InterceptMethodsAspect : InterceptMethodAspect
    {
        public override void OnMethodInvoked(MethodContext context)
        {
            context.Proceed();
        }
    }
}
