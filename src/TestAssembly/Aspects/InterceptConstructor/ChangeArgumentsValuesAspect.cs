using CodeLoom.Aspects;
using CodeLoom.Contexts;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using TestAssembly.ClassesToWeave;
using TestAssemblyReference;

namespace TestAssembly.Aspects.InterceptConstructor
{
    public class ChangeArgumentsValuesAspect : InterceptMethodAspect
    {
        public override void OnMethodInvoked(MethodContext context)
        {
            if(context.Instance.GetType() == typeof(InterceptConstructorClass<DateTime>))
            {
                var parameters = context.Method.GetParameters();

                if (parameters.Length > 1)
                {
                    context.Arguments.SetArgument(0, 2);
                    context.Arguments.SetArgument(1, new SimpleClass(2));
                    context.Arguments.SetArgument(2, DateTime.Today.AddDays(-1));
                    context.Arguments.SetArgument(3, new[] { 1, 2 });
                    context.Arguments.SetArgument(4, new[] { new SimpleClass(1), new SimpleClass(2) });
                    context.Arguments.SetArgument(5, new[] { DateTime.Today.AddDays(-1), DateTime.Today.AddDays(-2) });
                    context.Arguments.SetArgument(6, new List<int> { 1, 2 });
                    context.Arguments.SetArgument(7, new List<SimpleClass> { new SimpleClass(1), new SimpleClass(2) });
                    context.Arguments.SetArgument(8, new List<DateTime> { DateTime.Today.AddDays(-1), DateTime.Today.AddDays(-2) });
                }
                else if(parameters[0].ParameterType == typeof(int))
                {
                    context.Arguments.SetArgument(0, 2);
                }
                else if (parameters[0].ParameterType == typeof(SimpleClass))
                {
                    context.Arguments.SetArgument(0, new SimpleClass(2));
                }

            }
            else if (context.Instance.GetType() == typeof(InterceptConstructorClass<DateTime>.InnerClass<string>))
            {
                context.Arguments.SetArgument(0, DateTime.Today.AddDays(-1));
                context.Arguments.SetArgument(1, "b");
                context.Arguments.SetArgument(2, new[] { DateTime.Today.AddDays(-1), DateTime.Today.AddDays(-2) });
                context.Arguments.SetArgument(3, new[] { "a", "b" });
                context.Arguments.SetArgument(4, new List<DateTime> { DateTime.Today.AddDays(-1), DateTime.Today.AddDays(-2) });
                context.Arguments.SetArgument(5, new List<string> { "a", "b" });
            }

            context.Proceed();
        }
    }
}