using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestAssembly.Aspects;
using TestAssemblyReference;

namespace TestAssembly.ClassesToWeave
{
    public class InterceptStaticMethodsClass<T>
    {
        public static int ReturnOriginalIntValue(int a)
        {
            return a;
        }

        public static SimpleClass ReturnOriginalSimpleClassValue(SimpleClass a)
        {
            return a;
        }

        public static T ReturnOriginalTValue(T a)
        {
            return a;
        }

        public static T2 ReturnOriginalT2Value<T2>(T a, T2 b)
        {
            return b;
        }

        public static IEnumerable<T> ReturnOriginalTYieldEnumerable<T2>(T a, T2 b) where T2 : T
        {
            yield return a;
            yield return b;
        }

        public async static Task<T2> ReturnOriginalT2Async<T2>(T a, T2 b)
        {
            await Task.Delay(1);
            return b;
        }

        public static int ReturnInterceptedIntValue(int a)
        {
            return a;
        }

        public static SimpleClass ReturnInterceptedSimpleClassValue(SimpleClass a)
        {
            return a;
        }

        public static T ReturnInterceptedTValue(T a)
        {
            return a;
        }

        public static T2 ReturnInterceptedT2Value<T2>(T a, T2 b)
        {
            return b;
        }

        public static IEnumerable<T> ReturnInterceptedTYieldEnumerable<T2>(T a, T2 b) where T2 : T
        {
            yield return a;
            yield return b;
        }

        public async static Task<T2> ReturnInterceptedT2Async<T2>(T a, T2 b)
        {
            await Task.Delay(1);
            return b;
        }
    }
}
