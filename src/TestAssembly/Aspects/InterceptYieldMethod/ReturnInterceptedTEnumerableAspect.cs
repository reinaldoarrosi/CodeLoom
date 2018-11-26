using CodeLoom.Aspects;
using CodeLoom.Contexts;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace TestAssembly.Aspects.InterceptYieldMethod
{
    public class ReturnInterceptedTEnumerableAspect : IInterceptMethodAspect
    {
        public void OnMethodInvoked(MethodContext context)
        {
            context.Proceed();

            var newEnumerable = CreateNewEnumerable(context.ReturnValue as IEnumerable<string>);
            context.SetReturnValue(newEnumerable);
        }

        private IEnumerable<string> CreateNewEnumerable(IEnumerable<string> enumerable)
        {
            foreach (var item in enumerable)
            {
                yield return item;
            }

            yield return "b";
        }
    }
}
