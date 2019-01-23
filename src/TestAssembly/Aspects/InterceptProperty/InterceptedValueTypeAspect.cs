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
    public class InterceptedValueTypeAspect : IInterceptPropertyAspect
    {
        public void OnGet(PropertyContext context)
        {
            context.Proceed();
            var ret = (int)context.ReturnValue;

            if (ret == 1)
                context.SetReturnValue(2);
        }

        public void OnSet(PropertyContext context)
        {
            var arg = (int)context.Arguments[0];

            if (arg == 3)
                context.Arguments.SetArgument(0, 4);

            context.Proceed();
        }
    }
}
