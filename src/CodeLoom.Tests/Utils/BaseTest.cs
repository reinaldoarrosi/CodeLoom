using Fody;
using CodeLoom.Fody;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mono.Cecil;
using System.Reflection;
using System.Threading;

namespace CodeLoom.Tests
{
    public class AppDomainDelegate : MarshalByRefObject
    {
        public void Execute(Action action)
        {
            action();
        }

        public void Execute<T>(T parameter, Action<T> action)
        {
            action(parameter);
        }

        public T Execute<T>(Func<T> action)
        {
            return action();
        }

        public TResult Execute<T, TResult>(T parameter, Func<T, TResult> action)
        {
            return action(parameter);
        }
    }

    public abstract class BaseTest
    {
        private static TestResult _weaveResult;
        private static AppDomain _appDomain;        

        static BaseTest()
        {
            var weaver = new ModuleWeaver();
            _weaveResult = weaver.ExecuteTestRun("TestAssembly.dll", afterExecuteCallback: CopyDLLs);
            _appDomain = CreateAppDomain(AppDomain.CurrentDomain.SetupInformation.ApplicationBase);
        }

        public static void Execute(Action action)
        {
            var domainDelegate = (AppDomainDelegate)_appDomain.CreateInstanceAndUnwrap(
                typeof(AppDomainDelegate).Assembly.FullName,
                typeof(AppDomainDelegate).FullName
            );

            domainDelegate.Execute(action);
        }

        public static TestResult GetTestResult()
        {
            return _weaveResult;
        }

        private static AppDomain CreateAppDomain(string appBase)
        {
            var appDomainSetup = new AppDomainSetup();
            appDomainSetup.ApplicationBase = appBase + "/fodytemp";

            var appDomain = AppDomain.CreateDomain("CodeLoom.Tests.AppDomain", null, appDomainSetup);

            return appDomain;
        }

        private static void CopyDLLs(ModuleDefinition moduleDefinition)
        {
            if (!Directory.Exists("fodytemp")) Directory.CreateDirectory("fodytemp");

            foreach (var item in Directory.GetFiles(".", "*.*"))
            {
                if (Path.GetFileName(item) == "TestAssembly.dll") continue;
                File.Copy(item, $"./fodytemp/{Path.GetFileName(item)}", true);
            }
        }
    }
}
