using CodeLoom.Aspects;
using CodeLoom.Contexts;
using System;
using System.Threading.Tasks;

namespace CodeLoom.Bindings
{
    public abstract class AsyncMethodBinding
    {
        public class ProceedContinuation
        {
            private AsyncMethodContext _context;
            
            public ProceedContinuation(AsyncMethodContext context)
            {
                _context = context;
            }

            public void Continue<T>(Task<T> task)
            {
                if (task.Status == TaskStatus.RanToCompletion)
                    _context.SetReturnValue(task.Result);
                else if (task.Status == TaskStatus.Canceled)
                    throw new TaskCanceledException();
                else if (task.Status == TaskStatus.Faulted)
                    throw task.Exception.InnerException;
            }
        }

        public class RunContinuation
        {
            private AsyncMethodContext _context;

            public RunContinuation(AsyncMethodContext context)
            {
                _context = context;
            }

            public T Continue<T>(Task task)
            {
                if (task.Status == TaskStatus.RanToCompletion)
                    return (T)_context.ReturnValue;
                else if (task.Status == TaskStatus.Canceled)
                    throw new TaskCanceledException();
                else if (task.Status == TaskStatus.Faulted)
                    throw task.Exception.InnerException;
                else
                    return default(T);
            }
        }

        private Func<AsyncMethodContext, Task> _runAction;

        public AsyncMethodBinding(InterceptAsyncMethodAspect[] aspects)
        {
            Aspects = aspects;
            _runAction = CreateRunAction(aspects, 0);
        }

        public InterceptAsyncMethodAspect[] Aspects { get; private set; }

        public Task Run(AsyncMethodContext context)
        {
            return _runAction(context);
        }

        internal protected abstract Task Proceed(AsyncMethodContext context);

        private Func<AsyncMethodContext, Task> CreateRunAction(InterceptAsyncMethodAspect[] aspects, int index)
        {
            return async (ctx) =>
            {
                ctx.ProceedDelegate = index < aspects.Length - 1 ? CreateRunAction(aspects, index + 1) : Proceed;
                await aspects[index].OnMethodInvoked(ctx);
            };
        }
    }
}
