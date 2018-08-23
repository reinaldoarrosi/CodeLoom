using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CodeLoom.Core;

namespace CodeLoom.Aspects
{
    public abstract class BoundariesAspect : IAspect
    {
        public void Execute(Invocation invocation)
        {
            var context = new BoundariesAspectContext(invocation);

            try
            {
                OnEntry(context);
                if (context.Flow == BoundariesAspectContext.Flows.Return) return;
                if (context.Flow == BoundariesAspectContext.Flows.Throw && context.Exception != null) throw context.Exception;

                invocation.Proceed();

                OnExit(context);
                if (context.Flow == BoundariesAspectContext.Flows.Return) return;
                if (context.Flow == BoundariesAspectContext.Flows.Throw && context.Exception != null) throw context.Exception;
            }
            catch (Exception e)
            {
                context.Exception = e;
                OnException(context);

                if (context.Flow == BoundariesAspectContext.Flows.Return) return;
                if (context.Flow == BoundariesAspectContext.Flows.Throw && context.Exception != null) throw context.Exception;

                throw;
            }
        }

        public virtual void OnEntry(BoundariesAspectContext context) { }
        public virtual void OnExit(BoundariesAspectContext context) { }
        public virtual void OnException(BoundariesAspectContext context) { }
    }
}
