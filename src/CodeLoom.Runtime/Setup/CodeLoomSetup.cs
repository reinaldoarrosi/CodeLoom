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

        public virtual IEnumerable<IImplementInterfaceAspect> GetImplementInterfaceAspects(Type type)
        {
            yield break;
        }

        public virtual IEnumerable<IInterceptPropertyAspect> GetInterceptPropertyAspects(PropertyInfo property)
        {
            yield break;
        }

        public virtual IEnumerable<IInterceptMethodAspect> GetInterceptMethodAspects(MethodBase method)
        {
            yield break;
        }

        public virtual IEnumerable<IInterceptAsyncMethodAspect> GetInterceptAsyncMethodAspects(MethodBase method)
        {
            yield break;
        }
    }
}
