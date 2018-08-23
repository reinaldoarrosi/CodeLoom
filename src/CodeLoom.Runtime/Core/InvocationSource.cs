using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CodeLoom.Core
{
    public class InvocationSource
    {
        private static readonly string PROPERTY_GETTER_METHOD_PREFIX = "get_";
        private static readonly string PROPERTY_SETTER_METHOD_PREFIX = "set_";

        private Lazy<PropertyInfo> _propertyInfoCache;

        public InvocationSource(MethodBase method, object instance)
        {
            _propertyInfoCache = new Lazy<PropertyInfo>(GetPropertyFromMethod);

            Method = method;
            Instance = instance;
        }

        public Type Type { get { return Method.DeclaringType; } }
        public PropertyInfo Property { get { return _propertyInfoCache.Value; } }
        public MethodBase Method { get; private set; }
        public Type ReturnType { get { return (Method as MethodInfo)?.ReturnType; } }
        public bool IsPropertyGetter { get { return Method.IsSpecialName && Method.Name.StartsWith(PROPERTY_GETTER_METHOD_PREFIX); } }
        public bool IsPropertySetter { get { return Method.IsSpecialName && Method.Name.StartsWith(PROPERTY_SETTER_METHOD_PREFIX); } }
        public bool IsProperty { get { return IsPropertyGetter || IsPropertySetter; } }
        public bool IsConstructor { get { return Method.IsConstructor; } }
        public bool HasReturnValue { get { return ReturnType != null && ReturnType != typeof(void); } }
        public object Instance { get; private set; }

        private PropertyInfo GetPropertyFromMethod()
        {
            string propertyName = null;
            if (IsPropertyGetter) propertyName = Method.Name.Substring(PROPERTY_GETTER_METHOD_PREFIX.Length);
            if (IsPropertySetter) propertyName = Method.Name.Substring(PROPERTY_SETTER_METHOD_PREFIX.Length);
            if (propertyName == null) return null;

            var propertyInfo = Type.GetProperty(
                propertyName,
                BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic,
                null,
                (Method as MethodInfo)?.ReturnType,
                Method.GetParameters().Select(p => p.ParameterType).ToArray(),
                null
            );

            return propertyInfo;
        }
    }
}
