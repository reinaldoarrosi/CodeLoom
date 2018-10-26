using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using CodeLoom.Aspects;
using CodeLoom.Contexts;
using TestAssembly.Aspects.InterceptMethod;
using TestAssembly.ClassesToWeave;
using TestAssemblyReference;

[assembly:CodeLoom.CodeLoomSetup(typeof(TestAssembly.CodeLoomSetup))]

namespace TestAssembly
{
    public class Test
    {
        public void Method1(ref int[] a, ref string[] c, ref SimpleClass[] e, ref SimpleStruct[] g)
        {
            var v = new Arguments(new object[] {
                new int [] { 1 },
                new string [] { "a" },
                new SimpleClass[] { new SimpleClass(1) },
                new SimpleStruct[] { new SimpleStruct(2) }
            });

            a = (int[])v.GetArgument(0);
            c = (string[])v.GetArgument(2);
            e = (SimpleClass[])v.GetArgument(4);
            g = (SimpleStruct[])v.GetArgument(6);
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
                yield return new ReturnOriginalParametersAsStringAspect();

            if (method == typeof(InterceptMethodsClass).GetMethod(nameof(InterceptMethodsClass.ReturnInterceptedParametersAsString)))
                yield return new ReturnInterceptedParametersAsStringAspect();

            if (method == typeof(InterceptMethodsClass).GetMethod(nameof(InterceptMethodsClass.ReturnOriginalRefParametersAsString)))
                yield return new ReturnOriginalRefParametersAsStringAspect();

            if (method == typeof(InterceptMethodsClass).GetMethod(nameof(InterceptMethodsClass.ReturnInterceptedRefParametersAsString)))
                yield return new ReturnInterceptedRefParametersAsStringAspect();

            if (method == typeof(InterceptMethodsClass).GetMethod(nameof(InterceptMethodsClass.ReturnOriginalOutParametersAsString)))
                yield return new ReturnOriginalOutParametersAsStringAspect();

            if (method == typeof(InterceptMethodsClass).GetMethod(nameof(InterceptMethodsClass.ReturnInterceptedOutParametersAsString)))
                yield return new ReturnInterceptedOutParametersAsStringAspect();

            if (method == typeof(InterceptMethodsClass).GetMethod(nameof(InterceptMethodsClass.ChangeSumToSubtract)))
                yield return new ChangeSumToSubtractAspect(); 
            #endregion
        }
    }
}
