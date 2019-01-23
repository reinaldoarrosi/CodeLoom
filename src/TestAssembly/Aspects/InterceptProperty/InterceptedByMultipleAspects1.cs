using CodeLoom.Aspects;
using CodeLoom.Contexts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestAssemblyReference;

namespace TestAssembly.Aspects.InterceptProperty
{
    public class InterceptedByMultipleAspects1 : IInterceptPropertyAspect
    {
        public void OnGet(PropertyContext context)
        {
            context.Proceed();
            context.SetReturnValue(((int)context.ReturnValue) + 1);
        }

        public void OnSet(PropertyContext context)
        {
            var arg0 = (int)context.Arguments[0];
            context.Arguments.SetArgument(0, arg0 + 5);
            context.Proceed();
        }
    }
}
