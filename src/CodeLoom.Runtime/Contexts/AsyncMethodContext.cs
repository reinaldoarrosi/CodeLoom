using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CodeLoom.Contexts
{
    public class AsyncMethodContext
    {
        public AsyncMethodContext(object instance, MethodBase method, Arguments arguments)
        {
            Instance = instance;
            Method = method;
            Arguments = arguments;
        }

        public object Instance { get; private set; }
        public MethodBase Method { get; private set; }
        public Arguments Arguments { get; private set; }
        public object ReturnValue { get; private set; }

        internal Func<AsyncMethodContext, Task> ProceedDelegate { get; set; }

        public async Task Proceed()
        {
            await ProceedDelegate?.Invoke(this);
        }

        public void SetReturnValue(object value)
        {
            ReturnValue = value;
        }
    }
}
