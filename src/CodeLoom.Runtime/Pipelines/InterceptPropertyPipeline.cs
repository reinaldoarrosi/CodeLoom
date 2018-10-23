using CodeLoom.Aspects;
using CodeLoom.Contexts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeLoom.Pipelines
{
    public class InterceptPropertyPipeline
    {
        public InterceptPropertyPipeline(InterceptPropertyAspect[] aspects, Action<PropertyContext> proceedGetter, Action<PropertyContext> proceedSetter)
        {
            Aspects = aspects;
            GetterDelegate = CreateGetterDelegate(aspects, proceedGetter, 0);
            SetterDelegate = CreateSetterDelegate(aspects, proceedSetter, 0);
        }

        public InterceptPropertyAspect[] Aspects { get; private set; }
        public Action<PropertyContext> GetterDelegate { get; private set; }
        public Action<PropertyContext> SetterDelegate { get; private set; }

        public void RunGetter(PropertyContext context)
        {
            GetterDelegate(context);
        }

        public void RunSetter(PropertyContext context)
        {
            SetterDelegate(context);
        }

        private Action<PropertyContext> CreateGetterDelegate(InterceptPropertyAspect[] aspects, Action<PropertyContext> proceedGetter, int index)
        {
            return (ctx) =>
            {
                ctx.ProceedDelegate = index < aspects.Length - 1 ? CreateGetterDelegate(aspects, proceedGetter, index + 1) : proceedGetter;
                aspects[index].OnGet(ctx);
            };
        }

        private Action<PropertyContext> CreateSetterDelegate(InterceptPropertyAspect[] aspects, Action<PropertyContext> proceedSetter, int index)
        {
            return (ctx) =>
            {
                ctx.ProceedDelegate = index < aspects.Length - 1 ? CreateSetterDelegate(aspects, proceedSetter, index + 1) : proceedSetter;
                aspects[index].OnSet(ctx);
            };
        }
    }
}
