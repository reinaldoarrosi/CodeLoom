using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using CodeLoom.Aspects;
using CodeLoom.Bindings;
using CodeLoom.Contexts;
using TestAssembly.ClassesToWeave;
using TestAssemblyReference;

[assembly:CodeLoom.CodeLoomSetup(typeof(TestAssembly.CodeLoomSetup))]

namespace TestAssembly
{
    public class Test
    {
        private class Binding : AsyncMethodBinding
        {
            public static Binding INSTANCE;

            static Binding()
            {
                INSTANCE = new Binding(new[] { new Aspects.InterceptAsyncMethod.ReturnInterceptedValueAspect() });
            }

            public Binding(IInterceptAsyncMethodAspect[] aspects)
                : base(aspects)
            { }

            protected override Task Proceed(AsyncMethodContext context)
            {
                var instance = (Test)context.Instance;
                var arguments = context.Arguments;

                int a = (int)arguments.GetArgument(0);
                instance.Original_MethodA(a);

                return Task.CompletedTask;
            }
        }

        public void MethodA(int a)
        {
            object[] values = new object[] { a };
            MethodBase methodBase = typeof(Test).GetMethod("GenericMethod");
            Arguments arguments = new Arguments(values);
            AsyncMethodContext context = new AsyncMethodContext(this, methodBase, arguments);

            var t = Binding.INSTANCE.Run(context);
        }

        public async void Original_MethodA(int a)
        {
            await Task.Delay(1);
        }
    }

    public class CodeLoomSetup : CodeLoom.CodeLoomSetup
    {
        public override IEnumerable<IInterceptMethodAspect> GetMethodAspects(MethodBase method)
        {
            #region InterceptMethodsClass
            if (method == typeof(InterceptMethodsClass).GetMethod(nameof(InterceptMethodsClass.ReturnOriginalValueType)))
                yield return new Aspects.InterceptMethod.ReturnOriginalValueTypeAspect();
            
            if (method == typeof(InterceptMethodsClass).GetMethod(nameof(InterceptMethodsClass.ReturnOriginalRefType)))
                yield return new Aspects.InterceptMethod.ReturnOriginalRefTypeAspect();

            if (method == typeof(InterceptMethodsClass).GetMethod(nameof(InterceptMethodsClass.ReturnOriginalExternalValueType)))
                yield return new Aspects.InterceptMethod.ReturnOriginalExternalValueTypeAspect();

            if (method == typeof(InterceptMethodsClass).GetMethod(nameof(InterceptMethodsClass.ReturnOriginalExternalRefType)))
                yield return new Aspects.InterceptMethod.ReturnOriginalExternalRefTypeAspect();

            if (method == typeof(InterceptMethodsClass).GetMethod(nameof(InterceptMethodsClass.ReturnOriginalValueTypeArray)))
                yield return new Aspects.InterceptMethod.ReturnOriginalValueTypeArrayAspect();

            if (method == typeof(InterceptMethodsClass).GetMethod(nameof(InterceptMethodsClass.ReturnOriginalRefTypeArray)))
                yield return new Aspects.InterceptMethod.ReturnOriginalRefTypeArrayAspect();

            if (method == typeof(InterceptMethodsClass).GetMethod(nameof(InterceptMethodsClass.ReturnOriginalExternalValueTypeArray)))
                yield return new Aspects.InterceptMethod.ReturnOriginalExternalValueTypeArrayAspect();

            if (method == typeof(InterceptMethodsClass).GetMethod(nameof(InterceptMethodsClass.ReturnOriginalExternalRefTypeArray)))
                yield return new Aspects.InterceptMethod.ReturnOriginalExternalRefTypeArrayAspect();

            if (method == typeof(InterceptMethodsClass).GetMethod(nameof(InterceptMethodsClass.ReturnOriginalValueTypeList)))
                yield return new Aspects.InterceptMethod.ReturnOriginalValueTypeListAspect();

            if (method == typeof(InterceptMethodsClass).GetMethod(nameof(InterceptMethodsClass.ReturnOriginalRefTypeList)))
                yield return new Aspects.InterceptMethod.ReturnOriginalRefTypeListAspect();

            if (method == typeof(InterceptMethodsClass).GetMethod(nameof(InterceptMethodsClass.ReturnOriginalExternalValueTypeList)))
                yield return new Aspects.InterceptMethod.ReturnOriginalExternalValueTypeListAspect();

            if (method == typeof(InterceptMethodsClass).GetMethod(nameof(InterceptMethodsClass.ReturnOriginalExternalRefTypeList)))
                yield return new Aspects.InterceptMethod.ReturnOriginalExternalRefTypeListAspect();

            if (method == typeof(InterceptMethodsClass).GetMethod(nameof(InterceptMethodsClass.ReturnInterceptedValueType)))
                yield return new Aspects.InterceptMethod.ReturnInterceptedValueTypeAspect();

            if (method == typeof(InterceptMethodsClass).GetMethod(nameof(InterceptMethodsClass.ReturnInterceptedRefType)))
                yield return new Aspects.InterceptMethod.ReturnInterceptedRefTypeAspect();

            if (method == typeof(InterceptMethodsClass).GetMethod(nameof(InterceptMethodsClass.ReturnInterceptedExternalValueType)))
                yield return new Aspects.InterceptMethod.ReturnInterceptedExternalValueTypeAspect();

            if (method == typeof(InterceptMethodsClass).GetMethod(nameof(InterceptMethodsClass.ReturnInterceptedExternalRefType)))
                yield return new Aspects.InterceptMethod.ReturnInterceptedExternalRefTypeAspect();

            if (method == typeof(InterceptMethodsClass).GetMethod(nameof(InterceptMethodsClass.ReturnInterceptedValueTypeArray)))
                yield return new Aspects.InterceptMethod.ReturnInterceptedValueTypeArrayAspect();

            if (method == typeof(InterceptMethodsClass).GetMethod(nameof(InterceptMethodsClass.ReturnInterceptedRefTypeArray)))
                yield return new Aspects.InterceptMethod.ReturnInterceptedRefTypeArrayAspect();

            if (method == typeof(InterceptMethodsClass).GetMethod(nameof(InterceptMethodsClass.ReturnInterceptedExternalValueTypeArray)))
                yield return new Aspects.InterceptMethod.ReturnInterceptedExternalValueTypeArrayAspect();

            if (method == typeof(InterceptMethodsClass).GetMethod(nameof(InterceptMethodsClass.ReturnInterceptedExternalRefTypeArray)))
                yield return new Aspects.InterceptMethod.ReturnInterceptedExternalRefTypeArrayAspect();

            if (method == typeof(InterceptMethodsClass).GetMethod(nameof(InterceptMethodsClass.ReturnInterceptedValueTypeList)))
                yield return new Aspects.InterceptMethod.ReturnInterceptedValueTypeListAspect();

            if (method == typeof(InterceptMethodsClass).GetMethod(nameof(InterceptMethodsClass.ReturnInterceptedRefTypeList)))
                yield return new Aspects.InterceptMethod.ReturnInterceptedRefTypeListAspect();

            if (method == typeof(InterceptMethodsClass).GetMethod(nameof(InterceptMethodsClass.ReturnInterceptedExternalValueTypeList)))
                yield return new Aspects.InterceptMethod.ReturnInterceptedExternalValueTypeListAspect();

            if (method == typeof(InterceptMethodsClass).GetMethod(nameof(InterceptMethodsClass.ReturnInterceptedExternalRefTypeList)))
                yield return new Aspects.InterceptMethod.ReturnInterceptedExternalRefTypeListAspect();

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
                yield return new Aspects.InterceptMethod.ChangeSumToSubtractAspect();
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

            #region InterceptYieldMethodsClass
            if (method == typeof(InterceptYieldMethodsClass).GetMethod(nameof(InterceptYieldMethodsClass.ReturnOriginalIntEnumerable)))
                yield return new Aspects.InterceptYieldMethod.ReturnOriginalIntEnumerableAspect();

            if (method == typeof(InterceptYieldMethodsClass).GetMethod(nameof(InterceptYieldMethodsClass.ReturnOriginalSimpleClassEnumerable)))
                yield return new Aspects.InterceptYieldMethod.ReturnOriginalSimpleClassEnumerableAspect();

            if (method == typeof(InterceptYieldMethodsClass).GetMethod(nameof(InterceptYieldMethodsClass.ReturnOriginalTEnumerable)))
                yield return new Aspects.InterceptYieldMethod.ReturnOriginalTEnumerableAspect();

            if (method == typeof(InterceptYieldMethodsClass).GetMethod(nameof(InterceptYieldMethodsClass.ReturnInterceptedIntEnumerable)))
                yield return new Aspects.InterceptYieldMethod.ReturnInterceptedIntEnumerableAspect();

            if (method == typeof(InterceptYieldMethodsClass).GetMethod(nameof(InterceptYieldMethodsClass.ReturnInterceptedSimpleClassEnumerable)))
                yield return new Aspects.InterceptYieldMethod.ReturnInterceptedSimpleClassEnumerableAspect();

            if (method == typeof(InterceptYieldMethodsClass).GetMethod(nameof(InterceptYieldMethodsClass.ReturnInterceptedTEnumerable)))
                yield return new Aspects.InterceptYieldMethod.ReturnInterceptedTEnumerableAspect();

            if (method == typeof(InterceptYieldMethodsClass).GetMethod(nameof(InterceptYieldMethodsClass.ReturnEmptyEnumerable)))
                yield return new Aspects.InterceptYieldMethod.ReturnEmptyEnumerableAspect();

            if (method == typeof(InterceptYieldMethodsClass).GetMethod(nameof(InterceptYieldMethodsClass.ReplaceEmptyEnumerableWithANonEmptyEnumerable)))
                yield return new Aspects.InterceptYieldMethod.ReplaceEmptyEnumerableWithANonEmptyEnumerableAspect();
            #endregion

            #region InterceptConstructorClass
            if (method.DeclaringType.IsGenericType && method.DeclaringType.GetGenericTypeDefinition() == typeof(InterceptConstructorClass<>) && method.IsConstructor)
                yield return new Aspects.InterceptConstructor.ChangeArgumentsValuesAspect();

            if (method.DeclaringType.IsGenericType && method.DeclaringType.GetGenericTypeDefinition() == typeof(InterceptConstructorClass<>.InnerClass<>) && method.IsConstructor)
                yield return new Aspects.InterceptConstructor.ChangeArgumentsValuesAspect();
            #endregion

            #region InterceptStaticMethodsClass
            if (method == typeof(InterceptStaticMethodsClass<>).GetMethod(nameof(InterceptStaticMethodsClass<SimpleClass>.ReturnOriginalIntValue)))
                yield return new Aspects.InterceptStaticMethod.ReturnOriginalIntValueAspect();

            if (method == typeof(InterceptStaticMethodsClass<>).GetMethod(nameof(InterceptStaticMethodsClass<SimpleClass>.ReturnOriginalSimpleClassValue)))
                yield return new Aspects.InterceptStaticMethod.ReturnOriginalSimpleClassValueAspect();

            if (method == typeof(InterceptStaticMethodsClass<>).GetMethod(nameof(InterceptStaticMethodsClass<SimpleClass>.ReturnOriginalTValue)))
                yield return new Aspects.InterceptStaticMethod.ReturnOriginalTValueAspect();

            if (method == typeof(InterceptStaticMethodsClass<>).GetMethod(nameof(InterceptStaticMethodsClass<SimpleClass>.ReturnOriginalT2Value)))
                yield return new Aspects.InterceptStaticMethod.ReturnOriginalT2ValueAspect();

            if (method == typeof(InterceptStaticMethodsClass<>).GetMethod(nameof(InterceptStaticMethodsClass<SimpleClass>.ReturnOriginalTYieldEnumerable)))
                yield return new Aspects.InterceptStaticMethod.ReturnOriginalTYieldEnumerableAspect();

            if (method == typeof(InterceptStaticMethodsClass<>).GetMethod(nameof(InterceptStaticMethodsClass<SimpleClass>.ReturnInterceptedIntValue)))
                yield return new Aspects.InterceptStaticMethod.ReturnInterceptedIntValueAspect();

            if (method == typeof(InterceptStaticMethodsClass<>).GetMethod(nameof(InterceptStaticMethodsClass<SimpleClass>.ReturnInterceptedSimpleClassValue)))
                yield return new Aspects.InterceptStaticMethod.ReturnInterceptedSimpleClassValueAspect();

            if (method == typeof(InterceptStaticMethodsClass<>).GetMethod(nameof(InterceptStaticMethodsClass<SimpleClass>.ReturnInterceptedTValue)))
                yield return new Aspects.InterceptStaticMethod.ReturnInterceptedTValueAspect();

            if (method == typeof(InterceptStaticMethodsClass<>).GetMethod(nameof(InterceptStaticMethodsClass<SimpleClass>.ReturnInterceptedT2Value)))
                yield return new Aspects.InterceptStaticMethod.ReturnInterceptedT2ValueAspect();

            if (method == typeof(InterceptStaticMethodsClass<>).GetMethod(nameof(InterceptStaticMethodsClass<SimpleClass>.ReturnInterceptedTYieldEnumerable)))
                yield return new Aspects.InterceptStaticMethod.ReturnInterceptedTYieldEnumerableAspect();
            #endregion
        }

        public override IEnumerable<IInterceptAsyncMethodAspect> GetAsyncMethodAspects(MethodBase method)
        {
            #region InterceptAsyncMethodsClass
            if (method == typeof(InterceptAsyncMethodsClass).GetMethod(nameof(InterceptAsyncMethodsClass.ReturnOriginalValue)))
                yield return new Aspects.InterceptAsyncMethod.ReturnOriginalValueAspect();

            if (method == typeof(InterceptAsyncMethodsClass).GetMethod(nameof(InterceptAsyncMethodsClass.ReturnOriginalValueArray)))
                yield return new Aspects.InterceptAsyncMethod.ReturnOriginalValueArrayAspect();

            if (method == typeof(InterceptAsyncMethodsClass).GetMethod(nameof(InterceptAsyncMethodsClass.ReturnOriginalValueList)))
                yield return new Aspects.InterceptAsyncMethod.ReturnOriginalValueListAspect();

            if (method == typeof(InterceptAsyncMethodsClass).GetMethod(nameof(InterceptAsyncMethodsClass.ReturnInterceptedValue)))
                yield return new Aspects.InterceptAsyncMethod.ReturnInterceptedValueAspect();

            if (method == typeof(InterceptAsyncMethodsClass).GetMethod(nameof(InterceptAsyncMethodsClass.ReturnInterceptedValueArray)))
                yield return new Aspects.InterceptAsyncMethod.ReturnInterceptedValueArrayAspect();

            if (method == typeof(InterceptAsyncMethodsClass).GetMethod(nameof(InterceptAsyncMethodsClass.ReturnInterceptedValueList)))
                yield return new Aspects.InterceptAsyncMethod.ReturnInterceptedValueListAspect();

            if (method == typeof(InterceptAsyncMethodsClass).GetMethod(nameof(InterceptAsyncMethodsClass.ReturnOriginalGenericValue)))
                yield return new Aspects.InterceptAsyncMethod.ReturnOriginalGenericValueAspect();

            if (method == typeof(InterceptAsyncMethodsClass).GetMethod(nameof(InterceptAsyncMethodsClass.ReturnOriginalGenericValueArray)))
                yield return new Aspects.InterceptAsyncMethod.ReturnOriginalGenericValueArrayAspect();

            if (method == typeof(InterceptAsyncMethodsClass).GetMethod(nameof(InterceptAsyncMethodsClass.ReturnOriginalGenericValueList)))
                yield return new Aspects.InterceptAsyncMethod.ReturnOriginalGenericValueListAspect();

            if (method == typeof(InterceptAsyncMethodsClass).GetMethod(nameof(InterceptAsyncMethodsClass.ReturnInterceptedGenericValue)))
                yield return new Aspects.InterceptAsyncMethod.ReturnInterceptedGenericValueAspect();

            if (method == typeof(InterceptAsyncMethodsClass).GetMethod(nameof(InterceptAsyncMethodsClass.ReturnInterceptedGenericValueArray)))
                yield return new Aspects.InterceptAsyncMethod.ReturnInterceptedGenericValueArrayAspect();

            if (method == typeof(InterceptAsyncMethodsClass).GetMethod(nameof(InterceptAsyncMethodsClass.ReturnInterceptedGenericValueList)))
                yield return new Aspects.InterceptAsyncMethod.ReturnInterceptedGenericValueListAspect();

            if (method == typeof(InterceptAsyncMethodsClass).GetMethod(nameof(InterceptAsyncMethodsClass.ReturnOriginalParametersAsString)))
                yield return new Aspects.InterceptAsyncMethod.ReturnOriginalParametersAsStringAspect();

            if (method == typeof(InterceptAsyncMethodsClass).GetMethod(nameof(InterceptAsyncMethodsClass.ReturnInterceptedParametersAsString)))
                yield return new Aspects.InterceptAsyncMethod.ReturnInterceptedParametersAsStringAspect();

            if (method == typeof(InterceptAsyncMethodsClass).GetMethod(nameof(InterceptAsyncMethodsClass.InterceptAsyncMethodThatReturnsTask)))
                yield return new Aspects.InterceptAsyncMethod.InterceptAsyncMethodThatReturnsTaskAspect();

            if (method == typeof(InterceptAsyncMethodsClass).GetMethod(nameof(InterceptAsyncMethodsClass.InterceptAsyncMethodThatReturnsVoid)))
                yield return new Aspects.InterceptAsyncMethod.InterceptAsyncMethodThatReturnsVoidAspect();
            #endregion

            #region InterceptStaticMethodsClass
            if (method == typeof(InterceptStaticMethodsClass<>).GetMethod(nameof(InterceptStaticMethodsClass<SimpleClass>.ReturnOriginalT2Async)))
                yield return new Aspects.InterceptStaticMethod.ReturnOriginalT2AsyncAspect();

            if (method == typeof(InterceptStaticMethodsClass<>).GetMethod(nameof(InterceptStaticMethodsClass<SimpleClass>.ReturnInterceptedT2Async)))
                yield return new Aspects.InterceptStaticMethod.ReturnInterceptedT2AsyncAspect();
            #endregion
        }
    }
}
