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
        private InterceptMethodPipeline aspect;

        Test()
        {
            aspect = new InterceptMethodPipeline(new[] { new InterceptMethodsAspect() }, proceed_Method);
        }

        int Method(ref int a, ref string b, ref List<SimpleClass> c, out int a1, out string b1, out List<SimpleClass> c1)
        {
            var values = new object[] { a, b, c, null, null, null };
            var arguments = new Arguments(values);
            var context = new MethodContext(this, null, arguments);

            aspect.Run(context);

            a = (int)arguments.GetArgument(0);
            b = (string)arguments.GetArgument(1);
            c = (List<SimpleClass>)arguments.GetArgument(2);
            a1 = (int)arguments.GetArgument(3);
            b1 = (string)arguments.GetArgument(4);
            c1 = (List<SimpleClass>)arguments.GetArgument(5);
            return (int)context.ReturnValue;
        }

        int original_Method(ref int a, ref string b, ref List<SimpleClass> c, out int a1, out string b1, out List<SimpleClass> c1)
        {
            a1 = 0;
            b1 = null;
            c1 = null;
            return 0;
        }

        void proceed_Method(MethodContext context)
        {
            var arguments = context.Arguments;
            var a = (int)arguments.GetArgument(0);
            var b = (string)arguments.GetArgument(1);
            var c = (List<SimpleClass>)arguments.GetArgument(2);
            int a1;
            string b1;
            List<SimpleClass> c1;

            var result = original_Method(ref a, ref b, ref c, out a1, out b1, out c1);

            arguments.SetArgument(0, a);
            arguments.SetArgument(1, b);
            arguments.SetArgument(2, c);
            arguments.SetArgument(3, a1);
            arguments.SetArgument(4, b1);
            arguments.SetArgument(5, c1);
            context.SetReturnValue(result);
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
            yield break;

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
