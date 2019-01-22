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
        private RuntimeTypeHandle _typeHandle;
        private RuntimeMethodHandle _methodHandle;
        private Lazy<PropertyInfo> _property;

        public PropertyContext(object instance, RuntimeTypeHandle typeHandle, RuntimeMethodHandle methodHandle, Arguments arguments)
        {
            _typeHandle = typeHandle;
            _methodHandle = methodHandle;
            _property = new Lazy<PropertyInfo>(GetPropertyFromHandles);
            
            Instance = instance;
            Arguments = arguments;
        }

        public object Instance { get; private set; }
        public Arguments Arguments { get; private set; }
        public object ReturnValue { get; private set; }
        public PropertyInfo Property { get { return _property.Value; } }

        internal Action<PropertyContext> ProceedDelegate { get; set; }

        public void Proceed()
        {
            ProceedDelegate?.Invoke(this);
        }

        public void SetReturnValue(object value)
        {
            ReturnValue = value;
        }

        private PropertyInfo GetPropertyFromHandles()
        {
            // gets the Type and MethodBase from the runtime handles
            var type = Type.GetTypeFromHandle(_typeHandle);
            var method = MethodBase.GetMethodFromHandle(_methodHandle, _typeHandle);

            // finds the name of the property from the name of the method
            // the methods for a property are always named "get_PROPERTY" and "set_PROPERTY"
            // so we find the "_" char and get the substring after this char to find the property name
            var index = method.Name.IndexOf('_');
            var propertyName = method.Name.Substring(index + 1);

            // finds a property that belongs to "type" and have the same name as "propertyName"
            // also checks if the getter or setter of this property is equal to "method"
            var bindingFlags = BindingFlags.Public | BindingFlags.NonPublic | (method.IsStatic ? BindingFlags.Static : BindingFlags.Instance);
            var property = type.GetProperties(bindingFlags).FirstOrDefault(p => p.Name == propertyName && (p.GetMethod == method || p.SetMethod == method));

            return property;
        }
    }
}
