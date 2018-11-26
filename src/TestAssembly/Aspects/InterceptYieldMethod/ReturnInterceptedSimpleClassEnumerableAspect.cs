using CodeLoom.Aspects;
using CodeLoom.Contexts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using TestAssemblyReference;

namespace TestAssembly.Aspects.InterceptYieldMethod
{
    public class ReturnInterceptedSimpleClassEnumerableAspect : IInterceptMethodAspect
    {
        public void OnMethodInvoked(MethodContext context)
        {
            context.Proceed();

            var newEnumerable = CreateNewEnumerable(context.ReturnValue as IEnumerable<SimpleClass>);
            context.SetReturnValue(newEnumerable);
        }

        private IEnumerable<SimpleClass> CreateNewEnumerable(IEnumerable<SimpleClass> enumerable)
        {
            foreach (var item in enumerable)
            {
                yield return item;
            }

            yield return new SimpleClass(2);
        }
    }
}
