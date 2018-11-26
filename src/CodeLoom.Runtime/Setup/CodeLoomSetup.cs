using CodeLoom.Aspects;
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

        public virtual IEnumerable<IImplementInterfaceAspect> GetTypeAspects(Type type)
        {
            yield break;
        }

        public virtual IEnumerable<IInterceptPropertyAspect> GetPropertyAspects(PropertyInfo property)
        {
            yield break;
        }

        public virtual IEnumerable<IInterceptMethodAspect> GetMethodAspects(MethodBase method)
        {
            yield break;
        }

        public virtual IEnumerable<IInterceptAsyncMethodAspect> GetAsyncMethodAspects(MethodBase method)
        {
            yield break;
        }
    }
}
