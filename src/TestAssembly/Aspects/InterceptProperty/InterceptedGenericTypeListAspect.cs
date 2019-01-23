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
    public class InterceptedGenericTypeListAspect : IInterceptPropertyAspect
    {
        public void OnGet(PropertyContext context)
        {
            context.Proceed();
            var ret = context.ReturnValue as List<InheritsFromSimpleClass>;

            if (ret.Count == 1 && ret[0].Value == 1)
                context.SetReturnValue(new List<InheritsFromSimpleClass> { new InheritsFromSimpleClass(2), new InheritsFromSimpleClass(1) });
        }

        public void OnSet(PropertyContext context)
        {
            var arg = context.Arguments[0] as List<InheritsFromSimpleClass>;

            if (arg.Count == 1 && arg[0].Value == 3)
                context.Arguments.SetArgument(0, new List<InheritsFromSimpleClass> { new InheritsFromSimpleClass(4), new InheritsFromSimpleClass(3) });

            context.Proceed();
        }
    }
}
