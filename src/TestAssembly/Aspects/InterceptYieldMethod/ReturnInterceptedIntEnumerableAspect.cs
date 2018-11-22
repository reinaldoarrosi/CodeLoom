using CodeLoom.Aspects;
using CodeLoom.Contexts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace TestAssembly.Aspects.InterceptYieldMethod
{
    public class ReturnInterceptedIntEnumerableAspect : InterceptMethodAspect
    {
        public override void OnMethodInvoked(MethodContext context)
        {
            context.Proceed();

            var newEnumerable = CreateNewEnumerable(context.ReturnValue as IEnumerable<int>);
            context.SetReturnValue(newEnumerable);
        }

        private IEnumerable<int> CreateNewEnumerable(IEnumerable<int> enumerable)
        {
            foreach (var item in enumerable)
            {
                yield return item;
            }

            yield return 2;
        }
    }
}
