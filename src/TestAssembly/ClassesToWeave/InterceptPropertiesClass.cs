using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestAssemblyReference;

namespace TestAssembly.ClassesToWeave
{
    public class InterceptPropertiesClass<T> where T : SimpleClass
    {
        public InterceptPropertiesClass()
        {
            object TValue = new InheritsFromSimpleClass(1);

            OriginalValueType = 1;
            OriginalRefType = "a";
            OriginalExternalValueType = new SimpleStruct(1);
            OriginalExternalRefType = new SimpleClass(1);
            OriginalGenericType = (T)TValue;

            OriginalValueTypeArray = new[] { 1 };
            OriginalRefTypeArray = new[] { "a" };
            OriginalExternalValueTypeArray = new[] { new SimpleStruct(1) };
            OriginalExternalRefTypeArray = new[] { new SimpleClass(1) };
            OriginalGenericTypeArray = new[] { (T)TValue };

            OriginalValueTypeList = new List<int> { 1 };
            OriginalRefTypeList = new List<string> { "a" };
            OriginalExternalValueTypeList = new List<SimpleStruct> { new SimpleStruct(1) };
            OriginalExternalRefTypeList = new List<SimpleClass> { new SimpleClass(1) };
            OriginalGenericTypeList = new List<T> { (T)TValue };

            InterceptedValueType = 1;
            InterceptedRefType = "a";
            InterceptedExternalValueType = new SimpleStruct(1);
            InterceptedExternalRefType = new SimpleClass(1);
            InterceptedGenericType = (T)TValue;

            InterceptedValueTypeArray = new[] { 1 };
            InterceptedRefTypeArray = new[] { "a" };
            InterceptedExternalValueTypeArray = new[] { new SimpleStruct(1) };
            InterceptedExternalRefTypeArray = new[] { new SimpleClass(1) };
            InterceptedGenericTypeArray = new[] { (T)TValue };

            InterceptedValueTypeList = new List<int> { 1 };
            InterceptedRefTypeList = new List<string> { "a" };
            InterceptedExternalValueTypeList = new List<SimpleStruct> { new SimpleStruct(1) };
            InterceptedExternalRefTypeList = new List<SimpleClass> { new SimpleClass(1) };
            InterceptedGenericTypeList = new List<T> { (T)TValue };
        }

        public Tuple<int, string, SimpleStruct, SimpleClass, T, object> _indexerValue;
        public int _setOnlyPropertyValue;
        public int _interceptedByMultipleAspectsValue;

        public int OriginalValueType { get; set; }
        public string OriginalRefType { get; set; }
        public SimpleStruct OriginalExternalValueType { get; set; }
        public SimpleClass OriginalExternalRefType { get; set; }
        public T OriginalGenericType { get; set; }

        public int[] OriginalValueTypeArray { get; set; }
        public string[] OriginalRefTypeArray { get; set; }
        public SimpleStruct[] OriginalExternalValueTypeArray { get; set; }
        public SimpleClass[] OriginalExternalRefTypeArray { get; set; }
        public T[] OriginalGenericTypeArray { get; set; }

        public List<int> OriginalValueTypeList { get; set; }
        public List<string> OriginalRefTypeList { get; set; }
        public List<SimpleStruct> OriginalExternalValueTypeList { get; set; }
        public List<SimpleClass> OriginalExternalRefTypeList { get; set; }
        public List<T> OriginalGenericTypeList { get; set; }

        public int InterceptedValueType { get; set; }
        public string InterceptedRefType { get; set; }
        public SimpleStruct InterceptedExternalValueType { get; set; }
        public SimpleClass InterceptedExternalRefType { get; set; }
        public T InterceptedGenericType { get; set; }

        public int[] InterceptedValueTypeArray { get; set; }
        public string[] InterceptedRefTypeArray { get; set; }
        public SimpleStruct[] InterceptedExternalValueTypeArray { get; set; }
        public SimpleClass[] InterceptedExternalRefTypeArray { get; set; }
        public T[] InterceptedGenericTypeArray { get; set; }

        public List<int> InterceptedValueTypeList { get; set; }
        public List<string> InterceptedRefTypeList { get; set; }
        public List<SimpleStruct> InterceptedExternalValueTypeList { get; set; }
        public List<SimpleClass> InterceptedExternalRefTypeList { get; set; }
        public List<T> InterceptedGenericTypeList { get; set; }

        public int InterceptedByMultipleAspects
        {
            get
            {
                return _interceptedByMultipleAspectsValue;
            }
            set
            {
                _interceptedByMultipleAspectsValue = value;
            }
        }

        public Tuple<int, string, SimpleStruct, SimpleClass, T, object> this[int a, string b, SimpleStruct c, SimpleClass d, T e]
        {
            get
            {
                return _indexerValue;
            }
            set
            {
                _indexerValue = new Tuple<int, string, SimpleStruct, SimpleClass, T, object>(a, b, c, d, e, value);
            }
        }

        public IEnumerable<int> OriginalGetOnlyYieldProperty
        {
            get
            {
                for (int i = 0; i < 3; i++)
                {
                    yield return i;
                }
            }
        }

        public IEnumerable<int> InterceptedGetOnlyYieldProperty
        {
            get
            {
                for (int i = 0; i < 3; i++)
                {
                    yield return i;
                }
            }
        }

        public int OriginalSetOnlyProperty
        {
            set
            {
                _setOnlyPropertyValue = value;
            }
        }

        public int InterceptedSetOnlyProperty
        {
            set
            {
                _setOnlyPropertyValue = value;
            }
        }
    }
}
