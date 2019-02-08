using CodeLoom.Aspects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestAssemblyReference;

namespace TestAssembly.Aspects.ImplementInterface
{
    public interface IMethodsFromGenericInterface<T1, T2>
    {
        T1 ReturnValueFromFirstParameter(T1 a, T2 b);
        string ReturnRefParametersAsString(ref T1 a, ref T2 b);
        void ReturnOutParameters(out T1 a, out T2 b, out T1[] c, out T2[] d, out List<T1> e, out List<T2> f);
    }

    public interface IGenericMethodsFromGenericInterface<T1, T2>
    {
        T1 GenericReturnValueFromFirstParameter<T3, T4>(T1 a, T2 b, T3 c, T4 d);
        string GenericReturnRefParametersAsString<T3, T4>(ref T1 a, ref T2 b, ref T3 c, ref T4 d);
        void GenericReturnOutParameters<T3, T4>(out T1 a, out T2 b, out T3 c, out T4 d, out T1[] e, out T2[] f, out T3[] g, out T4[] h, out List<T1> i, out List<T2> j, out List<T3> k, out List<T4> l);
    }

    public interface IGenericMethodsFromGenericInterfaceWithRepeatedGenericParameter<T1, T2>
    {
        T1 GenericWithRepeatedGenericParameterReturnValueFromFirstParameter<T3, T2>(T1 a, T2 b, T3 c);
        string GenericWithRepeatedGenericParameterReturnRefParametersAsString<T3, T2>(ref T1 a, ref T2 b, ref T3 c);
        void GenericWithRepeatedGenericParameterReturnOutParameters<T3, T2>(out T1 a, out T2 b, out T3 c, out T1[] d, out T2[] e, out T3[] f, out List<T1> g, out List<T2> h, out List<T3> i);
    }

    public interface IGenericProperties<T1, T2>
    {
        T1 T1Value { get; set; }

        List<T1> T1List { get; set; }

        List<Tuple<T1, T2>> T1T2TupleList { get; set; }

        Tuple<T2,T1> this[T1 a, T2 b]
        {
            get;
            set;
        }
    }

    public interface IGenericConstraints<T1, T2> where T1 : struct where T2 : class
    {
        T1 GenericConstraintMethod(T1 a, T2 b);

        T1 GenericConstraintMethodWithRepeatedGenericParameter<T2>(T1 a, T2 b) where T2 : struct;
    }

    public class ImplementInterfaceGenericMethodsAspect<T1, T2> : IImplementInterfaceAspect, IMethodsFromGenericInterface<T1, T2>, IGenericMethodsFromGenericInterface<T1,T2>, IGenericMethodsFromGenericInterfaceWithRepeatedGenericParameter<T1, T2>, IGenericProperties<T1, T2>, IGenericConstraints<T1, T2> where T1 : struct where T2 : class
    {
        public Tuple<T1, T2, Tuple<T2, T1>> _indexerPropBackingField;


        #region Simple methods
        public T1 ReturnValueFromFirstParameter(T1 a, T2 b)
        {
            return a;
        }

        public string ReturnRefParametersAsString(ref T1 a, ref T2 b)
        {
            return Helper.AsString(a, b);
        }

        public void ReturnOutParameters(out T1 a, out T2 b, out T1[] c, out T2[] d, out List<T1> e, out List<T2> f)
        {
            a = default(T1);
            b = default(T2);
            c = new[] { default(T1) };
            d = new[] { default(T2) };
            e = new List<T1> { default(T1) };
            f = new List<T2> { default(T2) };
        }
        #endregion
        
        #region Generic Methods
        public T1 GenericReturnValueFromFirstParameter<T3, T4>(T1 a, T2 b, T3 c, T4 d)
        {
            return a;
        }

        public string GenericReturnRefParametersAsString<T3, T4>(ref T1 a, ref T2 b, ref T3 c, ref T4 d)
        {
            return Helper.AsString(a, b, c, d);
        }

        public void GenericReturnOutParameters<T3, T4>(out T1 a, out T2 b, out T3 c, out T4 d, out T1[] e, out T2[] f, out T3[] g, out T4[] h, out List<T1> i, out List<T2> j, out List<T3> k, out List<T4> l)
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

        #region Generic methods with repeated generic parameter
        public T1 GenericWithRepeatedGenericParameterReturnValueFromFirstParameter<T3, T2>(T1 a, T2 b, T3 c)
        {
            return a;
        }

        public string GenericWithRepeatedGenericParameterReturnRefParametersAsString<T3, T2>(ref T1 a, ref T2 b, ref T3 c)
        {
            return Helper.AsString(a, b, c);
        }

        public void GenericWithRepeatedGenericParameterReturnOutParameters<T3, T2>(out T1 a, out T2 b, out T3 c, out T1[] d, out T2[] e, out T3[] f, out List<T1> g, out List<T2> h, out List<T3> i)
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
        #endregion

        #region Generic Constraints
        public T1 GenericConstraintMethod(T1 a, T2 b)
        {
            return a;
        }

        public T1 GenericConstraintMethodWithRepeatedGenericParameter<T2>(T1 a, T2 b) where T2 : struct
        {
            return a;
        }
        #endregion

        #region Properties
        public T1 T1Value { get; set; }

        public List<T1> T1List { get; set; }

        public List<Tuple<T1, T2>> T1T2TupleList { get; set; }

        public Tuple<T2, T1> this[T1 a, T2 b] { get { return _indexerPropBackingField.Item3; } set { _indexerPropBackingField = new Tuple<T1, T2, Tuple<T2, T1>>(a, b, value); } }
        #endregion
    }
}
