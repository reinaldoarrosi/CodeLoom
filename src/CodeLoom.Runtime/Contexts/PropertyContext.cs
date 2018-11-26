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
    public class PropertyContext
    {
        public PropertyContext(object instance, PropertyInfo property, Arguments arguments)
        {
            Instance = instance;
            Property = property;
            Arguments = arguments;
        }

        public object Instance { get; private set; }
        public PropertyInfo Property { get; private set; }
        public Arguments Arguments { get; private set; }
        public object ReturnValue { get; private set; }

        internal Action<PropertyContext> ProceedDelegate { get; set; }

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
