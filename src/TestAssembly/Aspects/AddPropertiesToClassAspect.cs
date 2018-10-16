using CodeLoom.Aspects;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestAssemblyReference;

namespace TestAssembly.Aspects
{
    public interface IPropertiesAdded
    {
        int IntProperty { get; set; }

        string[] StringArrayProperty { get; set; }

        ArrayList ReferenceToNETFrameworkClassProperty { get; set; }

        SimpleClass ReferenceToClassInAnotherAssemblyProperty { get; set; }

        List<SimpleClass> GenericListOfClassInAnotherAssemblyProperty { get; set; }

        int ReadOnlyProperty { get; }

        int WriteOnlyProperty { set; }

        object this[int a, SimpleClass b] { get; set; }

        object this[List<int> a, SimpleClass b] { get; set; }
    }

    public interface IGenericPropertiesAdded1<T>
    {
        T GenericProperty1 { get; set; }

        T this[T a] { get; set; }
    }

    public interface IGenericPropertiesAdded2<T1, T2> : IGenericPropertiesAdded1<T1>
    {
        T1 GenericProperty2_A { get; set; }

        T2 GenericProperty2_B { get; set; }

        T2 this[T1 a, T2 b] { get; set; }
    }

    public class AddPropertiesToClassAspect : ImplementInterfaceAspect, IPropertiesAdded, IGenericPropertiesAdded1<SimpleClass>, IGenericPropertiesAdded2<int, string>
    {
        private SimpleClass _indexer_SimpleClass_value;
        private int _indexer_Int_value;
        private string _indexer_Int_String_value;
        private object _indexer_Int_SimpleClass_value;
        private object _indexer_ListOfInt_SimpleClass_value;

        public AddPropertiesToClassAspect()
        {
            _indexer_SimpleClass_value = new SimpleClass();
            _indexer_Int_value = 0;
            _indexer_Int_String_value = "";
            _indexer_Int_SimpleClass_value = new Tuple<int, SimpleClass, object>(0, null, null);
            _indexer_ListOfInt_SimpleClass_value = new Tuple<List<int>, SimpleClass, object>(null, null, null);

            IntProperty = 10;
            StringArrayProperty = new[] { "a", "b" };
            ReferenceToNETFrameworkClassProperty = new ArrayList() { 1, 2, 3 };
            ReferenceToClassInAnotherAssemblyProperty = new SimpleClass();
            GenericListOfClassInAnotherAssemblyProperty = new List<SimpleClass>() { new SimpleClass(), new SimpleClass() };
            ReadOnlyProperty = 11;
            WriteOnlyProperty = 12;

            GenericProperty1 = new SimpleClass();
            GenericProperty2_A = 13;
            GenericProperty2_B = "c";
            (this as IGenericPropertiesAdded1<int>).GenericProperty1 = 14;
        }

        public SimpleClass this[SimpleClass a] { get => _indexer_SimpleClass_value; set => _indexer_SimpleClass_value = value; }
        public int this[int a] { get => _indexer_Int_value; set => _indexer_Int_value = value + a; }
        public object this[int a, SimpleClass b] { get => _indexer_Int_SimpleClass_value; set => _indexer_Int_SimpleClass_value = new Tuple<int, SimpleClass, object>(a, b, value); }
        public object this[List<int> a, SimpleClass b] { get => _indexer_ListOfInt_SimpleClass_value; set => _indexer_ListOfInt_SimpleClass_value = new Tuple<List<int>, SimpleClass, object>(a, b, value); }
        public string this[int a, string b] { get => _indexer_Int_String_value; set => _indexer_Int_String_value = (value ?? "") + a.ToString() + b; }

        public int IntProperty { get; set; }
        public string[] StringArrayProperty { get; set; }
        public ArrayList ReferenceToNETFrameworkClassProperty { get; set; }
        public SimpleClass ReferenceToClassInAnotherAssemblyProperty { get; set; }
        public List<SimpleClass> GenericListOfClassInAnotherAssemblyProperty { get; set; }
        public int ReadOnlyProperty { get; private set; }
        public int WriteOnlyProperty { private get; set; }

        public SimpleClass GenericProperty1 { get; set; }
        public int GenericProperty2_A { get; set; }
        public string GenericProperty2_B { get; set; }
        int IGenericPropertiesAdded1<int>.GenericProperty1 { get; set; }
    }
}
