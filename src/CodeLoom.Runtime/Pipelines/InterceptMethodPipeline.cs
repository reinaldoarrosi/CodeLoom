using CodeLoom.Aspects;
using CodeLoom.Contexts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeLoom.Pipelines
{
    public class InterceptMethodPipeline
    {
        public InterceptMethodPipeline(InterceptMethodAspect[] aspects, Action<MethodContext> proceed)
        {
            Aspects = aspects;
            ProceedDelegate = CreateProceedDelegate(aspects, proceed, 0);
        }

        public InterceptMethodAspect[] Aspects { get; private set; }
        public Action<MethodContext> ProceedDelegate { get; private set; }

        public void Run(MethodContext context)
        {
            ProceedDelegate(context);
        }

        private Action<MethodContext> CreateProceedDelegate(InterceptMethodAspect[] aspects, Action<MethodContext> proceed, int index)
        {
            return (ctx) =>
            {
                ctx.ProceedDelegate = index < aspects.Length - 1 ? CreateProceedDelegate(aspects, proceed, index + 1) : proceed;
                aspects[index].OnMethodInvoked(ctx);
            };
        }
    }
}
