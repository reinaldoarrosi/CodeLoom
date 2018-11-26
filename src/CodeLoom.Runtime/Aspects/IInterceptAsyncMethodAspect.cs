using CodeLoom.Contexts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeLoom.Aspects
{
    public interface IInterceptAsyncMethodAspect
    {
        Task OnMethodInvoked(AsyncMethodContext context);
    }
}
