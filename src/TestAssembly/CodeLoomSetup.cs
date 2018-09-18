using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CodeLoom.Aspects;
using TestAssembly.Aspects;

[assembly:CodeLoom.CodeLoomSetup(typeof(TestAssembly.CodeLoomSetup))]

namespace TestAssembly
{
    public class CodeLoomSetup : CodeLoom.CodeLoomSetup
    {
        public override IEnumerable<InstanceAspect> GetInstanceAspects(Type type)
        {
            if (type == typeof(TestClass))
                yield return new TestInstanceAspect<double>();
        }
    }
}
