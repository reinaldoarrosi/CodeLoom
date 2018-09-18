using CodeLoom.Aspects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestAssembly.Aspects
{
    public interface ITestInstanceAspectInterface<T>
    {
        int this[string a, T b] { get; set; }

        T TestProperty { get; set; }

        T2 TestMethod<T2, T3>(T a, T2 b, T3 d, string c);
    }

    public interface ITestInstanceAspectInterface2<T, T2> : ITestInstanceAspectInterface<T2>
    {
        T2 this[T a, T b] { get; set; }

        T TestProperty2 { get; set; }

        int NormalProperty { get; set; }

        int NormalMethod(string a, double b);

        T2 TestMethod2(T a, string b);
    }

    public class TestInstanceAspect<T> : InstanceAspect, ITestInstanceAspectInterface2<T, int>
    {
        private TestInstanceAspect<T> aspect = null;

        public TestInstanceAspect()
        { }

        int ITestInstanceAspectInterface2<T, int>.this[T a, T b]
        {
            get
            {
                return 0;
            }
            set
            {

            }
        }

        int ITestInstanceAspectInterface<int>.this[string a, int b]
        {
            get
            {
                return 1;
            }
            set
            {

            }
        }

        int ITestInstanceAspectInterface<int>.TestProperty
        {
            get;
            set;
        }

        T ITestInstanceAspectInterface2<T, int>.TestProperty2
        {
            get;
            set;
        }

        int ITestInstanceAspectInterface2<T, int>.NormalProperty
        {
            get;
            set;
        }

        int ITestInstanceAspectInterface2<T, int>.NormalMethod(string a, double b)
        {
            throw new NotImplementedException();
        }

        T2 ITestInstanceAspectInterface<int>.TestMethod<T2, T3>(int a, T2 b, T3 d, string c)
        {
            throw new NotImplementedException();
        }

        int ITestInstanceAspectInterface2<T, int>.TestMethod2(T a, string b)
        {
            throw new NotImplementedException();
        }
    }

    public class OK : ITestInstanceAspectInterface2<string, float>
    {
        private OK a = null;

        int ITestInstanceAspectInterface<float>.this[string a, float b]
        {
            get
            {
                ITestInstanceAspectInterface<float> i = this.a;
                return i[a, b];
            }
            set
            {
                ITestInstanceAspectInterface<float> i = this.a;
                i[a, b] = value;
            }
        }

        float ITestInstanceAspectInterface2<string, float>.this[string a, string b]
        {
            get
            {
                ITestInstanceAspectInterface2<string, float> i = this.a;
                return i[a, b];
            }
            set
            {
                ITestInstanceAspectInterface2<string, float> i = this.a;
                i[a, b] = value;
            }
        }

        float ITestInstanceAspectInterface<float>.TestProperty
        {
            get
            {
                ITestInstanceAspectInterface<float> i = a;
                return i.TestProperty;
            }
            set
            {
                ITestInstanceAspectInterface<float> i = a;
                i.TestProperty = value;
            }
        }

        string ITestInstanceAspectInterface2<string, float>.TestProperty2
        {
            get
            {
                ITestInstanceAspectInterface2<string, float> i = a;
                return i.TestProperty2;
            }
            set
            {
                ITestInstanceAspectInterface2<string, float> i = a;
                i.TestProperty2 = value;
            }
        }

        int ITestInstanceAspectInterface2<string, float>.NormalProperty
        {
            get
            {
                ITestInstanceAspectInterface2<string, float> i = a;
                return i.NormalProperty;
            }
            set
            {
                ITestInstanceAspectInterface2<string, float> i = a;
                i.NormalProperty = value;
            }
        }

        int ITestInstanceAspectInterface2<string, float>.NormalMethod(string a, double b)
        {
            ITestInstanceAspectInterface2<string, float> i = this.a;
            return i.NormalMethod(a, b);
        }

        float ITestInstanceAspectInterface2<string, float>.TestMethod2(string a, string b)
        {
            ITestInstanceAspectInterface2<string, float> i = this.a;
            return i.TestMethod2(a, b);
        }

        T2 ITestInstanceAspectInterface<float>.TestMethod<T2, T3>(float a, T2 b, T3 d, string c)
        {
            ITestInstanceAspectInterface<float> i = this.a;
            return i.TestMethod<T2, T3>(a, b, d, c);
        }
    }
}
