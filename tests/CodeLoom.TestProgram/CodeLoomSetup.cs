using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
[assembly: CodeLoom.CodeLoomSetup(typeof(CodeLoom.TestProgram.CodeLoomSetup))]

namespace CodeLoom.TestProgram
{
    public class CodeLoomSetup : global::CodeLoom.CodeLoomSetup
    {
        public override bool ShouldWeaveType(Type type)
        {
            return type == typeof(TestClass);
        }
    }
}
