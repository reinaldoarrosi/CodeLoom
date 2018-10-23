using CodeLoom.Contexts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeLoom.Aspects
{
    public abstract class InterceptPropertyAspect
    {
        public abstract void OnGet(PropertyContext context);

        public abstract void OnSet(PropertyContext context);
    }
}
