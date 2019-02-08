using CodeLoom.Aspects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestAssemblyReference;

namespace TestAssembly.Aspects.ImplementInterface
{
    public interface ISimpleMethods
    {
        SimpleClass ReturnExternalRefType();
        SimpleStruct ReturnExternalValueType();              
        string ReturnRefType();
        int ReturnValueType();
        Task<int> ReturnAsyncInt(int a);
    }

    public interface ISimpleArrayMethods
    {
        SimpleClass[] ReturnExternalRefTypeArray();
        SimpleStruct[] ReturnExternalValueTypeArray();
        string[] ReturnRefTypeArray();
        int[] ReturnValueTypeArray();
    }

    public interface ISimpleListMethods
    {
        List<SimpleClass> ReturnExternalRefTypeList();
        List<SimpleStruct> ReturnExternalValueTypeList();
        List<string> ReturnRefTypeList();
        List<int> ReturnValueTypeList();
        IEnumerable<int> YieldIntList(params int[] ints);
    }

    public interface ISimpleMethodsWithRefParameters
    {
        string ReturnRefParametersAsString(ref int a, ref string b, ref SimpleStruct c, ref SimpleClass d, ref int[] e, ref string[] f, ref SimpleStruct[] g, ref SimpleClass[] h, ref List<int> i, ref List<string> j, ref List<SimpleStruct> k, ref List<SimpleClass> l);
    }

    public interface ISimpleMethodsWithOutParameters
    {
        void ReturnOutParameters(out int a, out string b, out SimpleStruct c, out SimpleClass d, out int[] e, out string[] f, out SimpleStruct[] g, out SimpleClass[] h, out List<int> i, out List<string> j, out List<SimpleStruct> k, out List<SimpleClass> l);
    }

    public interface ISimpleProperties
    {
        int ValueType { get; set; }

        string[] RefTypeArray { get; set; }

        List<SimpleClass> ExternalRefTypeList { get; set; }

        int ReadOnly { get; }
        int _readOnlyPropBackingField { get; set; }

        int WriteOnly { set; }
        int _writeOnlyPropBackingField { get; set; }

        int this[int a] { get; set; }
        Tuple<int, int> _indexer1PropBackingField { get; set; }

        
        int this[int a, int b] { get; set; }
        Tuple<int, int, int> _indexer2PropBackingField { get; set; }
    }

    public class ImplementInterfaceSimpleMethodsAspect : IImplementInterfaceAspect, ISimpleMethods, ISimpleArrayMethods, ISimpleListMethods, ISimpleMethodsWithRefParameters, ISimpleMethodsWithOutParameters, ISimpleProperties
    {
        public int ReturnValueType() { return 1; }

        public string ReturnRefType() { return "a"; }

        public SimpleStruct ReturnExternalValueType() { return new SimpleStruct(1); }

        public SimpleClass ReturnExternalRefType() { return new SimpleClass(1); }

        public async Task<int> ReturnAsyncInt(int a)
        {
            await Task.Delay(1000);
            return a;
        }


        public int[] ReturnValueTypeArray() { return new[] { 1, 2 }; }

        public string[] ReturnRefTypeArray() { return new[] { "a", "b" }; }

        public SimpleStruct[] ReturnExternalValueTypeArray() { return new[] { new SimpleStruct(1), new SimpleStruct(2) }; }

        public SimpleClass[] ReturnExternalRefTypeArray() { return new[] { new SimpleClass(1), new SimpleClass(2) }; }


        public List<int> ReturnValueTypeList() { return new List<int> { 1, 2 }; }

        public List<string> ReturnRefTypeList() { return new List<string> { "a", "b" }; }

        public List<SimpleStruct> ReturnExternalValueTypeList() { return new List<SimpleStruct> { new SimpleStruct(1), new SimpleStruct(2) }; }

        public List<SimpleClass> ReturnExternalRefTypeList() { return new List<SimpleClass> { new SimpleClass(1), new SimpleClass(2) }; }

        public IEnumerable<int> YieldIntList(params int[] ints)
        {
            foreach (var i in ints)
            {
                yield return i;
            }
        }


        public string ReturnRefParametersAsString(ref int a, ref string b, ref SimpleStruct c, ref SimpleClass d, ref int[] e, ref string[] f, ref SimpleStruct[] g, ref SimpleClass[] h, ref List<int> i, ref List<string> j, ref List<SimpleStruct> k, ref List<SimpleClass> l)
        {
            return Helper.AsString(a, b, c, d, e, f, g, h, i, j, k, l);
        }

        public void ReturnOutParameters(out int a, out string b, out SimpleStruct c, out SimpleClass d, out int[] e, out string[] f, out SimpleStruct[] g, out SimpleClass[] h, out List<int> i, out List<string> j, out List<SimpleStruct> k, out List<SimpleClass> l)
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

        public int ValueType { get; set; }

        public string[] RefTypeArray { get; set; }

        public List<SimpleClass> ExternalRefTypeList { get; set; }

        public int ReadOnly { get { return _readOnlyPropBackingField; } }
        public int _readOnlyPropBackingField { get; set; }


        public int WriteOnly { set { _writeOnlyPropBackingField = value; } }
        public int _writeOnlyPropBackingField { get; set; }


        public int this[int a]
        {
            get { return _indexer1PropBackingField.Item2; }
            set { _indexer1PropBackingField = new Tuple<int, int>(a, value); }
        }
        public Tuple<int, int> _indexer1PropBackingField { get; set; }

        public int this[int a, int b]
        {
            get { return _indexer2PropBackingField.Item3; }
            set { _indexer2PropBackingField = new Tuple<int, int, int>(a, b, value); }
        }
        public Tuple<int, int, int> _indexer2PropBackingField { get; set; }
    }
}
