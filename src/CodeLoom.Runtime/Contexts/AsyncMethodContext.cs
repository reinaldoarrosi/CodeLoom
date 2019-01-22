using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CodeLoom.Contexts
{
    [DebuggerStepThrough]
    public class AsyncMethodContext
    {
        private RuntimeTypeHandle _typeHandle;
        private RuntimeMethodHandle _methodHandle;
        private Lazy<MethodBase> _method;

        public AsyncMethodContext(object instance, RuntimeTypeHandle typeHandle, RuntimeMethodHandle methodHandle, Arguments arguments)
        {
            _typeHandle = typeHandle;
            _methodHandle = methodHandle;
            _method = new Lazy<MethodBase>(() => MethodBase.GetMethodFromHandle(_methodHandle, _typeHandle));
                   
            Instance = instance;
            Arguments = arguments;
        }

        public object Instance { get; private set; }
        public Arguments Arguments { get; private set; }
        public object ReturnValue { get; private set; }
        public MethodBase Method { get { return _method.Value; } }

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
