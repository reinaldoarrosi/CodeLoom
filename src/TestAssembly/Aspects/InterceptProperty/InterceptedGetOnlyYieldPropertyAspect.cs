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
    public class InterceptedGetOnlyYieldPropertyAspect : IInterceptPropertyAspect
    {
        public void OnGet(PropertyContext context)
        {
            context.Proceed();

            var original = context.ReturnValue as IEnumerable<int>;
            context.SetReturnValue(original.Select(i => i + 5).Concat(new[] { 8 }));
        }

        public void OnSet(PropertyContext context)
        {
            context.Proceed();
        }
    }
}
