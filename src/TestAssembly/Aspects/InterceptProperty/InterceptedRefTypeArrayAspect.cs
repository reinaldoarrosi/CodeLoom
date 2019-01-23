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
    public class InterceptedRefTypeArrayAspect : IInterceptPropertyAspect
    {
        public void OnGet(PropertyContext context)
        {
            context.Proceed();
            var ret = context.ReturnValue as string[];

            if (ret.Length == 1 && ret[0] == "a")
                context.SetReturnValue(new[] { "cd", "ab" });
        }

        public void OnSet(PropertyContext context)
        {
            var arg = context.Arguments[0] as string[];

            if (arg.Length == 1 && arg[0] == "c")
                context.Arguments.SetArgument(0, new[] { "ca", "cb" });

            context.Proceed();
        }
    }
}
