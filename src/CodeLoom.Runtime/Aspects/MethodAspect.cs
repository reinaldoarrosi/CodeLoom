using CodeLoom.Contexts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeLoom.Aspects
{
    public abstract class MethodAspect
    {
        protected abstract void OnMethodInvoked(MethodContext context);
    }
}
