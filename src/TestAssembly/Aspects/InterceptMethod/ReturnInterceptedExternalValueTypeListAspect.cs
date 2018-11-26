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
    public class ReturnInterceptedExternalValueTypeListAspect : IInterceptMethodAspect
    {
        public void OnMethodInvoked(MethodContext context)
        {
            context.Proceed();

            var ret = (List<SimpleStruct>)context.ReturnValue;
            ret.Insert(0, new SimpleStruct(2));
            context.SetReturnValue(ret);
        }
    }
}