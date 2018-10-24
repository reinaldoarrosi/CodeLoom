using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CodeLoom.Setup
{
    public class CodeLoomHelper
    {
        public static PropertyInfo GetPropertyInfo(Type declaringType, string name, MethodBase getMethod, MethodBase setMethod)
        {
            var bindingFlags = BindingFlags.Public | BindingFlags.NonPublic;

            if ((getMethod != null && getMethod.IsStatic) || (setMethod != null && setMethod.IsStatic))
                bindingFlags = bindingFlags | BindingFlags.Static;
            else
                bindingFlags = bindingFlags | BindingFlags.Instance;

            var properties = declaringType.GetProperties(bindingFlags);
            var property = properties.FirstOrDefault(p => p.Name == name && p.GetGetMethod(true) == getMethod && p.GetSetMethod(true) == setMethod);

            return property;
        }
    }
}
