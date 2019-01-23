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
    public class InterceptedExternalValueTypeArrayAspect : IInterceptPropertyAspect
    {
        public void OnGet(PropertyContext context)
        {
            context.Proceed();
            var ret = context.ReturnValue as SimpleStruct[];

            if (ret.Length == 1 && ret[0].Value == 1)
                context.SetReturnValue(new[] { new SimpleStruct(2), new SimpleStruct(1) });
        }

        public void OnSet(PropertyContext context)
        {
            var arg = context.Arguments[0] as SimpleStruct[];

            if (arg.Length == 1 && arg[0].Value == 3)
                context.Arguments.SetArgument(0, new[] { new SimpleStruct(4), new SimpleStruct(3) });

            context.Proceed();
        }
    }
}
