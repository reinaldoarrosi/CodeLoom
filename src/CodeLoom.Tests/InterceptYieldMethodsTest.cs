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
    public class InterceptYieldMethodsTest : BaseTest
    {
        [Fact]
        private void returns_original_value_when_ReturnOriginalIntEnumerable_is_called()
        {
            Execute(() =>
            {
                var instance = new InterceptYieldMethodsClass();
                var result = instance.ReturnOriginalIntEnumerable().ToArray();

                Assert.Single(result);
                Assert.Equal(1, result[0]);
            });
        }

        [Fact]
        private void returns_intercepted_value_when_ReturnInterceptedIntEnumerable_is_called()
        {
            Execute(() =>
            {
                var instance = new InterceptYieldMethodsClass();
                var result = instance.ReturnInterceptedIntEnumerable().ToArray();

                Assert.Equal(2, result.Length);
                Assert.Equal(1, result[0]);
                Assert.Equal(2, result[1]);
            });
        }

        [Fact]
        private void returns_original_value_when_ReturnOriginalSimpleClassEnumerable_is_called()
        {
            Execute(() =>
            {
                var instance = new InterceptYieldMethodsClass();
                var result = instance.ReturnOriginalSimpleClassEnumerable().ToArray();

                Assert.Single(result);
                Assert.Equal(1, result[0].Value);
            });
        }

        [Fact]
        private void returns_intercepted_value_when_ReturnInterceptedSimpleClassEnumerable_is_called()
        {
            Execute(() =>
            {
                var instance = new InterceptYieldMethodsClass();
                var result = instance.ReturnInterceptedSimpleClassEnumerable().ToArray();

                Assert.Equal(2, result.Length);
                Assert.Equal(1, result[0].Value);
                Assert.Equal(2, result[1].Value);
            });
        }

        [Fact]
        private void returns_original_value_when_ReturnOriginalTEnumerable_is_called()
        {
            Execute(() =>
            {
                var instance = new InterceptYieldMethodsClass();
                var result = instance.ReturnOriginalTEnumerable<string>().ToArray();

                Assert.Single(result);
                Assert.Equal(default(string), result[0]);
            });
        }

        [Fact]
        private void returns_intercepted_value_when_ReturnInterceptedTEnumerable_is_called()
        {
            Execute(() =>
            {
                var instance = new InterceptYieldMethodsClass();
                var result = instance.ReturnInterceptedTEnumerable<string>().ToArray();

                Assert.Equal(2, result.Length);
                Assert.Equal(default(string), result[0]);
                Assert.Equal("b", result[1]);
            });
        }

        [Fact]
        private void returns_empty_enumerable_when_ReturnEmptyEnumerable_is_called()
        {
            Execute(() =>
            {
                var instance = new InterceptYieldMethodsClass();
                var result = instance.ReturnEmptyEnumerable<DateTime>().ToArray();

                Assert.Empty(result);
            });
        }

        [Fact]
        private void returns_non_empty_enumerable_when_ReplaceEmptyEnumerableWithANonEmptyEnumerable_is_called()
        {
            Execute(() =>
            {
                var instance = new InterceptYieldMethodsClass();
                var result = instance.ReplaceEmptyEnumerableWithANonEmptyEnumerable<DateTime>().ToArray();

                Assert.NotEmpty(result);
            });
        }
    }
}
