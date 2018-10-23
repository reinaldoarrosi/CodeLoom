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
    public interface IMethodsAdded
    {
        void VoidMethod();

        void VoidMethodWithSimpleParameters(int a, string b);

        int IntMethodWithSimpleParameters(double a, byte[] b);

        int IntMethodWithRefParameters(ref double a, out byte[] b);

        T1 GenericMethodWithComplexParameters<T1, T2>(T1 a, List<List<List<T2>>> b, SimpleClass c);

        T1 GenericMethodWithComplexRefParameters<T1, T2>(ref T1 a, out List<List<List<T2>>> b, SimpleClass c);
    }

    public interface IGenericMethodsAdded1<T>
    {
        T ComplexGenericMethod1<T1, T2>(T a, T1 b, List<T2> c, SimpleClass d) where T2 : SimpleClass;

        T ComplexGenericMethodWithRefParameters1<T1, T2>(ref T a, out T1 b, List<T2> c, SimpleClass d) where T2 : SimpleClass;
    }

    public interface IGenericMethodsAdded2<T1, T2> : IGenericMethodsAdded1<T1>
    {
        T2 ComplexGenericMethod2<T1, T2, T3>(T1 a, T2 b, List<T3> c, SimpleClass d) where T3 : SimpleClass;

        T2 ComplexGenericMethodWithRefParameters2<T1, T2, T3>(ref T1 a, out T2 b, List<T3> c, SimpleClass d) where T3 : SimpleClass;
    }

    public class AddMethodsToClassAspect : ImplementInterfaceAspect, IMethodsAdded , IGenericMethodsAdded1<string>, IGenericMethodsAdded2<double, SimpleClass>
    {
        public string ComplexGenericMethod1<T1, T2>(string a, T1 b, List<T2> c, SimpleClass d) where T2 : SimpleClass
        {
            return a;
        }

        public double ComplexGenericMethod1<T1, T2>(double a, T1 b, List<T2> c, SimpleClass d) where T2 : SimpleClass
        {
            return a;
        }

        public T2 ComplexGenericMethod2<T1, T2, T3>(T1 a, T2 b, List<T3> c, SimpleClass d) where T3 : SimpleClass
        {
            return b;
        }

        public string ComplexGenericMethodWithRefParameters1<T1, T2>(ref string a, out T1 b, List<T2> c, SimpleClass d) where T2 : SimpleClass
        {
            b = default(T1);
            return a;
        }

        public double ComplexGenericMethodWithRefParameters1<T1, T2>(ref double a, out T1 b, List<T2> c, SimpleClass d) where T2 : SimpleClass
        {
            b = default(T1);
            return a;
        }

        public T2 ComplexGenericMethodWithRefParameters2<T1, T2, T3>(ref T1 a, out T2 b, List<T3> c, SimpleClass d) where T3 : SimpleClass
        {
            b = default(T2);
            return b;
        }

        public T1 GenericMethodWithComplexParameters<T1, T2>(T1 a, List<List<List<T2>>> b, SimpleClass c)
        {
            return a;
        }

        public T1 GenericMethodWithComplexRefParameters<T1, T2>(ref T1 a, out List<List<List<T2>>> b, SimpleClass c)
        {
            b = null;
            return a;
        }

        public int IntMethodWithRefParameters(ref double a, out byte[] b)
        {
            b = null;
            return 0;
        }

        public int IntMethodWithSimpleParameters(double a, byte[] b)
        {
            return 11;
        }

        public void VoidMethod()
        {
            return;
        }

        public void VoidMethodWithSimpleParameters(int a, string b)
        {
            return;
        }
    }
}
