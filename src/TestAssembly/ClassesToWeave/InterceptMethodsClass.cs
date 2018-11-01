using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestAssembly.Aspects;
using TestAssemblyReference;

namespace TestAssembly.ClassesToWeave
{
    public class InterceptMethodsClass
    {
        public int ReturnOriginalValueType() { return 1; }

        public string ReturnOriginalRefType() { return "a"; }

        public SimpleStruct ReturnOriginalExternalValueType() { return new SimpleStruct(1); }

        public SimpleClass ReturnOriginalExternalRefType() { return new SimpleClass(1); }


        public int[] ReturnOriginalValueTypeArray() { return new [] { 1 }; }

        public string[] ReturnOriginalRefTypeArray() { return new [] { "a" }; }

        public SimpleStruct[] ReturnOriginalExternalValueTypeArray() { return new[] { new SimpleStruct(1) }; }

        public SimpleClass[] ReturnOriginalExternalRefTypeArray() { return new [] { new SimpleClass(1) }; }


        public List<int> ReturnOriginalValueTypeList() { return new List<int> { 1 }; }

        public List<string> ReturnOriginalRefTypeList() { return new List<string> { "a" }; }

        public List<SimpleStruct> ReturnOriginalExternalValueTypeList() { return new List<SimpleStruct> { new SimpleStruct(1) }; }

        public List<SimpleClass> ReturnOriginalExternalRefTypeList() { return new List<SimpleClass> { new SimpleClass(1) }; }


        public int ReturnInterceptedValueType() { return 1; }

        public string ReturnInterceptedRefType() { return "a"; }

        public SimpleStruct ReturnInterceptedExternalValueType() { return new SimpleStruct(1); }

        public SimpleClass ReturnInterceptedExternalRefType() { return new SimpleClass(1); }


        public int[] ReturnInterceptedValueTypeArray() { return new[] { 1 }; }

        public string[] ReturnInterceptedRefTypeArray() { return new[] { "a" }; }

        public SimpleStruct[] ReturnInterceptedExternalValueTypeArray() { return new[] { new SimpleStruct(1) }; }

        public SimpleClass[] ReturnInterceptedExternalRefTypeArray() { return new[] { new SimpleClass(1) }; }


        public List<int> ReturnInterceptedValueTypeList() { return new List<int> { 1 }; }

        public List<string> ReturnInterceptedRefTypeList() { return new List<string> { "a" }; }

        public List<SimpleStruct> ReturnInterceptedExternalValueTypeList() { return new List<SimpleStruct> { new SimpleStruct(1) }; }

        public List<SimpleClass> ReturnInterceptedExternalRefTypeList() { return new List<SimpleClass> { new SimpleClass(1) }; }


        public string ReturnOriginalParametersAsString(int a, string b, SimpleStruct c, SimpleClass d, int[] e, string[] f, SimpleStruct[] g, SimpleClass[] h, List<int> i, List<string> j, List<SimpleStruct> k, List<SimpleClass> l)
        {
            return Helper.AsString(a, b, c, d, e, f, g, h, i, j, k, l);
        }


        public string ReturnInterceptedParametersAsString(int a, string b, SimpleStruct c, SimpleClass d, int[] e, string[] f, SimpleStruct[] g, SimpleClass[] h, List<int> i, List<string> j, List<SimpleStruct> k, List<SimpleClass> l)
        {
            return Helper.AsString(a, b, c, d, e, f, g, h, i, j, k, l);
        }


        public string ReturnOriginalRefParametersAsString(ref int a, ref string b, ref SimpleStruct c, ref SimpleClass d, ref int[] e, ref string[] f, ref SimpleStruct[] g, ref SimpleClass[] h, ref List<int> i, ref List<string> j, ref List<SimpleStruct> k, ref List<SimpleClass> l)
        {
            return Helper.AsString(a, b, c, d, e, f, g, h, i, j, k, l);
        }

        public string ReturnInterceptedRefParametersAsString(ref int a, ref string b, ref SimpleStruct c, ref SimpleClass d, ref int[] e, ref string[] f, ref SimpleStruct[] g, ref SimpleClass[] h, ref List<int> i, ref List<string> j, ref List<SimpleStruct> k, ref List<SimpleClass> l)
        {
            return Helper.AsString(a, b, c, d, e, f, g, h, i, j, k, l);
        }

        public void ReturnOriginalOutParameters(out int a, out string b, out SimpleStruct c, out SimpleClass d, out int[] e, out string[] f, out SimpleStruct[] g, out SimpleClass[] h, out List<int> i, out List<string> j, out List<SimpleStruct> k, out List<SimpleClass> l)
        {
            a = 1;
            b = "a";
            c = new SimpleStruct(1);
            d = new SimpleClass(1);
            e = new[] { 1 };
            f = new[] { "a" };
            g = new[] { new SimpleStruct(1) };
            h = new[] { new SimpleClass(1) };
            i = new List<int> { 1 };
            j = new List<string> { "a" };
            k = new List<SimpleStruct> { new SimpleStruct(1) };
            l = new List<SimpleClass> { new SimpleClass(1) };
        }

        public void ReturnInterceptedOutParameters(out int a, out string b, out SimpleStruct c, out SimpleClass d, out int[] e, out string[] f, out SimpleStruct[] g, out SimpleClass[] h, out List<int> i, out List<string> j, out List<SimpleStruct> k, out List<SimpleClass> l)
        {
            a = 1;
            b = "a";
            c = new SimpleStruct(1);
            d = new SimpleClass(1);
            e = new[] { 1 };
            f = new[] { "a" };
            g = new[] { new SimpleStruct(1) };
            h = new[] { new SimpleClass(1) };
            i = new List<int> { 1 };
            j = new List<string> { "a" };
            k = new List<SimpleStruct> { new SimpleStruct(1) };
            l = new List<SimpleClass> { new SimpleClass(1) };
        }

        public int ChangeSumToSubtract(int a, int b)
        {
            return a + b;
        }
    }
}
