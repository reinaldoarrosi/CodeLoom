using CodeLoom.Aspects;
using CodeLoom.Contexts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace TestAssembly.Aspects.InterceptProperty
{
    public class InterceptedRefTypeAspect : IInterceptPropertyAspect
    {
        public void OnGet(PropertyContext context)
        {
            context.Proceed();
            var ret = context.ReturnValue as string;

            if (ret == "a")
                context.SetReturnValue("ab");
        }

        public void OnSet(PropertyContext context)
        {
            var arg = context.Arguments[0] as string;

            if (arg == "c")
                context.Arguments.SetArgument(0, "ca");

            context.Proceed();
        }
    }
}
