using CodeLoom.Aspects;
using CodeLoom.Contexts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using TestAssemblyReference;

namespace TestAssembly.Aspects.InterceptMethod
{
    public class ReturnInterceptedExternalValueTypeAspect : InterceptMethodAspect
    {
        public override void OnMethodInvoked(MethodContext context)
        {
            context.Proceed();

            var ret = (SimpleStruct)context.ReturnValue;
            ret.Value = ret.Value + 1;
            context.SetReturnValue(ret);
        }
    }
}