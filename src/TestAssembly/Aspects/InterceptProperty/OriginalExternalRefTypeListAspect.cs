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
    public class OriginalExternalRefTypeListAspect : IInterceptPropertyAspect
    {
        public void OnGet(PropertyContext context)
        {
            context.Proceed();
        }

        public void OnSet(PropertyContext context)
        {
            context.Proceed();
        }
    }
}
