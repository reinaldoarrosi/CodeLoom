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

        public virtual IEnumerable<InstanceAspect> GetInstanceAspects(Type type)
        {
            yield break;
        }
    }
}
