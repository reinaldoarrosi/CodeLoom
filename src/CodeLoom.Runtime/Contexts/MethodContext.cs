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
        public MethodContext(object instance, MethodBase method, Arguments arguments)
        {
            Instance = instance;
            Method = method;
            Arguments = arguments;
        }

        public object Instance { get; private set; }
        public MethodBase Method { get; private set; }
        public Arguments Arguments { get; private set; }
        public object ReturnValue { get; private set; }

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
