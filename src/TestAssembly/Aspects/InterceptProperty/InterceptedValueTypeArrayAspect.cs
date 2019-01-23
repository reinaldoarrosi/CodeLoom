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
    public class InterceptedValueTypeArrayAspect : IInterceptPropertyAspect
    {
        public void OnGet(PropertyContext context)
        {
            context.Proceed();
            var ret = context.ReturnValue as int[];

            if (ret.Length == 1 && ret[0] == 1)
                context.SetReturnValue(new[] { 2, 1 });
        }

        public void OnSet(PropertyContext context)
        {
            var arg = context.Arguments[0] as int[];

            if (arg.Length == 1 && arg[0] == 3)
                context.Arguments.SetArgument(0, new[] { 4, 3 });

            context.Proceed();
        }
    }
}
