using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using CodeLoom;
using CodeLoom.Aspects;
using CodeLoom.Contexts;
using CodeLoom.Pipelines;
using TestAssembly.Aspects;
using TestAssembly.ClassesToWeave;
using TestAssemblyReference;

[assembly:CodeLoom.CodeLoomSetup(typeof(TestAssembly.CodeLoomSetup))]

namespace TestAssembly
{
    public abstract class Test<T>
    {
        private static InterceptPropertyPipeline aspect;

        static Test()
        {
            aspect = new InterceptPropertyPipeline(new[] { new InterceptPropertiesAspect() }, proceed, null);
        }

        static int original(int a, SimpleClass b, int[] c, SimpleClass d, List<int> e, List<SimpleClass> f)
        {
            var names = new string[] { nameof(a), nameof(b), nameof(c), nameof(d), nameof(e), nameof(f) };
            var types = new Type[] { typeof(int), typeof(SimpleClass), typeof(int[]), typeof(SimpleClass), typeof(List<int>), typeof(List<SimpleClass>) };
            var values = new object[] { a, b, c, d, e, f };
            var arguments = new Arguments(names, types, values);
            var context = new PropertyContext(null, arguments);

            aspect.RunGetter(context);

            return (int)context.ReturnValue;
        }

        static void proceed(PropertyContext context)
        {
            Arguments args = context.Arguments;
            int a = (int)args.GetArgumentValue(0);
            SimpleClass b = (SimpleClass)args.GetArgumentValue(1);
            int[] c = (int[])args.GetArgumentValue(2);
            SimpleClass d = (SimpleClass)args.GetArgumentValue(3);
            List<int> e = (List<int>)args.GetArgumentValue(4);
            List<SimpleClass> f = (List<SimpleClass>)args.GetArgumentValue(5);

            var ret = original(a,b,c,d,e,f);

            args.SetArgumentValue(0, a);
            args.SetArgumentValue(1, b);
            args.SetArgumentValue(2, c);
            args.SetArgumentValue(3, d);
            args.SetArgumentValue(4, e);
            args.SetArgumentValue(5, f);
            context.SetReturnValue(ret);
        }
    }

    public class CodeLoomSetup : CodeLoom.CodeLoomSetup
    {
        public override IEnumerable<ImplementInterfaceAspect> GetAspects(Type type)
        {
            if (type == typeof(ClassesToWeave.ClassWithWeavedProperties))
                yield return new AddPropertiesToClassAspect();

            if (type.FullName == typeof(ClassesToWeave.GenericClassWithWeavedProperties<,>).FullName)
                yield return new AddPropertiesToClassAspect();

            if (type == typeof(ClassesToWeave.ClassWithWeavedMethods))
                yield return new AddMethodsToClassAspect();

            if (type == typeof(ClassesToWeave.GenericClassWithWeavedMethods<,>))
                yield return new AddMethodsToClassAspect();
        }

        public override IEnumerable<InterceptPropertyAspect> GetAspects(PropertyInfo property)
        {
            if (property.DeclaringType == typeof(ClassWithInterceptedProperties))
                yield return new InterceptPropertiesAspect();

            if (property.DeclaringType == typeof(ParentClass))
                yield return new InterceptPropertiesAspect();

            if (property.DeclaringType == typeof(ClassWithInterceptedStaticProperties))
                yield return new InterceptPropertiesAspect();

            if (property.DeclaringType == typeof(ParentClassStatic))
                yield return new InterceptPropertiesAspect();
        }

        public override IEnumerable<InterceptMethodAspect> GetAspects(MethodBase method)
        {
            if (method.DeclaringType == typeof(ClassWithInterceptedMethods))
                yield return new InterceptMethodsAspect();

            if (method.DeclaringType == typeof(ParentMethodsClass))
                yield return new InterceptMethodsAspect();

            if (method.DeclaringType == typeof(ClassWithInterceptedStaticMethods))
                yield return new InterceptMethodsAspect();

            if (method.DeclaringType == typeof(ParentMethodsClassStatic))
                yield return new InterceptMethodsAspect();
        }
    }
}
