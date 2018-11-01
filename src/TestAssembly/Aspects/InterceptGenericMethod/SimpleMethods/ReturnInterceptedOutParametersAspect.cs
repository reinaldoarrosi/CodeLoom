using CodeLoom.Aspects;
using CodeLoom.Contexts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace TestAssembly.Aspects.InterceptGenericMethod.SimpleMethods
{
    public class ReturnInterceptedOutParametersAspect : InterceptMethodAspect
    {
        public override void OnMethodInvoked(MethodContext context)
        {
            context.Proceed();

            context.Arguments.SetArgument(0, 2);
            context.Arguments.SetArgument(1, "ab");
            context.Arguments.SetArgument(2, new int[] { 1, 2 });
            context.Arguments.SetArgument(3, new string[] { "ab", "cd" });
            context.Arguments.SetArgument(4, new List<int> { 1, 2 });
            context.Arguments.SetArgument(5, new List<string> { "ab", "cd" });
        }
    }
}