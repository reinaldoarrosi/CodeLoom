using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CodeLoom
{
    public abstract class CodeLoomSetup
    {
        public class Empty : CodeLoomSetup
        { }

        public virtual bool ShouldWeaveType(Type type)
        {
            return false;
        }

        public virtual bool ShouldWeaveField(FieldInfo field)
        {
            return true;
        }

        public virtual bool ShouldWeaveMethod(MethodBase method)
        {
            return true;
        }

        public virtual bool ShouldWeaveProperty(PropertyInfo property)
        {
            return true;
        }

        public virtual bool ShouldWeavePropertyGetter(PropertyInfo property)
        {
            return true;
        }

        public virtual bool ShouldWeavePropertySetter(PropertyInfo property)
        {
            return true;
        }
    }
}
