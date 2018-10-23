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

        public virtual IEnumerable<ImplementInterfaceAspect> GetAspects(Type type)
        {
            yield break;
        }

        public virtual IEnumerable<InterceptPropertyAspect> GetAspects(PropertyInfo property)
        {
            yield break;
        }

        public virtual IEnumerable<InterceptMethodAspect> GetAspects(MethodBase method)
        {
            yield break;
        }
    }
}
