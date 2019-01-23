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
    public class InterceptedRefTypeListAspect : IInterceptPropertyAspect
    {
        public void OnGet(PropertyContext context)
        {
            context.Proceed();
            var ret = context.ReturnValue as List<string>;

            if (ret.Count == 1 && ret[0] == "a")
                context.SetReturnValue(new List<string> { "cd", "ab" });
        }

        public void OnSet(PropertyContext context)
        {
            var arg = context.Arguments[0] as List<string>;

            if (arg.Count == 1 && arg[0] == "c")
                context.Arguments.SetArgument(0, new List<string> { "ca", "cb" });

            context.Proceed();
        }
    }
}
