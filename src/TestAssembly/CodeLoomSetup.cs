using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using CodeLoom.Aspects;
using CodeLoom.Bindings;
using CodeLoom.Contexts;
using TestAssembly.Aspects.InterceptGenericMethod;
using TestAssembly.Aspects.InterceptMethod;
using TestAssembly.ClassesToWeave;
using TestAssemblyReference;

[assembly:CodeLoom.CodeLoomSetup(typeof(TestAssembly.CodeLoomSetup))]

namespace TestAssembly
{
    public class Test<T1, T2>
    {
        private class Binding<T3, T2> : MethodBinding
        {
            public static Binding<T3, T2> INSTANCE;

            static Binding()
            {
                INSTANCE = new Binding<T3, T2>(new[] { new Aspects.InterceptGenericMethod.GenericMethods.GenericReturnOriginalValueFromFirstParameterAspect() });
            }

            public Binding(InterceptMethodAspect[] aspects)
                : base(aspects)
            { }

            protected override void Proceed(MethodContext context)
            {
                var instance = (Test<T1, T2>)context.Instance;
                var arguments = context.Arguments;

                T1 a = (T1)arguments.GetArgument(0);
                T2 b = (T2)arguments.GetArgument(1);
                T3 c = (T3)arguments.GetArgument(2);
                T1 returnValue = instance.Original_GenericMethod<T3, T2>(a, b, c);

                context.SetReturnValue(returnValue);
            }
        }

        public T1 GenericMethod<T3, T2>(T1 a, T2 b, T3 c)
        {
            object[] values = new object[] { a, b, c };
            MethodBase methodBase = typeof(Test<,>).GetMethod("GenericMethod");
            Arguments arguments = new Arguments(values);
            MethodContext context = new MethodContext(this, methodBase, arguments);

            Binding<T3, T2>.INSTANCE.Run(context);

            return (T1)context.ReturnValue;
        }

        public T1 Original_GenericMethod<T3, T2>(T1 a, T2 b, T3 c)
        {
            return a;
        }
    }

    public class CodeLoomSetup : CodeLoom.CodeLoomSetup
    {
        public override IEnumerable<InterceptMethodAspect> GetAspects(MethodBase method)
        {
            #region InterceptMethodsClass
            if (method == typeof(InterceptMethodsClass).GetMethod(nameof(InterceptMethodsClass.ReturnOriginalValueType)))
                yield return new ReturnOriginalValueTypeAspect();
            
            if (method == typeof(InterceptMethodsClass).GetMethod(nameof(InterceptMethodsClass.ReturnOriginalRefType)))
                yield return new ReturnOriginalRefTypeAspect();

            if (method == typeof(InterceptMethodsClass).GetMethod(nameof(InterceptMethodsClass.ReturnOriginalExternalValueType)))
                yield return new ReturnOriginalExternalValueTypeAspect();

            if (method == typeof(InterceptMethodsClass).GetMethod(nameof(InterceptMethodsClass.ReturnOriginalExternalRefType)))
                yield return new ReturnOriginalExternalRefTypeAspect();

            if (method == typeof(InterceptMethodsClass).GetMethod(nameof(InterceptMethodsClass.ReturnOriginalValueTypeArray)))
                yield return new ReturnOriginalValueTypeArrayAspect();

            if (method == typeof(InterceptMethodsClass).GetMethod(nameof(InterceptMethodsClass.ReturnOriginalRefTypeArray)))
                yield return new ReturnOriginalRefTypeArrayAspect();

            if (method == typeof(InterceptMethodsClass).GetMethod(nameof(InterceptMethodsClass.ReturnOriginalExternalValueTypeArray)))
                yield return new ReturnOriginalExternalValueTypeArrayAspect();

            if (method == typeof(InterceptMethodsClass).GetMethod(nameof(InterceptMethodsClass.ReturnOriginalExternalRefTypeArray)))
                yield return new ReturnOriginalExternalRefTypeArrayAspect();

            if (method == typeof(InterceptMethodsClass).GetMethod(nameof(InterceptMethodsClass.ReturnOriginalValueTypeList)))
                yield return new ReturnOriginalValueTypeListAspect();

            if (method == typeof(InterceptMethodsClass).GetMethod(nameof(InterceptMethodsClass.ReturnOriginalRefTypeList)))
                yield return new ReturnOriginalRefTypeListAspect();

            if (method == typeof(InterceptMethodsClass).GetMethod(nameof(InterceptMethodsClass.ReturnOriginalExternalValueTypeList)))
                yield return new ReturnOriginalExternalValueTypeListAspect();

            if (method == typeof(InterceptMethodsClass).GetMethod(nameof(InterceptMethodsClass.ReturnOriginalExternalRefTypeList)))
                yield return new ReturnOriginalExternalRefTypeListAspect();

            if (method == typeof(InterceptMethodsClass).GetMethod(nameof(InterceptMethodsClass.ReturnInterceptedValueType)))
                yield return new ReturnInterceptedValueTypeAspect();

            if (method == typeof(InterceptMethodsClass).GetMethod(nameof(InterceptMethodsClass.ReturnInterceptedRefType)))
                yield return new ReturnInterceptedRefTypeAspect();

            if (method == typeof(InterceptMethodsClass).GetMethod(nameof(InterceptMethodsClass.ReturnInterceptedExternalValueType)))
                yield return new ReturnInterceptedExternalValueTypeAspect();

            if (method == typeof(InterceptMethodsClass).GetMethod(nameof(InterceptMethodsClass.ReturnInterceptedExternalRefType)))
                yield return new ReturnInterceptedExternalRefTypeAspect();

            if (method == typeof(InterceptMethodsClass).GetMethod(nameof(InterceptMethodsClass.ReturnInterceptedValueTypeArray)))
                yield return new ReturnInterceptedValueTypeArrayAspect();

            if (method == typeof(InterceptMethodsClass).GetMethod(nameof(InterceptMethodsClass.ReturnInterceptedRefTypeArray)))
                yield return new ReturnInterceptedRefTypeArrayAspect();

            if (method == typeof(InterceptMethodsClass).GetMethod(nameof(InterceptMethodsClass.ReturnInterceptedExternalValueTypeArray)))
                yield return new ReturnInterceptedExternalValueTypeArrayAspect();

            if (method == typeof(InterceptMethodsClass).GetMethod(nameof(InterceptMethodsClass.ReturnInterceptedExternalRefTypeArray)))
                yield return new ReturnInterceptedExternalRefTypeArrayAspect();

            if (method == typeof(InterceptMethodsClass).GetMethod(nameof(InterceptMethodsClass.ReturnInterceptedValueTypeList)))
                yield return new ReturnInterceptedValueTypeListAspect();

            if (method == typeof(InterceptMethodsClass).GetMethod(nameof(InterceptMethodsClass.ReturnInterceptedRefTypeList)))
                yield return new ReturnInterceptedRefTypeListAspect();

            if (method == typeof(InterceptMethodsClass).GetMethod(nameof(InterceptMethodsClass.ReturnInterceptedExternalValueTypeList)))
                yield return new ReturnInterceptedExternalValueTypeListAspect();

            if (method == typeof(InterceptMethodsClass).GetMethod(nameof(InterceptMethodsClass.ReturnInterceptedExternalRefTypeList)))
                yield return new ReturnInterceptedExternalRefTypeListAspect();

            if (method == typeof(InterceptMethodsClass).GetMethod(nameof(InterceptMethodsClass.ReturnOriginalParametersAsString)))
                yield return new Aspects.InterceptMethod.ReturnOriginalParametersAsStringAspect();

            if (method == typeof(InterceptMethodsClass).GetMethod(nameof(InterceptMethodsClass.ReturnInterceptedParametersAsString)))
                yield return new Aspects.InterceptMethod.ReturnInterceptedParametersAsStringAspect();

            if (method == typeof(InterceptMethodsClass).GetMethod(nameof(InterceptMethodsClass.ReturnOriginalRefParametersAsString)))
                yield return new Aspects.InterceptMethod.ReturnOriginalRefParametersAsStringAspect();

            if (method == typeof(InterceptMethodsClass).GetMethod(nameof(InterceptMethodsClass.ReturnInterceptedRefParametersAsString)))
                yield return new Aspects.InterceptMethod.ReturnInterceptedRefParametersAsStringAspect();

            if (method == typeof(InterceptMethodsClass).GetMethod(nameof(InterceptMethodsClass.ReturnOriginalOutParameters)))
                yield return new Aspects.InterceptMethod.ReturnOriginalOutParametersAspect();

            if (method == typeof(InterceptMethodsClass).GetMethod(nameof(InterceptMethodsClass.ReturnInterceptedOutParameters)))
                yield return new Aspects.InterceptMethod.ReturnInterceptedOutParametersAspect();

            if (method == typeof(InterceptMethodsClass).GetMethod(nameof(InterceptMethodsClass.ChangeSumToSubtract)))
                yield return new ChangeSumToSubtractAspect();
            #endregion

            #region InterceptGenericMethodsClass
            #region Simple methods
            if (method == typeof(InterceptGenericMethodsClass<,>).GetMethod(nameof(InterceptGenericMethodsClass<int, int>.ReturnOriginalValueFromFirstParameter)))
                yield return new Aspects.InterceptGenericMethod.SimpleMethods.ReturnOriginalValueFromFirstParameterAspect();

            if (method == typeof(InterceptGenericMethodsClass<,>).GetMethod(nameof(InterceptGenericMethodsClass<int, int>.ReturnInterceptedValueFromFirstParameter)))
                yield return new Aspects.InterceptGenericMethod.SimpleMethods.ReturnInterceptedValueFromFirstParameterAspect();

            if (method == typeof(InterceptGenericMethodsClass<,>).GetMethod(nameof(InterceptGenericMethodsClass<int, int>.ReturnOriginalParametersAsString)))
                yield return new Aspects.InterceptGenericMethod.SimpleMethods.ReturnOriginalParametersAsStringAspect();

            if (method == typeof(InterceptGenericMethodsClass<,>).GetMethod(nameof(InterceptGenericMethodsClass<int, int>.ReturnInterceptedParametersAsString)))
                yield return new Aspects.InterceptGenericMethod.SimpleMethods.ReturnInterceptedParametersAsStringAspect();

            if (method == typeof(InterceptGenericMethodsClass<,>).GetMethod(nameof(InterceptGenericMethodsClass<int, int>.ReturnOriginalRefParametersAsString)))
                yield return new Aspects.InterceptGenericMethod.SimpleMethods.ReturnOriginalRefParametersAsStringAspect();

            if (method == typeof(InterceptGenericMethodsClass<,>).GetMethod(nameof(InterceptGenericMethodsClass<int, int>.ReturnInterceptedRefParametersAsString)))
                yield return new Aspects.InterceptGenericMethod.SimpleMethods.ReturnInterceptedRefParametersAsStringAspect();

            if (method == typeof(InterceptGenericMethodsClass<,>).GetMethod(nameof(InterceptGenericMethodsClass<int, int>.ReturnOriginalOutParameters)))
                yield return new Aspects.InterceptGenericMethod.SimpleMethods.ReturnOriginalOutParametersAspect();

            if (method == typeof(InterceptGenericMethodsClass<,>).GetMethod(nameof(InterceptGenericMethodsClass<int, int>.ReturnInterceptedOutParameters)))
                yield return new Aspects.InterceptGenericMethod.SimpleMethods.ReturnInterceptedOutParametersAspect();
            #endregion

            #region Generic methods
            if (method == typeof(InterceptGenericMethodsClass<,>).GetMethod(nameof(InterceptGenericMethodsClass<int, int>.GenericReturnOriginalValueFromFirstParameter)))
                yield return new Aspects.InterceptGenericMethod.GenericMethods.GenericReturnOriginalValueFromFirstParameterAspect();

            if (method == typeof(InterceptGenericMethodsClass<,>).GetMethod(nameof(InterceptGenericMethodsClass<int, int>.GenericReturnInterceptedValueFromFirstParameter)))
                yield return new Aspects.InterceptGenericMethod.GenericMethods.GenericReturnInterceptedValueFromFirstParameterAspect();

            if (method == typeof(InterceptGenericMethodsClass<,>).GetMethod(nameof(InterceptGenericMethodsClass<int, int>.GenericReturnOriginalParametersAsString)))
                yield return new Aspects.InterceptGenericMethod.GenericMethods.GenericReturnOriginalParametersAsStringAspect();

            if (method == typeof(InterceptGenericMethodsClass<,>).GetMethod(nameof(InterceptGenericMethodsClass<int, int>.GenericReturnInterceptedParametersAsString)))
                yield return new Aspects.InterceptGenericMethod.GenericMethods.GenericReturnInterceptedParametersAsStringAspect();

            if (method == typeof(InterceptGenericMethodsClass<,>).GetMethod(nameof(InterceptGenericMethodsClass<int, int>.GenericReturnOriginalRefParametersAsString)))
                yield return new Aspects.InterceptGenericMethod.GenericMethods.GenericReturnOriginalRefParametersAsStringAspect();

            if (method == typeof(InterceptGenericMethodsClass<,>).GetMethod(nameof(InterceptGenericMethodsClass<int, int>.GenericReturnInterceptedRefParametersAsString)))
                yield return new Aspects.InterceptGenericMethod.GenericMethods.GenericReturnInterceptedRefParametersAsStringAspect();

            if (method == typeof(InterceptGenericMethodsClass<,>).GetMethod(nameof(InterceptGenericMethodsClass<int, int>.GenericReturnOriginalOutParameters)))
                yield return new Aspects.InterceptGenericMethod.GenericMethods.GenericReturnOriginalOutParametersAspect();

            if (method == typeof(InterceptGenericMethodsClass<,>).GetMethod(nameof(InterceptGenericMethodsClass<int, int>.GenericReturnInterceptedOutParameters)))
                yield return new Aspects.InterceptGenericMethod.GenericMethods.GenericReturnInterceptedOutParametersAspect();
            #endregion

            #region Generic methods with repeated generic parameter
            if (method == typeof(InterceptGenericMethodsClass<,>).GetMethod(nameof(InterceptGenericMethodsClass<int, int>.GenericWithRepeatedGenericParameterReturnOriginalValueFromFirstParameter)))
                yield return new Aspects.InterceptGenericMethod.GenericMethodsWithRepeatedGenericParameter.GenericWithRepeatedGenericParameterReturnOriginalValueFromFirstParameterAspect();

            if (method == typeof(InterceptGenericMethodsClass<,>).GetMethod(nameof(InterceptGenericMethodsClass<int, int>.GenericWithRepeatedGenericParameterReturnInterceptedValueFromFirstParameter)))
                yield return new Aspects.InterceptGenericMethod.GenericMethodsWithRepeatedGenericParameter.GenericWithRepeatedGenericParameterReturnInterceptedValueFromFirstParameterAspect();

            if (method == typeof(InterceptGenericMethodsClass<,>).GetMethod(nameof(InterceptGenericMethodsClass<int, int>.GenericWithRepeatedGenericParameterReturnOriginalParametersAsString)))
                yield return new Aspects.InterceptGenericMethod.GenericMethodsWithRepeatedGenericParameter.GenericWithRepeatedGenericParameterReturnOriginalParametersAsStringAspect();

            if (method == typeof(InterceptGenericMethodsClass<,>).GetMethod(nameof(InterceptGenericMethodsClass<int, int>.GenericWithRepeatedGenericParameterReturnInterceptedParametersAsString)))
                yield return new Aspects.InterceptGenericMethod.GenericMethodsWithRepeatedGenericParameter.GenericWithRepeatedGenericParameterReturnInterceptedParametersAsStringAspect();

            if (method == typeof(InterceptGenericMethodsClass<,>).GetMethod(nameof(InterceptGenericMethodsClass<int, int>.GenericWithRepeatedGenericParameterReturnOriginalRefParametersAsString)))
                yield return new Aspects.InterceptGenericMethod.GenericMethodsWithRepeatedGenericParameter.GenericWithRepeatedGenericParameterReturnOriginalRefParametersAsStringAspect();

            if (method == typeof(InterceptGenericMethodsClass<,>).GetMethod(nameof(InterceptGenericMethodsClass<int, int>.GenericWithRepeatedGenericParameterReturnInterceptedRefParametersAsString)))
                yield return new Aspects.InterceptGenericMethod.GenericMethodsWithRepeatedGenericParameter.GenericWithRepeatedGenericParameterReturnInterceptedRefParametersAsStringAspect();

            if (method == typeof(InterceptGenericMethodsClass<,>).GetMethod(nameof(InterceptGenericMethodsClass<int, int>.GenericWithRepeatedGenericParameterReturnOriginalOutParameters)))
                yield return new Aspects.InterceptGenericMethod.GenericMethodsWithRepeatedGenericParameter.GenericWithRepeatedGenericParameterReturnOriginalOutParametersAspect();

            if (method == typeof(InterceptGenericMethodsClass<,>).GetMethod(nameof(InterceptGenericMethodsClass<int, int>.GenericWithRepeatedGenericParameterReturnInterceptedOutParameters)))
                yield return new Aspects.InterceptGenericMethod.GenericMethodsWithRepeatedGenericParameter.GenericWithRepeatedGenericParameterReturnInterceptedOutParametersAspect();
            #endregion
            #endregion
            
            #region InterceptGenericMethodsWithConstraintsClass
            #region Simple methods
            if (method == typeof(InterceptGenericMethodsWithConstraintsClass<,>).GetMethod(nameof(InterceptGenericMethodsWithConstraintsClass<int, int>.ReturnOriginalValueFromFirstParameter)))
                yield return new Aspects.InterceptGenericMethodWithConstraints.SimpleMethods.ReturnOriginalValueFromFirstParameterAspect();

            if (method == typeof(InterceptGenericMethodsWithConstraintsClass<,>).GetMethod(nameof(InterceptGenericMethodsWithConstraintsClass<int, int>.ReturnInterceptedValueFromFirstParameter)))
                yield return new Aspects.InterceptGenericMethodWithConstraints.SimpleMethods.ReturnInterceptedValueFromFirstParameterAspect();
            
            if (method == typeof(InterceptGenericMethodsWithConstraintsClass<,>).GetMethod(nameof(InterceptGenericMethodsWithConstraintsClass<int, int>.ReturnOriginalParametersAsString)))
                yield return new Aspects.InterceptGenericMethodWithConstraints.SimpleMethods.ReturnOriginalParametersAsStringAspect();

            if (method == typeof(InterceptGenericMethodsWithConstraintsClass<,>).GetMethod(nameof(InterceptGenericMethodsWithConstraintsClass<int, int>.ReturnInterceptedParametersAsString)))
                yield return new Aspects.InterceptGenericMethodWithConstraints.SimpleMethods.ReturnInterceptedParametersAsStringAspect();

            if (method == typeof(InterceptGenericMethodsWithConstraintsClass<,>).GetMethod(nameof(InterceptGenericMethodsWithConstraintsClass<int, int>.ReturnOriginalRefParametersAsString)))
                yield return new Aspects.InterceptGenericMethodWithConstraints.SimpleMethods.ReturnOriginalRefParametersAsStringAspect();

            if (method == typeof(InterceptGenericMethodsWithConstraintsClass<,>).GetMethod(nameof(InterceptGenericMethodsWithConstraintsClass<int, int>.ReturnInterceptedRefParametersAsString)))
                yield return new Aspects.InterceptGenericMethodWithConstraints.SimpleMethods.ReturnInterceptedRefParametersAsStringAspect();

            if (method == typeof(InterceptGenericMethodsWithConstraintsClass<,>).GetMethod(nameof(InterceptGenericMethodsWithConstraintsClass<int, int>.ReturnOriginalOutParameters)))
                yield return new Aspects.InterceptGenericMethodWithConstraints.SimpleMethods.ReturnOriginalOutParametersAspect();

            if (method == typeof(InterceptGenericMethodsWithConstraintsClass<,>).GetMethod(nameof(InterceptGenericMethodsWithConstraintsClass<int, int>.ReturnInterceptedOutParameters)))
                yield return new Aspects.InterceptGenericMethodWithConstraints.SimpleMethods.ReturnInterceptedOutParametersAspect();
            #endregion

            #region Generic methods
            if (method == typeof(InterceptGenericMethodsWithConstraintsClass<,>).GetMethod(nameof(InterceptGenericMethodsWithConstraintsClass<int, int>.GenericReturnOriginalValueFromFirstParameter)))
                yield return new Aspects.InterceptGenericMethodWithConstraints.GenericMethods.GenericReturnOriginalValueFromFirstParameterAspect();

            if (method == typeof(InterceptGenericMethodsWithConstraintsClass<,>).GetMethod(nameof(InterceptGenericMethodsWithConstraintsClass<int, int>.GenericReturnInterceptedValueFromFirstParameter)))
                yield return new Aspects.InterceptGenericMethodWithConstraints.GenericMethods.GenericReturnInterceptedValueFromFirstParameterAspect();

            if (method == typeof(InterceptGenericMethodsWithConstraintsClass<,>).GetMethod(nameof(InterceptGenericMethodsWithConstraintsClass<int, int>.GenericReturnOriginalParametersAsString)))
                yield return new Aspects.InterceptGenericMethodWithConstraints.GenericMethods.GenericReturnOriginalParametersAsStringAspect();

            if (method == typeof(InterceptGenericMethodsWithConstraintsClass<,>).GetMethod(nameof(InterceptGenericMethodsWithConstraintsClass<int, int>.GenericReturnInterceptedParametersAsString)))
                yield return new Aspects.InterceptGenericMethodWithConstraints.GenericMethods.GenericReturnInterceptedParametersAsStringAspect();

            if (method == typeof(InterceptGenericMethodsWithConstraintsClass<,>).GetMethod(nameof(InterceptGenericMethodsWithConstraintsClass<int, int>.GenericReturnOriginalRefParametersAsString)))
                yield return new Aspects.InterceptGenericMethodWithConstraints.GenericMethods.GenericReturnOriginalRefParametersAsStringAspect();

            if (method == typeof(InterceptGenericMethodsWithConstraintsClass<,>).GetMethod(nameof(InterceptGenericMethodsWithConstraintsClass<int, int>.GenericReturnInterceptedRefParametersAsString)))
                yield return new Aspects.InterceptGenericMethodWithConstraints.GenericMethods.GenericReturnInterceptedRefParametersAsStringAspect();

            if (method == typeof(InterceptGenericMethodsWithConstraintsClass<,>).GetMethod(nameof(InterceptGenericMethodsWithConstraintsClass<int, int>.GenericReturnOriginalOutParameters)))
                yield return new Aspects.InterceptGenericMethodWithConstraints.GenericMethods.GenericReturnOriginalOutParametersAspect();

            if (method == typeof(InterceptGenericMethodsWithConstraintsClass<,>).GetMethod(nameof(InterceptGenericMethodsWithConstraintsClass<int, int>.GenericReturnInterceptedOutParameters)))
                yield return new Aspects.InterceptGenericMethodWithConstraints.GenericMethods.GenericReturnInterceptedOutParametersAspect();
            #endregion

            #region Generic methods with repeated generic parameter
            if (method == typeof(InterceptGenericMethodsWithConstraintsClass<,>).GetMethod(nameof(InterceptGenericMethodsWithConstraintsClass<int, int>.GenericWithRepeatedGenericParameterReturnOriginalValueFromFirstParameter)))
                yield return new Aspects.InterceptGenericMethodWithConstraints.GenericMethodsWithRepeatedGenericParameter.GenericWithRepeatedGenericParameterReturnOriginalValueFromFirstParameterAspect();

            
            if (method == typeof(InterceptGenericMethodsWithConstraintsClass<,>).GetMethod(nameof(InterceptGenericMethodsWithConstraintsClass<int, int>.GenericWithRepeatedGenericParameterReturnInterceptedValueFromFirstParameter)))
                yield return new Aspects.InterceptGenericMethodWithConstraints.GenericMethodsWithRepeatedGenericParameter.GenericWithRepeatedGenericParameterReturnInterceptedValueFromFirstParameterAspect();
            
            if (method == typeof(InterceptGenericMethodsWithConstraintsClass<,>).GetMethod(nameof(InterceptGenericMethodsWithConstraintsClass<int, int>.GenericWithRepeatedGenericParameterReturnOriginalParametersAsString)))
                yield return new Aspects.InterceptGenericMethodWithConstraints.GenericMethodsWithRepeatedGenericParameter.GenericWithRepeatedGenericParameterReturnOriginalParametersAsStringAspect();

            if (method == typeof(InterceptGenericMethodsWithConstraintsClass<,>).GetMethod(nameof(InterceptGenericMethodsWithConstraintsClass<int, int>.GenericWithRepeatedGenericParameterReturnInterceptedParametersAsString)))
                yield return new Aspects.InterceptGenericMethodWithConstraints.GenericMethodsWithRepeatedGenericParameter.GenericWithRepeatedGenericParameterReturnInterceptedParametersAsStringAspect();

            if (method == typeof(InterceptGenericMethodsWithConstraintsClass<,>).GetMethod(nameof(InterceptGenericMethodsWithConstraintsClass<int, int>.GenericWithRepeatedGenericParameterReturnOriginalRefParametersAsString)))
                yield return new Aspects.InterceptGenericMethodWithConstraints.GenericMethodsWithRepeatedGenericParameter.GenericWithRepeatedGenericParameterReturnOriginalRefParametersAsStringAspect();

            if (method == typeof(InterceptGenericMethodsWithConstraintsClass<,>).GetMethod(nameof(InterceptGenericMethodsWithConstraintsClass<int, int>.GenericWithRepeatedGenericParameterReturnInterceptedRefParametersAsString)))
                yield return new Aspects.InterceptGenericMethodWithConstraints.GenericMethodsWithRepeatedGenericParameter.GenericWithRepeatedGenericParameterReturnInterceptedRefParametersAsStringAspect();

            if (method == typeof(InterceptGenericMethodsWithConstraintsClass<,>).GetMethod(nameof(InterceptGenericMethodsWithConstraintsClass<int, int>.GenericWithRepeatedGenericParameterReturnOriginalOutParameters)))
                yield return new Aspects.InterceptGenericMethodWithConstraints.GenericMethodsWithRepeatedGenericParameter.GenericWithRepeatedGenericParameterReturnOriginalOutParametersAspect();

            if (method == typeof(InterceptGenericMethodsWithConstraintsClass<,>).GetMethod(nameof(InterceptGenericMethodsWithConstraintsClass<int, int>.GenericWithRepeatedGenericParameterReturnInterceptedOutParameters)))
                yield return new Aspects.InterceptGenericMethodWithConstraints.GenericMethodsWithRepeatedGenericParameter.GenericWithRepeatedGenericParameterReturnInterceptedOutParametersAspect();
            #endregion
            #endregion
        }
    }
}
