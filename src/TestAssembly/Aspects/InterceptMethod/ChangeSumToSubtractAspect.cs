using CodeLoom.Aspects;
using CodeLoom.Contexts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace TestAssembly.Aspects.InterceptMethod
{
    public class ChangeSumToSubtractAspect : InterceptMethodAspect
    {
        public override void OnMethodInvoked(MethodContext context)
        {
            var a = (int)context.Arguments.GetArgument(0);
            var b = (int)context.Arguments.GetArgument(1);
            context.SetReturnValue(a - b);
        }
    }
}