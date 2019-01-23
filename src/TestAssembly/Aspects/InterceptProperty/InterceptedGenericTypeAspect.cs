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
    public class InterceptedGenericTypeAspect : IInterceptPropertyAspect
    {
        public void OnGet(PropertyContext context)
        {
            context.Proceed();
            var ret = context.ReturnValue as InheritsFromSimpleClass;

            if (ret.Value == 1)
                context.SetReturnValue(new InheritsFromSimpleClass(2));
        }

        public void OnSet(PropertyContext context)
        {
            var arg = context.Arguments[0] as InheritsFromSimpleClass;

            if (arg.Value == 3)
                context.Arguments.SetArgument(0, new InheritsFromSimpleClass(4));

            context.Proceed();
        }
    }
}
