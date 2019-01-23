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
    public class InterceptedExternalRefTypeAspect : IInterceptPropertyAspect
    {
        public void OnGet(PropertyContext context)
        {
            context.Proceed();
            var ret = context.ReturnValue as SimpleClass;

            if (ret.Value == 1)
                context.SetReturnValue(new SimpleClass(2));
        }

        public void OnSet(PropertyContext context)
        {
            var arg = context.Arguments[0] as SimpleClass;

            if (arg.Value == 3)
                context.Arguments.SetArgument(0, new SimpleClass(4));

            context.Proceed();
        }
    }
}
