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
using System;

namespace CodeLoom.Tests
{
    public class InterceptGenericMethodsTest : BaseTest
    {
        /*
        public class SimpleMethods
        {
            [Fact]
            private void returns_original_value_when_ReturnOriginalValueFromFirstParameter_is_called()
            {
                Execute(() =>
                {
                    var instance = new InterceptGenericMethodsClass<int, string>();
                    var result = instance.ReturnOriginalValueFromFirstParameter(1, "a");

                    Assert.Equal(1, result);
                });
            }

            [Fact]
            private void returns_intercepted_value_when_ReturnInterceptedValueFromFirstParameter_is_called()
            {
                Execute(() =>
                {
                    var instance = new InterceptGenericMethodsClass<int, string>();
                    var result = instance.ReturnInterceptedValueFromFirstParameter(1, "a");

                    Assert.Equal(2, result);
                });
            }

            [Fact]
            private void returns_original_parameters_as_string_when_ReturnOriginalParametersAsString_is_called()
            {
                Execute(() =>
                {
                    var instance = new InterceptGenericMethodsClass<int, string>();
                    var p1 = 1;
                    var p2 = "a";
                    var expected = Helper.AsString(p1, p2);

                    var result = instance.ReturnOriginalParametersAsString(p1, p2);

                    Assert.Equal(expected, result);
                });
            }

            [Fact]
            private void returns_intercepted_parameters_as_string_when_ReturnInterceptedParametersAsString_is_called()
            {
                Execute(() =>
                {
                    var instance = new InterceptGenericMethodsClass<int, string>();
                    var interceptedP1 = 2;
                    var interceptedP2 = "ab";
                    var expected = Helper.AsString(interceptedP1, interceptedP2);

                    var originalP1 = 1;
                    var originalP2 = "a";
                    var result = instance.ReturnInterceptedParametersAsString(originalP1, originalP2);

                    Assert.Equal(expected, result);
                });
            }

            [Fact]
            private void returns_original_parameters_as_string_when_ReturnOriginalRefParametersAsString_is_called()
            {
                Execute(() =>
                {
                    var instance = new InterceptGenericMethodsClass<int, string>();
                    var p1 = 1;
                    var p2 = "a";
                    var expected = Helper.AsString(p1, p2);

                    var ref1 = p1;
                    var ref2 = p2;
                    var result = instance.ReturnOriginalRefParametersAsString(ref ref1, ref ref2);

                    Assert.Equal(expected, result);
                    Assert.Equal(p1, ref1);
                    Assert.Same(p2, ref2);
                });
            }

            [Fact]
            private void returns_intercepted_parameters_as_string_when_ReturnInterceptedRefParametersAsString_is_called()
            {
                Execute(() =>
                {
                    var instance = new InterceptGenericMethodsClass<int, string>();
                    var interceptedP1 = 2;
                    var interceptedP2 = "ab";
                    var expected = Helper.AsString(interceptedP1, interceptedP2);

                    var p1 = 1;
                    var p2 = "a";
                    var ref1 = p1;
                    var ref2 = p2;
                    var result = instance.ReturnInterceptedRefParametersAsString(ref ref1, ref ref2);

                    Assert.Equal(expected, result);
                    Assert.NotEqual(p1, ref1);
                    Assert.NotSame(p2, ref2);
                });
            }

            [Fact]
            private void returns_original_parameters_as_string_when_ReturnOriginalOutParameters_is_called()
            {
                Execute(() =>
                {
                    var instance = new InterceptGenericMethodsClass<int, string>();
                    var p1 = default(int);
                    var p2 = default(string);
                    var p3 = new[] { default(int) };
                    var p4 = new[] { default(string) };
                    var p5 = new List<int> { default(int) };
                    var p6= new List<string> { default(string) };

                    int out1;
                    string out2;
                    int[] out3;
                    string[] out4;
                    List<int> out5;
                    List<string> out6;
                    instance.ReturnOriginalOutParameters(out out1, out out2, out out3, out out4, out out5, out out6);

                    Assert.Equal(p1, out1);
                    Assert.Equal(p2, out2);
                    Assert.Equal(p3[0], out3[0]);
                    Assert.Equal(p4[0], out4[0]);
                    Assert.Equal(p5[0], out5[0]);
                    Assert.Equal(p6[0], out6[0]);
                });
            }

            [Fact]
            private void returns_intercepted_parameters_as_string_when_ReturnInterceptedOutParameters_is_called()
            {
                Execute(() =>
                {
                    var instance = new InterceptGenericMethodsClass<int, string>();
                    var interceptedP1 = 2;
                    var interceptedP2 = "ab";
                    var interceptedP3 = new[] { 1, 2 };
                    var interceptedP4 = new[] { "ab", "cd" };
                    var interceptedP5 = new List<int> { 1, 2 };
                    var interceptedP6 = new List<string> { "ab", "cd" };

                    int out1;
                    string out2;
                    int[] out3;
                    string[] out4;
                    List<int> out5;
                    List<string> out6;
                    instance.ReturnInterceptedOutParameters(out out1, out out2, out out3, out out4, out out5, out out6);

                    Assert.Equal(interceptedP1, out1);
                    Assert.Equal(interceptedP2, out2);
                    Assert.Equal(interceptedP3[0], out3[0]);
                    Assert.Equal(interceptedP3[1], out3[1]);
                    Assert.Equal(interceptedP4[0], out4[0]);
                    Assert.Equal(interceptedP4[1], out4[1]);
                    Assert.Equal(interceptedP5[0], out5[0]);
                    Assert.Equal(interceptedP5[1], out5[1]);
                    Assert.Equal(interceptedP6[0], out6[0]);
                    Assert.Equal(interceptedP6[1], out6[1]);
                });
            }
        }

        public class GenericMethods
        {
            [Fact]
            private void returns_original_value_when_GenericReturnOriginalValueFromFirstParameter_is_called()
            {
                Execute(() =>
                {
                    var instance = new InterceptGenericMethodsClass<int, string>();
                    var result = instance.GenericReturnOriginalValueFromFirstParameter<SimpleStruct, SimpleClass>(1, "a", new SimpleStruct(1), new SimpleClass(1));

                    Assert.Equal(1, result);
                });
            }

            [Fact]
            private void returns_intercepted_value_when_GenericReturnInterceptedValueFromFirstParameter_is_called()
            {
                Execute(() =>
                {
                    var instance = new InterceptGenericMethodsClass<int, string>();
                    var result = instance.GenericReturnInterceptedValueFromFirstParameter<SimpleStruct, SimpleClass>(1, "a", new SimpleStruct(1), new SimpleClass(1));

                    Assert.Equal(2, result);
                });
            }

            [Fact]
            private void returns_original_parameters_as_string_when_GenericReturnOriginalParametersAsString_is_called()
            {
                Execute(() =>
                {
                    var instance = new InterceptGenericMethodsClass<int, string>();
                    var p1 = 1;
                    var p2 = "a";
                    var p3 = new SimpleStruct(1);
                    var p4 = new SimpleClass(1);
                    var expected = Helper.AsString(p1, p2, p3, p4);

                    var result = instance.GenericReturnOriginalParametersAsString<SimpleStruct, SimpleClass>(p1, p2, p3, p4);

                    Assert.Equal(expected, result);
                });
            }

            [Fact]
            private void returns_intercepted_parameters_as_string_when_GenericReturnInterceptedParametersAsString_is_called()
            {
                Execute(() =>
                {
                    var instance = new InterceptGenericMethodsClass<int, string>();
                    var interceptedP1 = 2;
                    var interceptedP2 = "ab";
                    var interceptedP3 = new SimpleStruct(2);
                    var interceptedP4 = new SimpleClass(2);
                    var expected = Helper.AsString(interceptedP1, interceptedP2, interceptedP3, interceptedP4);

                    var originalP1 = 1;
                    var originalP2 = "a";
                    var originalP3 = new SimpleStruct(1);
                    var originalP4 = new SimpleClass(1);
                    var result = instance.GenericReturnInterceptedParametersAsString<SimpleStruct, SimpleClass>(originalP1, originalP2, originalP3, originalP4);

                    Assert.Equal(expected, result);
                });
            }

            [Fact]
            private void returns_original_parameters_as_string_when_GenericReturnOriginalRefParametersAsString_is_called()
            {
                Execute(() =>
                {
                    var instance = new InterceptGenericMethodsClass<int, string>();
                    var p1 = 1;
                    var p2 = "a";
                    var p3 = new SimpleStruct(1);
                    var p4 = new SimpleClass(1);
                    var expected = Helper.AsString(p1, p2, p3, p4);

                    var ref1 = p1;
                    var ref2 = p2;
                    var ref3 = p3;
                    var ref4 = p4;
                    var result = instance.GenericReturnOriginalRefParametersAsString<SimpleStruct, SimpleClass>(ref ref1, ref ref2, ref ref3, ref ref4);

                    Assert.Equal(expected, result);
                    Assert.Equal(p1, ref1);
                    Assert.Same(p2, ref2);
                    Assert.Equal(p3, ref3);
                    Assert.Same(p4, ref4);
                });
            }

            [Fact]
            private void returns_intercepted_parameters_as_string_when_GenericReturnInterceptedRefParametersAsString_is_called()
            {
                Execute(() =>
                {
                    var instance = new InterceptGenericMethodsClass<int, string>();
                    var interceptedP1 = 2;
                    var interceptedP2 = "ab";
                    var interceptedP3 = new SimpleStruct(2);
                    var interceptedP4 = new SimpleClass(2);
                    var expected = Helper.AsString(interceptedP1, interceptedP2, interceptedP3, interceptedP4);

                    var p1 = 1;
                    var p2 = "a";
                    var p3 = new SimpleStruct(1);
                    var p4 = new SimpleClass(1);
                    var ref1 = p1;
                    var ref2 = p2;
                    var ref3 = p3;
                    var ref4 = p4;
                    var result = instance.GenericReturnInterceptedRefParametersAsString<SimpleStruct, SimpleClass>(ref ref1, ref ref2, ref ref3, ref ref4);

                    Assert.Equal(expected, result);
                    Assert.NotEqual(p1, ref1);
                    Assert.NotSame(p2, ref2);
                    Assert.NotEqual(p3, ref3);
                    Assert.NotSame(p4, ref4);
                });
            }

            [Fact]
            private void returns_original_parameters_as_string_when_GenericReturnOriginalOutParameters_is_called()
            {
                Execute(() =>
                {
                    var instance = new InterceptGenericMethodsClass<int, string>();
                    var p1 = default(int);
                    var p2 = default(string);
                    var p3 = default(SimpleStruct);
                    var p4 = default(SimpleClass);
                    var p5 = new[] { default(int) };
                    var p6 = new[] { default(string) };
                    var p7 = new[] { default(SimpleStruct) };
                    var p8 = new[] { default(SimpleClass) };
                    var p9 = new List<int> { default(int) };
                    var p10 = new List<string> { default(string) };
                    var p11 = new List<SimpleStruct> { default(SimpleStruct) };
                    var p12 = new List<SimpleClass> { default(SimpleClass) };

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
                    instance.GenericReturnOriginalOutParameters<SimpleStruct, SimpleClass>(out out1, out out2, out out3, out out4, out out5, out out6, out out7, out out8, out out9, out out10, out out11, out out12);

                    Assert.Equal(p1, out1);
                    Assert.Equal(p2, out2);
                    Assert.Equal(p3, out3);
                    Assert.Equal(p4, out4);
                    Assert.Equal(p5[0], out5[0]);
                    Assert.Equal(p6[0], out6[0]);
                    Assert.Equal(p7[0], out7[0]);
                    Assert.Equal(p8[0], out8[0]);
                    Assert.Equal(p9[0], out9[0]);
                    Assert.Equal(p10[0], out10[0]);
                    Assert.Equal(p11[0], out11[0]);
                    Assert.Equal(p12[0], out12[0]);
                });
            }

            [Fact]
            private void returns_intercepted_parameters_as_string_when_GenericReturnInterceptedOutParameters_is_called()
            {
                Execute(() =>
                {
                    var instance = new InterceptGenericMethodsClass<int, string>();
                    var interceptedP1 = 2;
                    var interceptedP2 = "ab";
                    var interceptedP3 = new SimpleStruct(2);
                    var interceptedP4 = new SimpleClass(2);
                    var interceptedP5 = new[] { 1, 2 };
                    var interceptedP6 = new[] { "ab", "cd" };
                    var interceptedP7 = new[] { new SimpleStruct(1), new SimpleStruct(2) };
                    var interceptedP8 = new[] { new SimpleClass(1), new SimpleClass(2) };
                    var interceptedP9 = new List<int> { 1, 2 };
                    var interceptedP10 = new List<string> { "ab", "cd" };
                    var interceptedP11 = new List<SimpleStruct> { new SimpleStruct(1), new SimpleStruct(2) };
                    var interceptedP12 = new List<SimpleClass> { new SimpleClass(1), new SimpleClass(2) };

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
                    instance.GenericReturnInterceptedOutParameters<SimpleStruct, SimpleClass>(out out1, out out2, out out3, out out4, out out5, out out6, out out7, out out8, out out9, out out10, out out11, out out12);

                    Assert.Equal(interceptedP1, out1);
                    Assert.Equal(interceptedP2, out2);
                    Assert.Equal(interceptedP3.Value, out3.Value);
                    Assert.Equal(interceptedP4.Value, out4.Value);
                    Assert.Equal(interceptedP5[0], out5[0]);
                    Assert.Equal(interceptedP5[1], out5[1]);
                    Assert.Equal(interceptedP6[0], out6[0]);
                    Assert.Equal(interceptedP6[1], out6[1]);
                    Assert.Equal(interceptedP7[0].Value, out7[0].Value);
                    Assert.Equal(interceptedP7[1].Value, out7[1].Value);
                    Assert.Equal(interceptedP8[0].Value, out8[0].Value);
                    Assert.Equal(interceptedP8[1].Value, out8[1].Value);
                    Assert.Equal(interceptedP9[0], out9[0]);
                    Assert.Equal(interceptedP9[1], out9[1]);
                    Assert.Equal(interceptedP10[0], out10[0]);
                    Assert.Equal(interceptedP10[1], out10[1]);
                    Assert.Equal(interceptedP11[0].Value, out11[0].Value);
                    Assert.Equal(interceptedP11[1].Value, out11[1].Value);
                    Assert.Equal(interceptedP12[0].Value, out12[0].Value);
                    Assert.Equal(interceptedP12[1].Value, out12[1].Value);
                });
            }
        }
        */

        public class GenericMethodsWithRepeatedGenericParameter
        {
            [Fact]
            private void returns_original_value_when_GenericWithRepeatedGenericParameterReturnOriginalValueFromFirstParameter_is_called()
            {
                Execute(() =>
                {
                    var instance = new InterceptGenericMethodsClass<int, string>();
                    var result = instance.GenericWithRepeatedGenericParameterReturnOriginalValueFromFirstParameter<SimpleClass, SimpleStruct>(1, new SimpleStruct(1), new SimpleClass(1));

                    Assert.Equal(1, result);
                });
            }

            /*
            [Fact]
            private void returns_intercepted_value_when_GenericWithRepeatedGenericParameterReturnInterceptedValueFromFirstParameter_is_called()
            {
                Execute(() =>
                {
                    var instance = new InterceptGenericMethodsClass<int, string>();
                    var result = instance.GenericWithRepeatedGenericParameterReturnInterceptedValueFromFirstParameter<SimpleClass, SimpleStruct>(1, new SimpleStruct(1), new SimpleClass(1));

                    Assert.Equal(2, result);
                });
            }

            [Fact]
            private void returns_original_parameters_as_string_when_GenericWithRepeatedGenericParameterReturnOriginalParametersAsString_is_called()
            {
                Execute(() =>
                {
                    var instance = new InterceptGenericMethodsClass<int, string>();
                    var p1 = 1;
                    var p2 = new SimpleStruct(1);
                    var p3 = new SimpleClass(1);
                    var expected = Helper.AsString(p1, p2, p3);

                    var result = instance.GenericWithRepeatedGenericParameterReturnOriginalParametersAsString<SimpleClass, SimpleStruct>(p1, p2, p3);

                    Assert.Equal(expected, result);
                });
            }

            [Fact]
            private void returns_intercepted_parameters_as_string_when_GenericWithRepeatedGenericParameterReturnInterceptedParametersAsString_is_called()
            {
                Execute(() =>
                {
                    var instance = new InterceptGenericMethodsClass<int, string>();
                    var interceptedP1 = 2;
                    var interceptedP2 = new SimpleStruct(2);
                    var interceptedP3 = new SimpleClass(2);
                    var expected = Helper.AsString(interceptedP1, interceptedP2, interceptedP3);

                    var originalP1 = 1;
                    var originalP2 = new SimpleStruct(1);
                    var originalP3 = new SimpleClass(1);
                    var result = instance.GenericWithRepeatedGenericParameterReturnInterceptedParametersAsString<SimpleClass, SimpleStruct>(originalP1, originalP2, originalP3);

                    Assert.Equal(expected, result);
                });
            }

            [Fact]
            private void returns_original_parameters_as_string_when_GenericWithRepeatedGenericParameterReturnOriginalRefParametersAsString_is_called()
            {
                Execute(() =>
                {
                    var instance = new InterceptGenericMethodsClass<int, string>();
                    var p1 = 1;
                    var p2 = new SimpleStruct(1);
                    var p3 = new SimpleClass(1);
                    var expected = Helper.AsString(p1, p2, p3);

                    var ref1 = p1;
                    var ref2 = p2;
                    var ref3 = p3;
                    var result = instance.GenericWithRepeatedGenericParameterReturnOriginalRefParametersAsString<SimpleClass, SimpleStruct>(ref ref1, ref ref2, ref ref3);

                    Assert.Equal(expected, result);
                    Assert.Equal(p1, ref1);
                    Assert.Equal(p2, ref2);
                    Assert.Same(p3, ref3);
                });
            }

            [Fact]
            private void returns_intercepted_parameters_as_string_when_GenericWithRepeatedGenericParameterReturnInterceptedRefParametersAsString_is_called()
            {
                Execute(() =>
                {
                    var instance = new InterceptGenericMethodsClass<int, string>();
                    var interceptedP1 = 2;
                    var interceptedP2 = new SimpleStruct(2);
                    var interceptedP3 = new SimpleClass(2);
                    var expected = Helper.AsString(interceptedP1, interceptedP2, interceptedP3);

                    var p1 = 1;
                    var p2 = new SimpleStruct(1);
                    var p3 = new SimpleClass(1);
                    var ref1 = p1;
                    var ref2 = p2;
                    var ref3 = p3;
                    var result = instance.GenericWithRepeatedGenericParameterReturnInterceptedRefParametersAsString<SimpleClass, SimpleStruct>(ref ref1, ref ref2, ref ref3);

                    Assert.Equal(expected, result);
                    Assert.NotEqual(p1, ref1);
                    Assert.NotEqual(p2, ref2);
                    Assert.NotSame(p3, ref3);
                });
            }

            [Fact]
            private void returns_original_parameters_as_string_when_GenericWithRepeatedGenericParameterReturnOriginalOutParameters_is_called()
            {
                Execute(() =>
                {
                    var instance = new InterceptGenericMethodsClass<int, string>();
                    var p1 = default(int);
                    var p2 = default(SimpleStruct);
                    var p3 = default(SimpleClass);
                    var p4 = new[] { default(int) };
                    var p5 = new[] { default(SimpleStruct) };
                    var p6 = new[] { default(SimpleClass) };
                    var p7 = new List<int> { default(int) };
                    var p8 = new List<SimpleStruct> { default(SimpleStruct) };
                    var p9 = new List<SimpleClass> { default(SimpleClass) };

                    int out1;
                    SimpleStruct out2;
                    SimpleClass out3;
                    int[] out4;
                    SimpleStruct[] out5;
                    SimpleClass[] out6;
                    List<int> out7;
                    List<SimpleStruct> out8;
                    List<SimpleClass> out9;
                    instance.GenericWithRepeatedGenericParameterReturnOriginalOutParameters<SimpleClass, SimpleStruct>(out out1, out out2, out out3, out out4, out out5, out out6, out out7, out out8, out out9);

                    Assert.Equal(p1, out1);
                    Assert.Equal(p2, out2);
                    Assert.Equal(p3, out3);
                    Assert.Equal(p4[0], out4[0]);
                    Assert.Equal(p5[0], out5[0]);
                    Assert.Equal(p6[0], out6[0]);
                    Assert.Equal(p7[0], out7[0]);
                    Assert.Equal(p8[0], out8[0]);
                    Assert.Equal(p9[0], out9[0]);
                });
            }

            [Fact]
            private void returns_intercepted_parameters_as_string_when_GenericWithRepeatedGenericParameterReturnInterceptedOutParameters_is_called()
            {
                Execute(() =>
                {
                    var instance = new InterceptGenericMethodsClass<int, string>();
                    var interceptedP1 = 2;
                    var interceptedP2 = new SimpleStruct(2);
                    var interceptedP3 = new SimpleClass(2);
                    var interceptedP4 = new[] { 1, 2 };
                    var interceptedP5 = new[] { new SimpleStruct(1), new SimpleStruct(2) };
                    var interceptedP6 = new[] { new SimpleClass(1), new SimpleClass(2) };
                    var interceptedP7 = new List<int> { 1, 2 };
                    var interceptedP8 = new List<SimpleStruct> { new SimpleStruct(1), new SimpleStruct(2) };
                    var interceptedP9 = new List<SimpleClass> { new SimpleClass(1), new SimpleClass(2) };

                    int out1;
                    SimpleStruct out2;
                    SimpleClass out3;
                    int[] out4;
                    SimpleStruct[] out5;
                    SimpleClass[] out6;
                    List<int> out7;
                    List<SimpleStruct> out8;
                    List<SimpleClass> out9;
                    instance.GenericWithRepeatedGenericParameterReturnInterceptedOutParameters<SimpleClass, SimpleStruct>(out out1, out out2, out out3, out out4, out out5, out out6, out out7, out out8, out out9);

                    Assert.Equal(interceptedP1, out1);
                    Assert.Equal(interceptedP2.Value, out2.Value);
                    Assert.Equal(interceptedP3.Value, out3.Value);
                    Assert.Equal(interceptedP4[0], out4[0]);
                    Assert.Equal(interceptedP4[1], out4[1]);
                    Assert.Equal(interceptedP5[0].Value, out5[0].Value);
                    Assert.Equal(interceptedP5[1].Value, out5[1].Value);
                    Assert.Equal(interceptedP6[0].Value, out6[0].Value);
                    Assert.Equal(interceptedP6[1].Value, out6[1].Value);
                    Assert.Equal(interceptedP7[0], out7[0]);
                    Assert.Equal(interceptedP7[1], out7[1]);
                    Assert.Equal(interceptedP8[0].Value, out8[0].Value);
                    Assert.Equal(interceptedP8[1].Value, out8[1].Value);
                    Assert.Equal(interceptedP9[0].Value, out9[0].Value);
                    Assert.Equal(interceptedP9[1].Value, out9[1].Value);
                });
            }
            */
        }
    }
}
