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
    public class ReturnInterceptedRefTypeListAspect : IInterceptMethodAspect
    {
        public void OnMethodInvoked(MethodContext context)
        {
            context.Proceed();

            var ret = (List<string>)context.ReturnValue;
            var item = ret[0];
            ret.Remove(item);
            ret.Add("cd");
            ret.Add(item + "b");
            context.SetReturnValue(ret);
        }
    }
}