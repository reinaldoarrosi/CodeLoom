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
using TestAssembly.Aspects.InterceptAsyncMethod;
using System.Threading;

namespace CodeLoom.Tests
{
    public class InterceptAsyncMethodsTest : BaseTest
    {
        public class SimpleValues
        {
            [Fact]
            private void returns_original_value_when_ReturnOriginalValue_is_called()
            {
                Execute(() =>
                {
                    var instance = new InterceptAsyncMethodsClass();
                    var result = instance.ReturnOriginalValue().Result;

                    Assert.Equal(1, result);
                });
            }

            [Fact]
            private void returns_intercepted_value_when_ReturnInterceptedValue_is_called()
            {
                Execute(() =>
                {
                    var instance = new InterceptAsyncMethodsClass();
                    var result = instance.ReturnInterceptedValue().Result;

                    Assert.Equal(2, result);
                });
            }
        }

        public class Arrays
        {
            [Fact]
            private void returns_original_value_when_ReturnOriginalValueArray_is_called()
            {
                Execute(() =>
                {
                    var instance = new InterceptAsyncMethodsClass();
                    var result = instance.ReturnOriginalValueArray().Result;

                    Assert.Single(result);
                    Assert.Equal(1, result[0]);
                });
            }

            [Fact]
            private void returns_intercepted_value_when_ReturnInterceptedValueArray_is_called()
            {
                Execute(() =>
                {
                    var instance = new InterceptAsyncMethodsClass();
                    var result = instance.ReturnInterceptedValueArray().Result;

                    Assert.Equal(2, result.Length);
                    Assert.Equal(2, result[0]);
                    Assert.Equal(1, result[1]);
                });
            }
        }

        public class Lists
        {
            [Fact]
            private void returns_original_value_when_ReturnOriginalValueList_is_called()
            {
                Execute(() =>
                {
                    var instance = new InterceptAsyncMethodsClass();
                    var result = instance.ReturnOriginalValueList().Result;

                    Assert.Single(result);
                    Assert.Equal(1, result[0]);
                });
            }

            [Fact]
            private void returns_intercepted_value_when_ReturnInterceptedValueList_is_called()
            {
                Execute(() =>
                {
                    var instance = new InterceptAsyncMethodsClass();
                    var result = instance.ReturnInterceptedValueList().Result;

                    Assert.Equal(2, result.Count);
                    Assert.Equal(2, result[0]);
                    Assert.Equal(1, result[1]);
                });
            }
        }

        public class Generics
        {
            [Fact]
            private void returns_original_value_when_ReturnOriginalGenericValue_is_called()
            {
                Execute(() =>
                {
                    var instance = new InterceptAsyncMethodsClass();
                    var result = instance.ReturnOriginalGenericValue<string>("a").Result;

                    Assert.Equal("a", result);
                });
            }

            [Fact]
            private void returns_intercepted_value_when_ReturnInterceptedGenericValue_is_called()
            {
                Execute(() =>
                {
                    var instance = new InterceptAsyncMethodsClass();
                    var result = instance.ReturnInterceptedGenericValue<string>("a").Result;

                    Assert.Equal("ab", result);
                });
            }

            [Fact]
            private void returns_original_value_when_ReturnOriginalGenericValueArray_is_called()
            {
                Execute(() =>
                {
                    var instance = new InterceptAsyncMethodsClass();
                    var result = instance.ReturnOriginalGenericValueArray<string>(new[] { "a" }).Result;

                    Assert.Equal("a", result[0]);
                });
            }

            [Fact]
            private void returns_intercepted_value_when_ReturnInterceptedGenericValueArray_is_called()
            {
                Execute(() =>
                {
                    var instance = new InterceptAsyncMethodsClass();
                    var result = instance.ReturnInterceptedGenericValueArray<string>(new[] { "a" }).Result;

                    Assert.Equal("a", result[0]);
                    Assert.Equal("b", result[1]);
                });
            }

            [Fact]
            private void returns_original_value_when_ReturnOriginalGenericValueList_is_called()
            {
                Execute(() =>
                {
                    var instance = new InterceptAsyncMethodsClass();
                    var result = instance.ReturnOriginalGenericValueList<string>(new List<string> { "a" }).Result;

                    Assert.Equal("a", result[0]);
                });
            }

            [Fact]
            private void returns_intercepted_value_when_ReturnInterceptedGenericValueList_is_called()
            {
                Execute(() =>
                {
                    var instance = new InterceptAsyncMethodsClass();
                    var result = instance.ReturnInterceptedGenericValueList<string>(new List<string> { "a" }).Result;

                    Assert.Equal("a", result[0]);
                    Assert.Equal("b", result[1]);
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
                    var instance = new InterceptAsyncMethodsClass();
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

                    var result = instance.ReturnOriginalParametersAsString(p1, p2, p3, p4, p5, p6, p7, p8, p9, p10, p11, p12).Result;

                    Assert.Equal(expected, result);
                });
            }

            [Fact]
            private void return_string_with_intercepted_parameters_values_when_ReturnInterceptedParametersAsString_is_called()
            {
                Execute(() =>
                {
                    var instance = new InterceptAsyncMethodsClass();
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
                    var result = instance.ReturnInterceptedParametersAsString(originalP1, originalP2, originalP3, originalP4, originalP5, originalP6, originalP7, originalP8, originalP9, originalP10, originalP11, originalP12).Result;

                    Assert.Equal(expected, result);
                });
            }
        }

        public class OtherMethods
        {
            [Fact]
            private void should_intercept_async_method_that_returns_task()
            {
                Execute(() =>
                {
                    InterceptAsyncMethodThatReturnsTaskAspect.ResetCounters();
                    var instance = new InterceptAsyncMethodsClass();

                    var t = instance.InterceptAsyncMethodThatReturnsTask();

                    Assert.Equal(1, InterceptAsyncMethodThatReturnsTaskAspect.COUNTER_BEFORE);
                    Assert.Equal(0, InterceptAsyncMethodThatReturnsTaskAspect.COUNTER_AFTER);
                    t.Wait();
                    Assert.Equal(1, InterceptAsyncMethodThatReturnsTaskAspect.COUNTER_BEFORE);
                    Assert.Equal(1, InterceptAsyncMethodThatReturnsTaskAspect.COUNTER_AFTER);
                });
            }

            [Fact]
            private void should_intercept_async_method_that_returns_void()
            {
                Execute(() =>
                {
                    InterceptAsyncMethodThatReturnsVoidAspect.ResetCounters();
                    var instance = new InterceptAsyncMethodsClass();

                    instance.InterceptAsyncMethodThatReturnsVoid();

                    Assert.Equal(1, InterceptAsyncMethodThatReturnsVoidAspect.COUNTER_BEFORE);
                    Assert.Equal(1, InterceptAsyncMethodThatReturnsVoidAspect.COUNTER_AFTER);
                });
            }
        }
    }
}
