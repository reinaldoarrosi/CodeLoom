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
    public class InterceptGenericMethodsWithConstraintsTest : BaseTest
    {
        public class SimpleMethods
        {
            [Fact]
            private void returns_original_value_when_ReturnOriginalValueFromFirstParameter_is_called()
            {
                Execute(() =>
                {
                    var instance = new InterceptGenericMethodsWithConstraintsClass<InheritsFromSimpleClass, SimpleClass>();
                    var result = instance.ReturnOriginalValueFromFirstParameter(new InheritsFromSimpleClass(1), new SimpleClass(1));

                    Assert.Equal(1, result.Value);
                });
            }

            [Fact]
            private void returns_intercepted_value_when_ReturnInterceptedValueFromFirstParameter_is_called()
            {
                Execute(() =>
                {
                    var instance = new InterceptGenericMethodsWithConstraintsClass<InheritsFromSimpleClass, SimpleClass>();
                    var result = instance.ReturnInterceptedValueFromFirstParameter(new InheritsFromSimpleClass(1), new SimpleClass(1));

                    Assert.Equal(2, result.Value);
                });
            }

            [Fact]
            private void returns_original_parameters_as_string_when_ReturnOriginalParametersAsString_is_called()
            {
                Execute(() =>
                {
                    var instance = new InterceptGenericMethodsWithConstraintsClass<InheritsFromSimpleClass, SimpleClass>();
                    var p1 = new InheritsFromSimpleClass(1);
                    var p2 = new SimpleClass(1);
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
                    var instance = new InterceptGenericMethodsWithConstraintsClass<InheritsFromSimpleClass, SimpleClass>();
                    var interceptedP1 = new InheritsFromSimpleClass(2);
                    var interceptedP2 = new SimpleClass(2);
                    var expected = Helper.AsString(interceptedP1, interceptedP2);

                    var originalP1 = new InheritsFromSimpleClass(1);
                    var originalP2 = new SimpleClass(1);
                    var result = instance.ReturnInterceptedParametersAsString(originalP1, originalP2);

                    Assert.Equal(expected, result);
                });
            }

            [Fact]
            private void returns_original_parameters_as_string_when_ReturnOriginalRefParametersAsString_is_called()
            {
                Execute(() =>
                {
                    var instance = new InterceptGenericMethodsWithConstraintsClass<InheritsFromSimpleClass, SimpleClass>();
                    var p1 = new InheritsFromSimpleClass(1);
                    var p2 = new SimpleClass(1);
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
                    var instance = new InterceptGenericMethodsWithConstraintsClass<InheritsFromSimpleClass, SimpleClass>();
                    var interceptedP1 = new InheritsFromSimpleClass(2);
                    var interceptedP2 = new SimpleClass(2);
                    var expected = Helper.AsString(interceptedP1, interceptedP2);

                    var p1 = new InheritsFromSimpleClass(1);
                    var p2 = new SimpleClass(1);
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
                    var instance = new InterceptGenericMethodsWithConstraintsClass<InheritsFromSimpleClass, SimpleClass>();
                    var p1 = default(InheritsFromSimpleClass);
                    var p2 = default(SimpleClass);
                    var p3 = new[] { default(InheritsFromSimpleClass) };
                    var p4 = new[] { default(SimpleClass) };
                    var p5 = new List<InheritsFromSimpleClass> { default(InheritsFromSimpleClass) };
                    var p6 = new List<SimpleClass> { default(SimpleClass) };

                    InheritsFromSimpleClass out1;
                    SimpleClass out2;
                    InheritsFromSimpleClass[] out3;
                    SimpleClass[] out4;
                    List<InheritsFromSimpleClass> out5;
                    List<SimpleClass> out6;
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
                    var instance = new InterceptGenericMethodsWithConstraintsClass<InheritsFromSimpleClass, SimpleClass>();
                    var interceptedP1 = new InheritsFromSimpleClass(2);
                    var interceptedP2 = new SimpleClass(2);
                    var interceptedP3 = new[] { new InheritsFromSimpleClass(1), new InheritsFromSimpleClass(2) };
                    var interceptedP4 = new[] { new SimpleClass(1), new SimpleClass(2) };
                    var interceptedP5 = new List<InheritsFromSimpleClass> { new InheritsFromSimpleClass(1), new InheritsFromSimpleClass(2) };
                    var interceptedP6 = new List<SimpleClass> { new SimpleClass(1), new SimpleClass(2) };

                    InheritsFromSimpleClass out1;
                    SimpleClass out2;
                    InheritsFromSimpleClass[] out3;
                    SimpleClass[] out4;
                    List<InheritsFromSimpleClass> out5;
                    List<SimpleClass> out6;
                    instance.ReturnInterceptedOutParameters(out out1, out out2, out out3, out out4, out out5, out out6);

                    Assert.Equal(interceptedP1.Value, out1.Value);
                    Assert.Equal(interceptedP2.Value, out2.Value);
                    Assert.Equal(interceptedP3[0].Value, out3[0].Value);
                    Assert.Equal(interceptedP3[1].Value, out3[1].Value);
                    Assert.Equal(interceptedP4[0].Value, out4[0].Value);
                    Assert.Equal(interceptedP4[1].Value, out4[1].Value);
                    Assert.Equal(interceptedP5[0].Value, out5[0].Value);
                    Assert.Equal(interceptedP5[1].Value, out5[1].Value);
                    Assert.Equal(interceptedP6[0].Value, out6[0].Value);
                    Assert.Equal(interceptedP6[1].Value, out6[1].Value);
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
                    var instance = new InterceptGenericMethodsWithConstraintsClass<InheritsFromSimpleClass, SimpleClass>();
                    var result = instance.GenericReturnOriginalValueFromFirstParameter<SimpleStruct, SimpleClass>(new InheritsFromSimpleClass(1), new SimpleClass(1), new SimpleStruct(1), new SimpleClass(1));

                    Assert.Equal(1, result.Value);
                });
            }

            [Fact]
            private void returns_intercepted_value_when_GenericReturnInterceptedValueFromFirstParameter_is_called()
            {
                Execute(() =>
                {
                    var instance = new InterceptGenericMethodsWithConstraintsClass<InheritsFromSimpleClass, SimpleClass>();
                    var result = instance.GenericReturnInterceptedValueFromFirstParameter<SimpleStruct, SimpleClass>(new InheritsFromSimpleClass(1), new SimpleClass(1), new SimpleStruct(1), new SimpleClass(1));

                    Assert.Equal(2, result.Value);
                });
            }

            [Fact]
            private void returns_original_parameters_as_string_when_GenericReturnOriginalParametersAsString_is_called()
            {
                Execute(() =>
                {
                    var instance = new InterceptGenericMethodsWithConstraintsClass<InheritsFromSimpleClass, SimpleClass>();
                    var p1 = new InheritsFromSimpleClass(1);
                    var p2 = new SimpleClass(1);
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
                    var instance = new InterceptGenericMethodsWithConstraintsClass<InheritsFromSimpleClass, SimpleClass>();
                    var interceptedP1 = new InheritsFromSimpleClass(2);
                    var interceptedP2 = new SimpleClass(2);
                    var interceptedP3 = new SimpleStruct(2);
                    var interceptedP4 = new SimpleClass(2);
                    var expected = Helper.AsString(interceptedP1, interceptedP2, interceptedP3, interceptedP4);

                    var originalP1 = new InheritsFromSimpleClass(1);
                    var originalP2 = new SimpleClass(1);
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
                    var instance = new InterceptGenericMethodsWithConstraintsClass<InheritsFromSimpleClass, SimpleClass>();
                    var p1 = new InheritsFromSimpleClass(1);
                    var p2 = new SimpleClass(1);
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
                    var instance = new InterceptGenericMethodsWithConstraintsClass<InheritsFromSimpleClass, SimpleClass>();
                    var interceptedP1 = new InheritsFromSimpleClass(2);
                    var interceptedP2 = new SimpleClass(2);
                    var interceptedP3 = new SimpleStruct(2);
                    var interceptedP4 = new SimpleClass(2);
                    var expected = Helper.AsString(interceptedP1, interceptedP2, interceptedP3, interceptedP4);

                    var p1 = new InheritsFromSimpleClass(1);
                    var p2 = new SimpleClass(1);
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
                    var instance = new InterceptGenericMethodsWithConstraintsClass<InheritsFromSimpleClass, SimpleClass>();
                    var p1 = default(InheritsFromSimpleClass);
                    var p2 = default(SimpleClass);
                    var p3 = default(SimpleStruct);
                    var p4 = default(SimpleClass);
                    var p5 = new[] { default(InheritsFromSimpleClass) };
                    var p6 = new[] { default(SimpleClass) };
                    var p7 = new[] { default(SimpleStruct) };
                    var p8 = new[] { default(SimpleClass) };
                    var p9 = new List<InheritsFromSimpleClass> { default(InheritsFromSimpleClass) };
                    var p10 = new List<SimpleClass> { default(SimpleClass) };
                    var p11 = new List<SimpleStruct> { default(SimpleStruct) };
                    var p12 = new List<SimpleClass> { default(SimpleClass) };

                    InheritsFromSimpleClass out1;
                    SimpleClass out2;
                    SimpleStruct out3;
                    SimpleClass out4;
                    InheritsFromSimpleClass[] out5;
                    SimpleClass[] out6;
                    SimpleStruct[] out7;
                    SimpleClass[] out8;
                    List<InheritsFromSimpleClass> out9;
                    List<SimpleClass> out10;
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
                    var instance = new InterceptGenericMethodsWithConstraintsClass<InheritsFromSimpleClass, SimpleClass>();
                    var interceptedP1 = new InheritsFromSimpleClass(2);
                    var interceptedP2 = new SimpleClass(2);
                    var interceptedP3 = new SimpleStruct(2);
                    var interceptedP4 = new SimpleClass(2);
                    var interceptedP5 = new[] { new InheritsFromSimpleClass(1), new InheritsFromSimpleClass(2) };
                    var interceptedP6 = new[] { new SimpleClass(1), new SimpleClass(2) };
                    var interceptedP7 = new[] { new SimpleStruct(1), new SimpleStruct(2) };
                    var interceptedP8 = new[] { new SimpleClass(1), new SimpleClass(2) };
                    var interceptedP9 = new List<InheritsFromSimpleClass> { new InheritsFromSimpleClass(1), new InheritsFromSimpleClass(2) };
                    var interceptedP10 = new List<SimpleClass> { new SimpleClass(1), new SimpleClass(2) };
                    var interceptedP11 = new List<SimpleStruct> { new SimpleStruct(1), new SimpleStruct(2) };
                    var interceptedP12 = new List<SimpleClass> { new SimpleClass(1), new SimpleClass(2) };

                    InheritsFromSimpleClass out1;
                    SimpleClass out2;
                    SimpleStruct out3;
                    SimpleClass out4;
                    InheritsFromSimpleClass[] out5;
                    SimpleClass[] out6;
                    SimpleStruct[] out7;
                    SimpleClass[] out8;
                    List<InheritsFromSimpleClass> out9;
                    List<SimpleClass> out10;
                    List<SimpleStruct> out11;
                    List<SimpleClass> out12;
                    instance.GenericReturnInterceptedOutParameters<SimpleStruct, SimpleClass>(out out1, out out2, out out3, out out4, out out5, out out6, out out7, out out8, out out9, out out10, out out11, out out12);

                    Assert.Equal(interceptedP1.Value, out1.Value);
                    Assert.Equal(interceptedP2.Value, out2.Value);
                    Assert.Equal(interceptedP3.Value, out3.Value);
                    Assert.Equal(interceptedP4.Value, out4.Value);
                    Assert.Equal(interceptedP5[0].Value, out5[0].Value);
                    Assert.Equal(interceptedP5[1].Value, out5[1].Value);
                    Assert.Equal(interceptedP6[0].Value, out6[0].Value);
                    Assert.Equal(interceptedP6[1].Value, out6[1].Value);
                    Assert.Equal(interceptedP7[0].Value, out7[0].Value);
                    Assert.Equal(interceptedP7[1].Value, out7[1].Value);
                    Assert.Equal(interceptedP8[0].Value, out8[0].Value);
                    Assert.Equal(interceptedP8[1].Value, out8[1].Value);
                    Assert.Equal(interceptedP9[0].Value, out9[0].Value);
                    Assert.Equal(interceptedP9[1].Value, out9[1].Value);
                    Assert.Equal(interceptedP10[0].Value, out10[0].Value);
                    Assert.Equal(interceptedP10[1].Value, out10[1].Value);
                    Assert.Equal(interceptedP11[0].Value, out11[0].Value);
                    Assert.Equal(interceptedP11[1].Value, out11[1].Value);
                    Assert.Equal(interceptedP12[0].Value, out12[0].Value);
                    Assert.Equal(interceptedP12[1].Value, out12[1].Value);
                });
            }
        }
        
        public class GenericMethodsWithRepeatedGenericParameter
        {
            
            [Fact]
            private void returns_original_value_when_GenericWithRepeatedGenericParameterReturnOriginalValueFromFirstParameter_is_called()
            {
                Execute(() =>
                {
                    var instance = new InterceptGenericMethodsWithConstraintsClass<InheritsFromSimpleClass, SimpleClass>();
                    var result = instance.GenericWithRepeatedGenericParameterReturnOriginalValueFromFirstParameter<int[], IEnumerable>(new InheritsFromSimpleClass(1), new[] { "a" }, new[] { 1 });

                    Assert.Equal(1, result.Value);
                });
            }

            [Fact]
            private void returns_intercepted_value_when_GenericWithRepeatedGenericParameterReturnInterceptedValueFromFirstParameter_is_called()
            {
                Execute(() =>
                {
                    var instance = new InterceptGenericMethodsWithConstraintsClass<InheritsFromSimpleClass, SimpleClass>();
                    var result = instance.GenericWithRepeatedGenericParameterReturnInterceptedValueFromFirstParameter<int[], IEnumerable>(new InheritsFromSimpleClass(1), new[] { "a" }, new[] { 1 });

                    Assert.Equal(2, result.Value);
                });
            }
            
            [Fact]
            private void returns_original_parameters_as_string_when_GenericWithRepeatedGenericParameterReturnOriginalParametersAsString_is_called()
            {
                Execute(() =>
                {
                    var instance = new InterceptGenericMethodsWithConstraintsClass<InheritsFromSimpleClass, SimpleClass>();
                    var p1 = new InheritsFromSimpleClass(1);
                    var p2 = new[] { "a" };
                    var p3 = new[] { 1 };
                    var expected = Helper.AsString(p1, p2, p3);

                    var result = instance.GenericWithRepeatedGenericParameterReturnOriginalParametersAsString<int[], IEnumerable>(p1, p2, p3);

                    Assert.Equal(expected, result);
                });
            }

            [Fact]
            private void returns_intercepted_parameters_as_string_when_GenericWithRepeatedGenericParameterReturnInterceptedParametersAsString_is_called()
            {
                Execute(() =>
                {
                    var instance = new InterceptGenericMethodsWithConstraintsClass<InheritsFromSimpleClass, SimpleClass>();
                    var interceptedP1 = new InheritsFromSimpleClass(2);
                    var interceptedP2 = new[] { "ab" };
                    var interceptedP3 = new[] { 2 };
                    var expected = Helper.AsString(interceptedP1, interceptedP2, interceptedP3);

                    var originalP1 = new InheritsFromSimpleClass(1);
                    var originalP2 = new[] { "a" };
                    var originalP3 = new[] { 1 };
                    var result = instance.GenericWithRepeatedGenericParameterReturnInterceptedParametersAsString<int[], IEnumerable>(originalP1, originalP2, originalP3);

                    Assert.Equal(expected, result);
                });
            }

            [Fact]
            private void returns_original_parameters_as_string_when_GenericWithRepeatedGenericParameterReturnOriginalRefParametersAsString_is_called()
            {
                Execute(() =>
                {
                    var instance = new InterceptGenericMethodsWithConstraintsClass<InheritsFromSimpleClass, SimpleClass>();
                    var p1 = new InheritsFromSimpleClass(1);
                    var p2 = new[] { "a" } as IEnumerable;
                    var p3 = new[] { 1 };
                    var expected = Helper.AsString(p1, p2, p3);

                    var ref1 = p1;
                    var ref2 = p2;
                    var ref3 = p3;
                    var result = instance.GenericWithRepeatedGenericParameterReturnOriginalRefParametersAsString<int[], IEnumerable>(ref ref1, ref ref2, ref ref3);

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
                    var instance = new InterceptGenericMethodsWithConstraintsClass<InheritsFromSimpleClass, SimpleClass>();
                    var interceptedP1 = new InheritsFromSimpleClass(2);
                    var interceptedP2 = new[] { "ab" };
                    var interceptedP3 = new[] { 2 };
                    var expected = Helper.AsString(interceptedP1, interceptedP2, interceptedP3);

                    var p1 = new InheritsFromSimpleClass(1);
                    var p2 = new[] { "a" } as IEnumerable;
                    var p3 = new[] { 1 };
                    var ref1 = p1;
                    var ref2 = p2;
                    var ref3 = p3;
                    var result = instance.GenericWithRepeatedGenericParameterReturnInterceptedRefParametersAsString<int[], IEnumerable>(ref ref1, ref ref2, ref ref3);

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
                    var instance = new InterceptGenericMethodsWithConstraintsClass<InheritsFromSimpleClass, SimpleClass>();
                    var p1 = default(InheritsFromSimpleClass);
                    var p2 = default(IEnumerable);
                    var p3 = default(int[]);
                    var p4 = new[] { default(InheritsFromSimpleClass) };
                    var p5 = new[] { default(IEnumerable) };
                    var p6 = new[] { default(int[]) };
                    var p7 = new List<InheritsFromSimpleClass> { default(InheritsFromSimpleClass) };
                    var p8 = new List<IEnumerable> { default(IEnumerable) };
                    var p9 = new List<int[]> { default(int[]) };

                    InheritsFromSimpleClass out1;
                    IEnumerable out2;
                    int[] out3;
                    InheritsFromSimpleClass[] out4;
                    IEnumerable[] out5;
                    int[][] out6;
                    List<InheritsFromSimpleClass> out7;
                    List<IEnumerable> out8;
                    List<int[]> out9;
                    instance.GenericWithRepeatedGenericParameterReturnOriginalOutParameters<int[], IEnumerable>(out out1, out out2, out out3, out out4, out out5, out out6, out out7, out out8, out out9);

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
                    var instance = new InterceptGenericMethodsWithConstraintsClass<InheritsFromSimpleClass, SimpleClass>();
                    var interceptedP1 = new InheritsFromSimpleClass(2);
                    var interceptedP2 = new[] { "ab" } as IEnumerable;
                    var interceptedP3 = new[] { 2 };
                    var interceptedP4 = new[] { new InheritsFromSimpleClass(1), new InheritsFromSimpleClass(2) };
                    var interceptedP5 = new[] { new[] { "ab" } };
                    var interceptedP6 = new[] { new[] { 2 } };
                    var interceptedP7 = new List<InheritsFromSimpleClass> { new InheritsFromSimpleClass(1), new InheritsFromSimpleClass(2) };
                    var interceptedP8 = new List<IEnumerable> { new[] { "ab" } };
                    var interceptedP9 = new List<int[]> { new[] { 2 } };

                    InheritsFromSimpleClass out1;
                    IEnumerable out2;
                    int[] out3;
                    InheritsFromSimpleClass[] out4;
                    IEnumerable[] out5;
                    int[][] out6;
                    List<InheritsFromSimpleClass> out7;
                    List<IEnumerable> out8;
                    List<int[]> out9;
                    instance.GenericWithRepeatedGenericParameterReturnInterceptedOutParameters<int[], IEnumerable>(out out1, out out2, out out3, out out4, out out5, out out6, out out7, out out8, out out9);

                    Assert.Equal(interceptedP1.Value, out1.Value);
                    Assert.Equal(interceptedP2.Cast<string>().First(), out2.Cast<string>().First());
                    Assert.Equal(interceptedP3[0], out3[0]);
                    Assert.Equal(interceptedP4[0].Value, out4[0].Value);
                    Assert.Equal(interceptedP4[1].Value, out4[1].Value);
                    Assert.Equal(interceptedP5[0].Cast<string>().First(), out5[0].Cast<string>().First());
                    Assert.Equal(interceptedP6[0][0], out6[0][0]);
                    Assert.Equal(interceptedP7[0].Value, out7[0].Value);
                    Assert.Equal(interceptedP7[1].Value, out7[1].Value);
                    Assert.Equal(interceptedP8[0].Cast<string>().First(), out8[0].Cast<string>().First());
                    Assert.Equal(interceptedP9[0][0], out9[0][0]);
                });
            }
        }
    }
}
