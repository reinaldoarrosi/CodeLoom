using CodeLoom.Aspects;
using CodeLoom.Contexts;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeLoom.Bindings
{
    [DebuggerStepThrough]
    public abstract class MethodBinding
    {
        private Action<MethodContext> _runAction;

        public MethodBinding(IInterceptMethodAspect[] aspects)
        {
            Aspects = aspects;
            _runAction = CreateRunAction(aspects, 0);
        }

        public IInterceptMethodAspect[] Aspects { get; private set; }

        public void Run(MethodContext context)
        {
            _runAction(context);
        }

        internal protected abstract void Proceed(MethodContext context);

        private Action<MethodContext> CreateRunAction(IInterceptMethodAspect[] aspects, int index)
        {
            return (ctx) =>
            {
                ctx.ProceedDelegate = index < aspects.Length - 1 ? CreateRunAction(aspects, index + 1) : Proceed;
                aspects[index].OnMethodInvoked(ctx);
            };
        }
    }
}
