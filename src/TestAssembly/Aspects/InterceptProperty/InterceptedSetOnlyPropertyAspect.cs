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
    public class InterceptedSetOnlyPropertyAspect : IInterceptPropertyAspect
    {
        public void OnGet(PropertyContext context)
        {
            context.Proceed();
        }

        public void OnSet(PropertyContext context)
        {
            context.Arguments.SetArgument(0, ((int)context.Arguments[0]) + 1);
            context.Proceed();
        }
    }
}
