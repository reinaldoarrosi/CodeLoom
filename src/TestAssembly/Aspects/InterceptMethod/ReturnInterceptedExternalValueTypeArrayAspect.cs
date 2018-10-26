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
    public class ReturnInterceptedExternalValueTypeArrayAspect : InterceptMethodAspect
    {
        public override void OnMethodInvoked(MethodContext context)
        {
            context.Proceed();

            var ret = (SimpleStruct[])context.ReturnValue;
            ret = new[] { new SimpleStruct(2), ret[0] };
            context.SetReturnValue(ret);
        }
    }
}