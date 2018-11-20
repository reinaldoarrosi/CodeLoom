using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestAssembly.Aspects;
using TestAssemblyReference;

namespace TestAssembly.ClassesToWeave
{
    public class InterceptAsyncMethodsClass
    {
        public async Task<int> ReturnOriginalValue() { await Task.Delay(1); return 1; }

        public async Task<int[]> ReturnOriginalValueArray() { await Task.Delay(1); return new [] { 1 }; }

        public async Task<List<int>> ReturnOriginalValueList() { await Task.Delay(1); return new List<int> { 1 }; }



        public async Task<int> ReturnInterceptedValue() { await Task.Delay(1); return 1; }

        public async Task<int[]> ReturnInterceptedValueArray() { await Task.Delay(1); return new[] { 1 }; }

        public async Task<List<int>> ReturnInterceptedValueList() { await Task.Delay(1); return new List<int> { 1 }; }


        public async Task<T> ReturnOriginalGenericValue<T>(T a) { await Task.Delay(1); return a; }

        public async Task<T> ReturnInterceptedGenericValue<T>(T a) { await Task.Delay(1); return a; }

        public async Task<T[]> ReturnOriginalGenericValueArray<T>(T[] a) { await Task.Delay(1); return a; }

        public async Task<T[]> ReturnInterceptedGenericValueArray<T>(T[] a) { await Task.Delay(1); return a; }

        public async Task<List<T>> ReturnOriginalGenericValueList<T>(List<T> a) { await Task.Delay(1); return a; }

        public async Task<List<T>> ReturnInterceptedGenericValueList<T>(List<T> a) { await Task.Delay(1); return a; }


        public async Task<string> ReturnOriginalParametersAsString(int a, string b, SimpleStruct c, SimpleClass d, int[] e, string[] f, SimpleStruct[] g, SimpleClass[] h, List<int> i, List<string> j, List<SimpleStruct> k, List<SimpleClass> l)
        {
            await Task.Delay(1);
            return Helper.AsString(a, b, c, d, e, f, g, h, i, j, k, l);
        }

        public async Task<string> ReturnInterceptedParametersAsString(int a, string b, SimpleStruct c, SimpleClass d, int[] e, string[] f, SimpleStruct[] g, SimpleClass[] h, List<int> i, List<string> j, List<SimpleStruct> k, List<SimpleClass> l)
        {
            await Task.Delay(1);
            return Helper.AsString(a, b, c, d, e, f, g, h, i, j, k, l);
        }

        public async Task InterceptAsyncMethodThatReturnsTask()
        {
            await Task.Delay(100);
        }

        public async void InterceptAsyncMethodThatReturnsVoid()
        {
            await Task.Delay(100);
        }
    }
}
