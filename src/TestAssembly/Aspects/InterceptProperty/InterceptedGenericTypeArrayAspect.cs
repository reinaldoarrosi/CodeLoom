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
    public class InterceptedGenericTypeArrayAspect : IInterceptPropertyAspect
    {
        public void OnGet(PropertyContext context)
        {
            context.Proceed();
            var ret = context.ReturnValue as InheritsFromSimpleClass[];

            if (ret.Length == 1 && ret[0].Value == 1)
                context.SetReturnValue(new[] { new InheritsFromSimpleClass(2), new InheritsFromSimpleClass(1) });
        }

        public void OnSet(PropertyContext context)
        {
            var arg = context.Arguments[0] as InheritsFromSimpleClass[];

            if (arg.Length == 1 && arg[0].Value == 3)
                context.Arguments.SetArgument(0, new[] { new InheritsFromSimpleClass(4), new InheritsFromSimpleClass(3) });

            context.Proceed();
        }
    }
}
