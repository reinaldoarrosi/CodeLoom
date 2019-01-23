using CodeLoom.Aspects;
using CodeLoom.Contexts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using TestAssemblyReference;

namespace TestAssembly.Aspects.InterceptProperty
{
    public class InterceptedExternalRefTypeArrayAspect : IInterceptPropertyAspect
    {
        public void OnGet(PropertyContext context)
        {
            context.Proceed();
            var ret = context.ReturnValue as SimpleClass[];

            if(ret.Length == 1 && ret[0].Value == 1)
                context.SetReturnValue(new[] { new SimpleClass(2), new SimpleClass(1) });
        }

        public void OnSet(PropertyContext context)
        {
            var arg = context.Arguments[0] as SimpleClass[];

            if(arg.Length == 1 && arg[0].Value == 3)
                context.Arguments.SetArgument(0, new[] { new SimpleClass(4), new SimpleClass(3) });

            context.Proceed();
        }
    }
}
