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
    public class OK : IMethodsAdded, IGenericMethodsAdded1<string>, IGenericMethodsAdded2<double, SimpleClass>
    {
        private IMethodsAdded k_instance;
        private IGenericMethodsAdded1<string> k_instance2;
        private IGenericMethodsAdded2<double, SimpleClass> k_instance3;

        void IMethodsAdded.VoidMethod()
        {
            k_instance.VoidMethod();
        }

        void IMethodsAdded.VoidMethodWithSimpleParameters(int a, string b)
        {
            k_instance.VoidMethodWithSimpleParameters(a, b);
        }

        int IMethodsAdded.IntMethodWithSimpleParameters(double a, byte[] b)
        {
            return k_instance.IntMethodWithSimpleParameters(a, b);
        }

        T1 IMethodsAdded.GenericMethodWithComplexParameters<T1, T2>(T1 a, List<List<List<T2>>> b, SimpleClass c)
        {
            var obj = k_instance;
            return obj.GenericMethodWithComplexParameters<T1, T2>(a, b, c);
        }

        string IGenericMethodsAdded1<string>.ComplexGenericMethod1<T1, T2>(string a, T1 b, List<T2> c, SimpleClass d)
        {
            var obj = k_instance2;
            return obj.ComplexGenericMethod1<T1, T2>(a, b, c, d);
        }

        double IGenericMethodsAdded1<double>.ComplexGenericMethod1<T1, T2>(double a, T1 b, List<T2> c, SimpleClass d)
        {
            var obj = k_instance3;
            return k_instance3.ComplexGenericMethod1<T1, T2>(a, b, c, d);
        }

        T2 IGenericMethodsAdded2<double, SimpleClass>.ComplexGenericMethod2<T1, T2, T3>(T1 a, T2 b, List<T3> c, SimpleClass d)
        {
            var obj = k_instance3;
            return k_instance3.ComplexGenericMethod2<T1, T2, T3>(a, b, c, d);
        }
    }

    public interface IMethodsAdded
    {
        void VoidMethod();

        void VoidMethodWithSimpleParameters(int a, string b);

        int IntMethodWithSimpleParameters(double a, byte[] b);

        T1 GenericMethodWithComplexParameters<T1, T2>(T1 a, List<List<List<T2>>> b, SimpleClass c);
    }

    public interface IGenericMethodsAdded1<T>
    {
        T ComplexGenericMethod1<T1, T2>(T a, T1 b, List<T2> c, SimpleClass d) where T2 : SimpleClass;
    }

    public interface IGenericMethodsAdded2<T1, T2> : IGenericMethodsAdded1<T1>
    {
        T2 ComplexGenericMethod2<T1, T2, T3>(T1 a, T2 b, List<T3> c, SimpleClass d) where T3 : SimpleClass;
    }

    public class AddMethodsToClassAspect : ImplementInterfaceAspect, IMethodsAdded, IGenericMethodsAdded1<string>, IGenericMethodsAdded2<double, SimpleClass>
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

        public T1 GenericMethodWithComplexParameters<T1, T2>(T1 a, List<List<List<T2>>> b, SimpleClass c)
        {
            return a;
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
