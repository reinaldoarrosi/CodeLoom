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
    public class InterceptedExternalValueTypeAspect : IInterceptPropertyAspect
    {
        public void OnGet(PropertyContext context)
        {
            context.Proceed();
            var ret = (SimpleStruct)context.ReturnValue;

            if (ret.Value == 1)
                context.SetReturnValue(new SimpleStruct(2));
        }

        public void OnSet(PropertyContext context)
        {
            var arg = (SimpleStruct)context.Arguments[0];

            if (arg.Value == 3)
                context.Arguments.SetArgument(0, new SimpleStruct(4));

            context.Proceed();
        }
    }
}
