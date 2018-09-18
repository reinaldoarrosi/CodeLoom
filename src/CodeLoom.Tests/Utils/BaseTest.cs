using Fody;
using CodeLoom.Fody;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mono.Cecil;

namespace CodeLoom.Fody.Tests
{
    public abstract class BaseTest
    {
        public static TestResult WeaveResult;

        static BaseTest()
        {
            var weaver = new ModuleWeaver();
            WeaveResult = weaver.ExecuteTestRun("TestAssembly.dll", beforeExecuteCallback: CopyRuntimeDLL, afterExecuteCallback: CopyRuntimeDLL);
        }

        private static void CopyRuntimeDLL(ModuleDefinition moduleDefinition)
        {
            if (!Directory.Exists("fodytemp")) Directory.CreateDirectory("fodytemp");
            File.Copy("CodeLoom.Runtime.dll", "fodytemp\\CodeLoom.Runtime.dll", true);
        }
    }
}
