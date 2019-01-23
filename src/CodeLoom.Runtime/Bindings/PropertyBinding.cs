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
    public abstract class PropertyBinding
    {
        private Action<PropertyContext> _runGetterAction;
        private Action<PropertyContext> _runSetterAction;

        public PropertyBinding(IInterceptPropertyAspect[] aspects)
        {
            Aspects = aspects;
            _runGetterAction = CreateRunGetterAction(aspects, 0);
            _runSetterAction = CreateRunSetterAction(aspects, 0);
        }

        public IInterceptPropertyAspect[] Aspects { get; private set; }

        public void RunGetter(PropertyContext context)
        {
            _runGetterAction(context);
        }

        public void RunSetter(PropertyContext context)
        {
            _runSetterAction(context);
        }

        internal protected virtual void ProceedGet(PropertyContext context)
        { }

        internal protected virtual void ProceedSet(PropertyContext context)
        { }

        private Action<PropertyContext> CreateRunGetterAction(IInterceptPropertyAspect[] aspects, int index)
        {
            return (ctx) =>
            {
                ctx.ProceedDelegate = index < aspects.Length - 1 ? CreateRunGetterAction(aspects, index + 1) : ProceedGet;
                aspects[index].OnGet(ctx);
            };
        }

        private Action<PropertyContext> CreateRunSetterAction(IInterceptPropertyAspect[] aspects, int index)
        {
            return (ctx) =>
            {
                ctx.ProceedDelegate = index < aspects.Length - 1 ? CreateRunSetterAction(aspects, index + 1) : ProceedSet;
                aspects[index].OnSet(ctx);
            };
        }
    }
}
