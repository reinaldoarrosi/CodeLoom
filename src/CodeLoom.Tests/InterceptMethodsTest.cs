using CodeLoom.Fody;
using Xunit;
using Fody;
using System.Reflection;
using System.Collections.Generic;
using System.IO;
using System.Collections;
using TestAssembly.Aspects;
using TestAssembly.ClassesToWeave;
using System.Linq;
using TestAssemblyReference;

namespace CodeLoom.Tests
{
    public class InterceptMethodsTest : BaseTest
    {
        public class SimpleValues
        {
            [Fact]
            private void returns_original_value_when_ReturnOriginalValueType_is_called()
            {
                Execute(() =>
                {
                    var instance = new InterceptMethodsClass();
                    var result = instance.ReturnOriginalValueType();

                    Assert.Equal(1, result);
                });
            }

            [Fact]
            private void returns_intercepted_value_when_ReturnInterceptedValueType_is_called()
            {
                Execute(() =>
                {
                    var instance = new InterceptMethodsClass();
                    var result = instance.ReturnInterceptedValueType();

                    Assert.Equal(2, result);
                });
            }

            [Fact]
            private void returns_original_value_when_ReturnOriginalRefType_is_called()
            {
                Execute(() =>
                {
                    var instance = new InterceptMethodsClass();
                    var result = instance.ReturnOriginalRefType();

                    Assert.Equal("a", result);
                });
            }

            [Fact]
            private void returns_intercepted_value_when_ReturnInterceptedRefType_is_called()
            {
                Execute(() =>
                {
                    var instance = new InterceptMethodsClass();
                    var result = instance.ReturnInterceptedRefType();

                    Assert.Equal("ab", result);
                });
            }

            [Fact]
            private void returns_original_value_when_ReturnOriginalExternalValueType_is_called()
            {
                Execute(() =>
                {
                    var instance = new InterceptMethodsClass();
                    var result = instance.ReturnOriginalExternalValueType();

                    Assert.Equal(1, result.Value);
                });
            }

            [Fact]
            private void returns_intercepted_value_when_ReturnInterceptedExternalValueType_is_called()
            {
                Execute(() =>
                {
                    var instance = new InterceptMethodsClass();
                    var result = instance.ReturnInterceptedExternalValueType();

                    Assert.Equal(2, result.Value);
                });
            }

            [Fact]
            private void returns_original_value_when_ReturnOriginalExternalRefType_is_called()
            {
                Execute(() =>
                {
                    var instance = new InterceptMethodsClass();
                    var result = instance.ReturnOriginalExternalRefType();

                    Assert.Equal(1, result.Value);
                });
            }

            [Fact]
            private void returns_intercepted_value_when_ReturnInterceptedExternalRefType_is_called()
            {
                Execute(() =>
                {
                    var instance = new InterceptMethodsClass();
                    var result = instance.ReturnInterceptedExternalRefType();

                    Assert.Equal(2, result.Value);
                });
            }
        }

        public class Arrays
        {
            [Fact]
            private void returns_original_value_when_ReturnOriginalValueTypeArray_is_called()
            {
                Execute(() =>
                {
                    var instance = new InterceptMethodsClass();
                    var result = instance.ReturnOriginalValueTypeArray();

                    Assert.Single(result);
                    Assert.Equal(1, result[0]);
                });
            }

            [Fact]
            private void returns_intercepted_value_when_ReturnInterceptedValueTypeArray_is_called()
            {
                Execute(() =>
                {
                    var instance = new InterceptMethodsClass();
                    var result = instance.ReturnInterceptedValueTypeArray();

                    Assert.Equal(2, result.Length);
                    Assert.Equal(2, result[0]);
                    Assert.Equal(1, result[1]);
                });
            }

            [Fact]
            private void returns_original_value_when_ReturnOriginalRefTypeArray_is_called()
            {
                Execute(() =>
                {
                    var instance = new InterceptMethodsClass();
                    var result = instance.ReturnOriginalRefTypeArray();

                    Assert.Single(result);
                    Assert.Equal("a", result[0]);
                });
            }

            [Fact]
            private void returns_intercepted_value_when_ReturnInterceptedRefTypeArray_is_called()
            {
                Execute(() =>
                {
                    var instance = new InterceptMethodsClass();
                    var result = instance.ReturnInterceptedRefTypeArray();

                    Assert.Equal(2, result.Length);
                    Assert.Equal("cd", result[0]);
                    Assert.Equal("ab", result[1]);
                });
            }

            [Fact]
            private void returns_original_value_when_ReturnOriginalExternalValueTypeArray_is_called()
            {
                Execute(() =>
                {
                    var instance = new InterceptMethodsClass();
                    var result = instance.ReturnOriginalExternalValueTypeArray();

                    Assert.Single(result);
                    Assert.Equal(1, result[0].Value);
                });
            }

            [Fact]
            private void returns_intercepted_value_when_ReturnInterceptedExternalValueTypeArray_is_called()
            {
                Execute(() =>
                {
                    var instance = new InterceptMethodsClass();
                    var result = instance.ReturnInterceptedExternalValueTypeArray();

                    Assert.Equal(2, result.Length);
                    Assert.Equal(2, result[0].Value);
                    Assert.Equal(1, result[1].Value);
                });
            }

            [Fact]
            private void returns_original_value_when_ReturnOriginalExternalRefTypeArray_is_called()
            {
                Execute(() =>
                {
                    var instance = new InterceptMethodsClass();
                    var result = instance.ReturnOriginalExternalRefTypeArray();

                    Assert.Single(result);
                    Assert.Equal(1, result[0].Value);
                });
            }

            [Fact]
            private void returns_intercepted_value_when_ReturnInterceptedExternalRefTypeArray_is_called()
            {
                Execute(() =>
                {
                    var instance = new InterceptMethodsClass();
                    var result = instance.ReturnInterceptedExternalRefTypeArray();

                    Assert.Equal(2, result.Length);
                    Assert.Equal(2, result[0].Value);
                    Assert.Equal(1, result[1].Value);
                });
            }
        }

        public class Lists
        {
            [Fact]
            private void returns_original_value_when_ReturnOriginalValueTypeList_is_called()
            {
                Execute(() =>
                {
                    var instance = new InterceptMethodsClass();
                    var result = instance.ReturnOriginalValueTypeList();

                    Assert.Single(result);
                    Assert.Equal(1, result[0]);
                });
            }

            [Fact]
            private void returns_intercepted_value_when_ReturnInterceptedValueTypeList_is_called()
            {
                Execute(() =>
                {
                    var instance = new InterceptMethodsClass();
                    var result = instance.ReturnInterceptedValueTypeList();

                    Assert.Equal(2, result.Count);
                    Assert.Equal(2, result[0]);
                    Assert.Equal(1, result[1]);
                });
            }

            [Fact]
            private void returns_original_value_when_ReturnOriginalRefTypeList_is_called()
            {
                Execute(() =>
                {
                    var instance = new InterceptMethodsClass();
                    var result = instance.ReturnOriginalRefTypeList();

                    Assert.Single(result);
                    Assert.Equal("a", result[0]);
                });
            }

            [Fact]
            private void returns_intercepted_value_when_ReturnInterceptedRefTypeList_is_called()
            {
                Execute(() =>
                {
                    var instance = new InterceptMethodsClass();
                    var result = instance.ReturnInterceptedRefTypeList();

                    Assert.Equal(2, result.Count);
                    Assert.Equal("cd", result[0]);
                    Assert.Equal("ab", result[1]);
                });
            }

            [Fact]
            private void returns_original_value_when_ReturnOriginalExternalValueTypeList_is_called()
            {
                Execute(() =>
                {
                    var instance = new InterceptMethodsClass();
                    var result = instance.ReturnOriginalExternalValueTypeList();

                    Assert.Single(result);
                    Assert.Equal(1, result[0].Value);
                });
            }

            [Fact]
            private void returns_intercepted_value_when_ReturnInterceptedExternalValueTypev_is_called()
            {
                Execute(() =>
                {
                    var instance = new InterceptMethodsClass();
                    var result = instance.ReturnInterceptedExternalValueTypeList();

                    Assert.Equal(2, result.Count);
                    Assert.Equal(2, result[0].Value);
                    Assert.Equal(1, result[1].Value);
                });
            }

            [Fact]
            private void returns_original_value_when_ReturnOriginalExternalRefTypev_is_called()
            {
                Execute(() =>
                {
                    var instance = new InterceptMethodsClass();
                    var result = instance.ReturnOriginalExternalRefTypeList();

                    Assert.Single(result);
                    Assert.Equal(1, result[0].Value);
                });
            }

            [Fact]
            private void returns_intercepted_value_when_ReturnInterceptedExternalRefTypeList_is_called()
            {
                Execute(() =>
                {
                    var instance = new InterceptMethodsClass();
                    var result = instance.ReturnInterceptedExternalRefTypeList();

                    Assert.Equal(2, result.Count);
                    Assert.Equal(2, result[0].Value);
                    Assert.Equal(1, result[1].Value);
                });
            }
        }

        public class SimpleParameters
        {
            [Fact]
            private void return_string_with_original_parameters_values_when_ReturnOriginalParametersAsString_is_called()
            {
                Execute(() =>
                {
                    var instance = new InterceptMethodsClass();
                    var p1 = 1;
                    var p2 = "a";
                    var p3 = new SimpleStruct(1);
                    var p4 = new SimpleClass(1);
                    var p5 = new[] { 1 };
                    var p6 = new[] { "a" };
                    var p7 = new[] { new SimpleStruct(1) };
                    var p8 = new[] { new SimpleClass(1) };
                    var p9 = new List<int> { 1 };
                    var p10 = new List<string> { "a" };
                    var p11 = new List<SimpleStruct> { new SimpleStruct(1) };
                    var p12 = new List<SimpleClass> { new SimpleClass(1) };
                    var expected = Helper.AsString(p1, p2, p3, p4, p5, p6, p7, p8, p9, p10, p11, p12);

                    var result = instance.ReturnOriginalParametersAsString(p1, p2, p3, p4, p5, p6, p7, p8, p9, p10, p11, p12);

                    Assert.Equal(expected, result);
                });
            }

            [Fact]
            private void return_string_with_intercepted_parameters_values_when_ReturnInterceptedParametersAsString_is_called()
            {
                Execute(() =>
                {
                    var instance = new InterceptMethodsClass();
                    var interceptedP1 = 2;
                    var interceptedP2 = "ab";
                    var interceptedP3 = new SimpleStruct(2);
                    var interceptedP4 = new SimpleClass(2);
                    var interceptedP5 = new[] { 2, 1 };
                    var interceptedP6 = new[] { "cd", "ab" };
                    var interceptedP7 = new[] { new SimpleStruct(2), new SimpleStruct(1) };
                    var interceptedP8 = new[] { new SimpleClass(2), new SimpleClass(1) };
                    var interceptedP9 = new List<int> { 2, 1 };
                    var interceptedP10 = new List<string> { "cd", "ab" };
                    var interceptedP11 = new List<SimpleStruct> { new SimpleStruct(2), new SimpleStruct(1) };
                    var interceptedP12 = new List<SimpleClass> { new SimpleClass(2), new SimpleClass(1) };
                    var expected = Helper.AsString(interceptedP1, interceptedP2, interceptedP3, interceptedP4, interceptedP5, interceptedP6, interceptedP7, interceptedP8, interceptedP9, interceptedP10, interceptedP11, interceptedP12);

                    var originalP1 = 1;
                    var originalP2 = "a";
                    var originalP3 = new SimpleStruct(1);
                    var originalP4 = new SimpleClass(1);
                    var originalP5 = new[] { 1 };
                    var originalP6 = new[] { "a" };
                    var originalP7 = new[] { new SimpleStruct(1) };
                    var originalP8 = new[] { new SimpleClass(1) };
                    var originalP9 = new List<int> { 1 };
                    var originalP10 = new List<string> { "a" };
                    var originalP11 = new List<SimpleStruct> { new SimpleStruct(1) };
                    var originalP12 = new List<SimpleClass> { new SimpleClass(1) };
                    var result = instance.ReturnInterceptedParametersAsString(originalP1, originalP2, originalP3, originalP4, originalP5, originalP6, originalP7, originalP8, originalP9, originalP10, originalP11, originalP12);

                    Assert.Equal(expected, result);
                });
            }
        }

        public class RefParameters
        {
            [Fact]
            private void return_string_with_original_parameters_values_when_ReturnOriginalRefParametersAsString_is_called()
            {
                Execute(() =>
                {
                    var instance = new InterceptMethodsClass();
                    var p1 = 1;
                    var p2 = "a";
                    var p3 = new SimpleStruct(1);
                    var p4 = new SimpleClass(1);
                    var p5 = new[] { 1 };
                    var p6 = new[] { "a" };
                    var p7 = new[] { new SimpleStruct(1) };
                    var p8 = new[] { new SimpleClass(1) };
                    var p9 = new List<int> { 1 };
                    var p10 = new List<string> { "a" };
                    var p11 = new List<SimpleStruct> { new SimpleStruct(1) };
                    var p12 = new List<SimpleClass> { new SimpleClass(1) };
                    var expected = Helper.AsString(p1, p2, p3, p4, p5, p6, p7, p8, p9, p10, p11, p12);

                    var ref1 = p1;
                    var ref2 = p2;
                    var ref3 = p3;
                    var ref4 = p4;
                    var ref5 = p5;
                    var ref6 = p6;
                    var ref7 = p7;
                    var ref8 = p8;
                    var ref9 = p9;
                    var ref10 = p10;
                    var ref11 = p11;
                    var ref12 = p12;
                    var result = instance.ReturnOriginalRefParametersAsString(ref ref1, ref ref2, ref ref3, ref ref4, ref ref5, ref ref6, ref ref7, ref ref8, ref ref9, ref ref10, ref ref11, ref ref12);

                    Assert.Equal(expected, result);
                    Assert.Equal(p1, ref1);
                    Assert.Same(p2, ref2);
                    Assert.Equal(p3, ref3);
                    Assert.Same(p4, ref4);
                    Assert.Same(p5, ref5);
                    Assert.Same(p6, ref6);
                    Assert.Same(p7, ref7);
                    Assert.Same(p8, ref8);
                    Assert.Same(p9, ref9);
                    Assert.Same(p10, ref10);
                    Assert.Same(p11, ref11);
                    Assert.Same(p12, ref12);
                });
            }

            [Fact]
            private void return_string_with_intercepted_parameters_values_when_ReturnInterceptedRefParametersAsString_is_called()
            {
                Execute(() =>
                {
                    var instance = new InterceptMethodsClass();
                    var interceptedP1 = 2;
                    var interceptedP2 = "ab";
                    var interceptedP3 = new SimpleStruct(2);
                    var interceptedP4 = new SimpleClass(2);
                    var interceptedP5 = new[] { 2, 1 };
                    var interceptedP6 = new[] { "cd", "ab" };
                    var interceptedP7 = new[] { new SimpleStruct(2), new SimpleStruct(1) };
                    var interceptedP8 = new[] { new SimpleClass(2), new SimpleClass(1) };
                    var interceptedP9 = new List<int> { 2, 1 };
                    var interceptedP10 = new List<string> { "cd", "ab" };
                    var interceptedP11 = new List<SimpleStruct> { new SimpleStruct(2), new SimpleStruct(1) };
                    var interceptedP12 = new List<SimpleClass> { new SimpleClass(2), new SimpleClass(1) };
                    var expected = Helper.AsString(interceptedP1, interceptedP2, interceptedP3, interceptedP4, interceptedP5, interceptedP6, interceptedP7, interceptedP8, interceptedP9, interceptedP10, interceptedP11, interceptedP12);

                    var p1 = 1;
                    var p2 = "a";
                    var p3 = new SimpleStruct(1);
                    var p4 = new SimpleClass(1);
                    var p5 = new[] { 1 };
                    var p6 = new[] { "a" };
                    var p7 = new[] { new SimpleStruct(1) };
                    var p8 = new[] { new SimpleClass(1) };
                    var p9 = new List<int> { 1 };
                    var p10 = new List<string> { "a" };
                    var p11 = new List<SimpleStruct> { new SimpleStruct(1) };
                    var p12 = new List<SimpleClass> { new SimpleClass(1) };
                    var ref1 = p1;
                    var ref2 = p2;
                    var ref3 = p3;
                    var ref4 = p4;
                    var ref5 = p5;
                    var ref6 = p6;
                    var ref7 = p7;
                    var ref8 = p8;
                    var ref9 = p9;
                    var ref10 = p10;
                    var ref11 = p11;
                    var ref12 = p12;
                    var result = instance.ReturnInterceptedRefParametersAsString(ref ref1, ref ref2, ref ref3, ref ref4, ref ref5, ref ref6, ref ref7, ref ref8, ref ref9, ref ref10, ref ref11, ref ref12);

                    Assert.Equal(expected, result);
                    Assert.NotEqual(p1, ref1);
                    Assert.NotSame(p2, ref2);
                    Assert.NotEqual(p3, ref3);
                    Assert.NotSame(p4, ref4);
                    Assert.NotSame(p5, ref5);
                    Assert.NotSame(p6, ref6);
                    Assert.NotSame(p7, ref7);
                    Assert.NotSame(p8, ref8);
                    Assert.NotSame(p9, ref9);
                    Assert.NotSame(p10, ref10);
                    Assert.NotSame(p11, ref11);
                    Assert.NotSame(p12, ref12);
                });
            }
        }

        public class OutParameters
        {
            [Fact]
            private void return_string_with_original_parameters_values_when_ReturnOriginalOutParameters_is_called()
            {
                Execute(() =>
                {
                    var instance = new InterceptMethodsClass();
                    var p1 = 1;
                    var p2 = "a";
                    var p3 = new SimpleStruct(1);
                    var p4 = new SimpleClass(1);
                    var p5 = new[] { 1 };
                    var p6 = new[] { "a" };
                    var p7 = new[] { new SimpleStruct(1) };
                    var p8 = new[] { new SimpleClass(1) };
                    var p9 = new List<int> { 1 };
                    var p10 = new List<string> { "a" };
                    var p11 = new List<SimpleStruct> { new SimpleStruct(1) };
                    var p12 = new List<SimpleClass> { new SimpleClass(1) };

                    int out1;
                    string out2;
                    SimpleStruct out3;
                    SimpleClass out4;
                    int[] out5;
                    string[] out6;
                    SimpleStruct[] out7;
                    SimpleClass[] out8;
                    List<int> out9;
                    List<string> out10;
                    List<SimpleStruct> out11;
                    List<SimpleClass> out12;
                    instance.ReturnOriginalOutParameters(out out1, out out2, out out3, out out4, out out5, out out6, out out7, out out8, out out9, out out10, out out11, out out12);

                    Assert.Equal(p1, out1);
                    Assert.Equal(p2, out2);
                    Assert.Equal(p3.Value, out3.Value);
                    Assert.Equal(p4.Value, out4.Value);
                    Assert.Equal(p5[0], out5[0]);
                    Assert.Equal(p6[0], out6[0]);
                    Assert.Equal(p7[0].Value, out7[0].Value);
                    Assert.Equal(p8[0].Value, out8[0].Value);
                    Assert.Equal(p9[0], out9[0]);
                    Assert.Equal(p10[0], out10[0]);
                    Assert.Equal(p11[0].Value, out11[0].Value);
                    Assert.Equal(p12[0].Value, out12[0].Value);
                });
            }

            [Fact]
            private void return_string_with_intercepted_parameters_values_when_ReturnInterceptedOutParameters_is_called()
            {
                Execute(() =>
                {
                    var instance = new InterceptMethodsClass();
                    var interceptedP1 = 2;
                    var interceptedP2 = "ab";
                    var interceptedP3 = new SimpleStruct(2);
                    var interceptedP4 = new SimpleClass(2);
                    var interceptedP5 = new[] { 2, 1 };
                    var interceptedP6 = new[] { "cd", "ab" };
                    var interceptedP7 = new[] { new SimpleStruct(2), new SimpleStruct(1) };
                    var interceptedP8 = new[] { new SimpleClass(2), new SimpleClass(1) };
                    var interceptedP9 = new List<int> { 2, 1 };
                    var interceptedP10 = new List<string> { "cd", "ab" };
                    var interceptedP11 = new List<SimpleStruct> { new SimpleStruct(2), new SimpleStruct(1) };
                    var interceptedP12 = new List<SimpleClass> { new SimpleClass(2), new SimpleClass(1) };

                    int out1;
                    string out2;
                    SimpleStruct out3;
                    SimpleClass out4;
                    int[] out5;
                    string[] out6;
                    SimpleStruct[] out7;
                    SimpleClass[] out8;
                    List<int> out9;
                    List<string> out10;
                    List<SimpleStruct> out11;
                    List<SimpleClass> out12;
                    instance.ReturnInterceptedOutParameters(out out1, out out2, out out3, out out4, out out5, out out6, out out7, out out8, out out9, out out10, out out11, out out12);

                    Assert.Equal(interceptedP1, out1);
                    Assert.Equal(interceptedP2, out2);
                    Assert.Equal(interceptedP3.Value, out3.Value);
                    Assert.Equal(interceptedP4.Value, out4.Value);
                    Assert.Equal(interceptedP5[0], out5[0]);
                    Assert.Equal(interceptedP6[0], out6[0]);
                    Assert.Equal(interceptedP7[0].Value, out7[0].Value);
                    Assert.Equal(interceptedP8[0].Value, out8[0].Value);
                    Assert.Equal(interceptedP9[0], out9[0]);
                    Assert.Equal(interceptedP10[0], out10[0]);
                    Assert.Equal(interceptedP11[0].Value, out11[0].Value);
                    Assert.Equal(interceptedP12[0].Value, out12[0].Value);
                });
            }
        }

        public class ChangeMethodBody
        {
            [Fact]
            private void change_method_body_so_that_is_subtracts_two_numbers_instead_of_adding_them()
            {
                Execute(() =>
                {
                    var instance = new InterceptMethodsClass();
                    var result = instance.ChangeSumToSubtract(2, 3);

                    Assert.Equal(-1, result);
                });
            }
        }
    }
}
