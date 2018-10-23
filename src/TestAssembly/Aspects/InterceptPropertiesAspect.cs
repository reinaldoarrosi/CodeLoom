using CodeLoom.Aspects;
using CodeLoom.Contexts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestAssembly.Aspects
{
    public class InterceptPropertiesAspect : InterceptPropertyAspect
    {
        public override void OnGet(PropertyContext context)
        {
            context.Proceed();
        }

        public override void OnSet(PropertyContext context)
        {
            context.Proceed();
        }
    }
}
