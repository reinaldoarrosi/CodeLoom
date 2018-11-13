using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestAssembly.Aspects;
using TestAssemblyReference;

namespace TestAssembly.ClassesToWeave
{
    public class InterceptGenericMethodsClass<T1, T2>
    {
        /*
        #region Simple methods
        public T1 ReturnOriginalValueFromFirstParameter(T1 a, T2 b)
        {
            return a;
        }

        public T1 ReturnInterceptedValueFromFirstParameter(T1 a, T2 b)
        {
            return a;
        }

        public string ReturnOriginalParametersAsString(T1 a, T2 b)
        {
            return Helper.AsString(a, b);
        }

        public string ReturnInterceptedParametersAsString(T1 a, T2 b)
        {
            return Helper.AsString(a, b);
        }

        public string ReturnOriginalRefParametersAsString(ref T1 a, ref T2 b)
        {
            return Helper.AsString(a, b);
        }

        public string ReturnInterceptedRefParametersAsString(ref T1 a, ref T2 b)
        {
            return Helper.AsString(a, b);
        }

        public void ReturnOriginalOutParameters(out T1 a, out T2 b, out T1[] c, out T2[] d, out List<T1> e, out List<T2> f)
        {
            a = default(T1);
            b = default(T2);
            c = new[] { default(T1) };
            d = new[] { default(T2) };
            e = new List<T1> { default(T1) };
            f = new List<T2> { default(T2) };
        }

        public void ReturnInterceptedOutParameters(out T1 a, out T2 b, out T1[] c, out T2[] d, out List<T1> e, out List<T2> f)
        {
            a = default(T1);
            b = default(T2);
            c = new[] { default(T1) };
            d = new[] { default(T2) };
            e = new List<T1> { default(T1) };
            f = new List<T2> { default(T2) };
        }
        #endregion

        #region Generic methods
        public T1 GenericReturnOriginalValueFromFirstParameter<T3, T4>(T1 a, T2 b, T3 c, T4 d)
        {
            return a;
        }

        public T1 GenericReturnInterceptedValueFromFirstParameter<T3, T4>(T1 a, T2 b, T3 c, T4 d)
        {
            return a;
        }

        public string GenericReturnOriginalParametersAsString<T3, T4>(T1 a, T2 b, T3 c, T4 d)
        {
            return Helper.AsString(a, b, c, d);
        }

        public string GenericReturnInterceptedParametersAsString<T3, T4>(T1 a, T2 b, T3 c, T4 d)
        {
            return Helper.AsString(a, b, c, d);
        }

        public string GenericReturnOriginalRefParametersAsString<T3, T4>(ref T1 a, ref T2 b, ref T3 c, ref T4 d)
        {
            return Helper.AsString(a, b, c, d);
        }

        public string GenericReturnInterceptedRefParametersAsString<T3, T4>(ref T1 a, ref T2 b, ref T3 c, ref T4 d)
        {
            return Helper.AsString(a, b, c, d);
        }

        public void GenericReturnOriginalOutParameters<T3, T4>(out T1 a, out T2 b, out T3 c, out T4 d, out T1[] e, out T2[] f, out T3[] g, out T4[] h, out List<T1> i, out List<T2> j, out List<T3> k, out List<T4> l)
        {
            a = default(T1);
            b = default(T2);
            c = default(T3);
            d = default(T4);
            e = new[] { default(T1) };
            f = new[] { default(T2) };
            g = new[] { default(T3) };
            h = new[] { default(T4) };
            i = new List<T1> { default(T1) };
            j = new List<T2> { default(T2) };
            k = new List<T3> { default(T3) };
            l = new List<T4> { default(T4) };
        }

        public void GenericReturnInterceptedOutParameters<T3, T4>(out T1 a, out T2 b, out T3 c, out T4 d, out T1[] e, out T2[] f, out T3[] g, out T4[] h, out List<T1> i, out List<T2> j, out List<T3> k, out List<T4> l)
        {
            a = default(T1);
            b = default(T2);
            c = default(T3);
            d = default(T4);
            e = new[] { default(T1) };
            f = new[] { default(T2) };
            g = new[] { default(T3) };
            h = new[] { default(T4) };
            i = new List<T1> { default(T1) };
            j = new List<T2> { default(T2) };
            k = new List<T3> { default(T3) };
            l = new List<T4> { default(T4) };
        }
        #endregion
        */

        #region Generic methods with repeated generic parameter
        public T1 GenericWithRepeatedGenericParameterReturnOriginalValueFromFirstParameter<T3, T2>(T1 a, T2 b, T3 c)
        {
            return a;
        }

        /*
        public T1 GenericWithRepeatedGenericParameterReturnInterceptedValueFromFirstParameter<T3, T2>(T1 a, T2 b, T3 c)
        {
            return a;
        }

        public string GenericWithRepeatedGenericParameterReturnOriginalParametersAsString<T3, T2>(T1 a, T2 b, T3 c)
        {
            return Helper.AsString(a, b);
        }

        public string GenericWithRepeatedGenericParameterReturnInterceptedParametersAsString<T3, T2>(T1 a, T2 b, T3 c)
        {
            return Helper.AsString(a, b);
        }

        public string GenericWithRepeatedGenericParameterReturnOriginalRefParametersAsString<T3, T2>(ref T1 a, ref T2 b, ref T3 c)
        {
            return Helper.AsString(a, b);
        }

        public string GenericWithRepeatedGenericParameterReturnInterceptedRefParametersAsString<T3, T2>(ref T1 a, ref T2 b, ref T3 c)
        {
            return Helper.AsString(a, b);
        }

        public void GenericWithRepeatedGenericParameterReturnOriginalOutParameters<T3, T2>(out T1 a, out T2 b, out T3 c, out T1[] d, out T2[] e, out T3[] f, out List<T1> g, out List<T2> h, out List<T3> i)
        {
            a = default(T1);
            b = default(T2);
            c = default(T3);
            d = new[] { default(T1) };
            e = new[] { default(T2) };
            f = new[] { default(T3) };
            g = new List<T1> { default(T1) };
            h = new List<T2> { default(T2) };
            i = new List<T3> { default(T3) };
        }

        public void GenericWithRepeatedGenericParameterReturnInterceptedOutParameters<T3, T2>(out T1 a, out T2 b, out T3 c, out T1[] d, out T2[] e, out T3[] f, out List<T1> g, out List<T2> h, out List<T3> i)
        {
            a = default(T1);
            b = default(T2);
            c = default(T3);
            d = new[] { default(T1) };
            e = new[] { default(T2) };
            f = new[] { default(T3) };
            g = new List<T1> { default(T1) };
            h = new List<T2> { default(T2) };
            i = new List<T3> { default(T3) };
        } 
        */
        #endregion
    }
}
