using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
[assembly: CodeLoom.CodeLoomSetup(typeof(TestAssembly.CodeLoomSetup))]

namespace TestAssembly
{
    public class CodeLoomSetup : CodeLoom.CodeLoomSetup
    {
        public override bool ShouldWeaveType(Type type)
        {
            return type == typeof(TestClass);
        }
    }
}
