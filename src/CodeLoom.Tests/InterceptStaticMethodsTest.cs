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
using System;

namespace CodeLoom.Tests
{
    public class InterceptStaticMethodsTest : BaseTest
    {
        [Fact]
        private void returns_original_value_when_ReturnOriginalIntValue_is_called()
        {
            Execute(() =>
            {
                var result = InterceptStaticMethodsClass<SimpleClass>.ReturnOriginalIntValue(1);

                Assert.Equal(1, result);
            });
        }

        [Fact]
        private void returns_original_value_when_ReturnOriginalSimpleClassValue_is_called()
        {
            Execute(() =>
            {
                var result = InterceptStaticMethodsClass<SimpleClass>.ReturnOriginalSimpleClassValue(new SimpleClass(1));

                Assert.Equal(1, result.Value);
            });
        }

        [Fact]
        private void returns_original_value_when_ReturnOriginalTValue_is_called()
        {
            Execute(() =>
            {
                var result = InterceptStaticMethodsClass<SimpleClass>.ReturnOriginalTValue(new SimpleClass(1));

                Assert.Equal(1, result.Value);
            });
        }

        [Fact]
        private void returns_original_value_when_ReturnOriginalT2Value_is_called()
        {
            Execute(() =>
            {
                var result = InterceptStaticMethodsClass<SimpleClass>.ReturnOriginalT2Value<string>(new SimpleClass(1), "a");

                Assert.Equal("a", result);
            });
        }

        [Fact]
        private void returns_original_value_when_ReturnOriginalTYieldEnumerable_is_called()
        {
            Execute(() =>
            {
                var result = InterceptStaticMethodsClass<SimpleClass>
                    .ReturnOriginalTYieldEnumerable<InheritsFromSimpleClass>(new SimpleClass(1), new InheritsFromSimpleClass(1))
                    .ToArray();

                Assert.Equal(2, result.Length);
                Assert.Equal(1, result[0].Value);
                Assert.Equal(1, result[1].Value);
            });
        }

        [Fact]
        private void returns_original_value_when_ReturnOriginalT2Async_is_called()
        {
            Execute(() =>
            {
                var result = InterceptStaticMethodsClass<SimpleClass>.ReturnOriginalT2Async<string>(new SimpleClass(1), "a").Result;

                Assert.Equal("a", result);
            });
        }

        [Fact]
        private void returns_intercepted_value_when_ReturnInterceptedIntValue_is_called()
        {
            Execute(() =>
            {
                var result = InterceptStaticMethodsClass<SimpleClass>.ReturnInterceptedIntValue(1);

                Assert.Equal(2, result);
            });
        }

        [Fact]
        private void returns_intercepted_value_when_ReturnInterceptedSimpleClassValue_is_called()
        {
            Execute(() =>
            {
                var result = InterceptStaticMethodsClass<SimpleClass>.ReturnInterceptedSimpleClassValue(new SimpleClass(1));

                Assert.Equal(2, result.Value);
            });
        }

        [Fact]
        private void returns_intercepted_value_when_ReturnInterceptedTValue_is_called()
        {
            Execute(() =>
            {
                var result = InterceptStaticMethodsClass<SimpleClass>.ReturnInterceptedTValue(new SimpleClass(1));

                Assert.Equal(2, result.Value);
            });
        }

        [Fact]
        private void returns_intercepted_value_when_ReturnInterceptedT2Value_is_called()
        {
            Execute(() =>
            {
                var result = InterceptStaticMethodsClass<SimpleClass>.ReturnInterceptedT2Value<string>(new SimpleClass(1), "a");

                Assert.Equal("b", result);
            });
        }

        [Fact]
        private void returns_intercepted_value_when_ReturnInterceptedTYieldEnumerable_is_called()
        {
            Execute(() =>
            {
                var result = InterceptStaticMethodsClass<SimpleClass>
                    .ReturnInterceptedTYieldEnumerable<InheritsFromSimpleClass>(new SimpleClass(1), new InheritsFromSimpleClass(1))
                    .ToArray();

                Assert.Equal(2, result.Length);
                Assert.Equal(2, result[0].Value);
                Assert.Equal(2, result[1].Value);
            });
        }

        [Fact]
        private void returns_intercepted_value_when_ReturnInterceptedT2Async_is_called()
        {
            Execute(() =>
            {
                var result = InterceptStaticMethodsClass<SimpleClass>.ReturnInterceptedT2Async<string>(new SimpleClass(1), "a").Result;

                Assert.Equal("b", result);
            });
        }
    }
}
