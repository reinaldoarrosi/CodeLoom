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
    public class MethodContext
    {
        private RuntimeTypeHandle _typeHandle;
        private RuntimeMethodHandle _methodHandle;
        private Lazy<MethodBase> _method;

        public MethodContext(object instance, RuntimeTypeHandle typeHandle, RuntimeMethodHandle methodHandle, Arguments arguments)
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

        internal Action<MethodContext> ProceedDelegate { get; set; }

        public void Proceed()
        {
            ProceedDelegate?.Invoke(this);
        }

        public void SetReturnValue(object value)
        {
            ReturnValue = value;
        }
    }
}
