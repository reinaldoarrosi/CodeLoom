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
    public class InterceptedValueTypeListAspect : IInterceptPropertyAspect
    {
        public void OnGet(PropertyContext context)
        {
            context.Proceed();
            var ret = context.ReturnValue as List<int>;

            if (ret.Count == 1 && ret[0] == 1)
                context.SetReturnValue(new List<int> { 2, 1 });
        }

        public void OnSet(PropertyContext context)
        {
            var arg = context.Arguments[0] as List<int>;

            if (arg.Count == 1 && arg[0] == 3)
                context.Arguments.SetArgument(0, new List<int> { 4, 3 });

            context.Proceed();
        }
    }
}
