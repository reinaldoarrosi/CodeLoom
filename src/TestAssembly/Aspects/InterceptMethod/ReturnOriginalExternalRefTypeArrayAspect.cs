using CodeLoom.Aspects;
using CodeLoom.Contexts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace TestAssembly.Aspects.InterceptMethod
{
    public class ReturnOriginalExternalRefTypeArrayAspect : InterceptMethodAspect
    {
        public override void OnMethodInvoked(MethodContext context)
        {
            context.Proceed();
        }
    }
}