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
    public class ReturnInterceptedExternalRefTypeListAspect : InterceptMethodAspect
    {
        public override void OnMethodInvoked(MethodContext context)
        {
            context.Proceed();

            var ret = (List<SimpleClass>)context.ReturnValue;
            ret.Insert(0, new SimpleClass(2));
            context.SetReturnValue(ret);
        }
    }
}