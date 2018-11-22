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
    public class InterceptConstructorTest : BaseTest
    {
        [Fact]
        private void intercepts_constructor_when_creating_an_instance_of_InterceptConstructorClass_with_int_parameter()
        {
            Execute(() =>
            {
                var instance = new InterceptConstructorClass<DateTime>(1);

                Assert.Equal(2, instance.IntValue);
            });
        }

        [Fact]
        private void intercepts_constructor_when_creating_an_instance_of_InterceptConstructorClass_with_SimpleClass_parameter()
        {
            Execute(() =>
            {
                var instance = new InterceptConstructorClass<DateTime>(new SimpleClass(1));

                Assert.Equal(2, instance.SimpleClassValue.Value);
            });
        }

        [Fact]
        private void intercepts_constructor_when_creating_an_instance_of_InterceptConstructorClass_with_multiple_parameters()
        {
            Execute(() =>
            {
                var expectedA = 2;
                var expectedB = new SimpleClass(2);
                var expectedC = DateTime.Today.AddDays(-1);
                var expectedD = new[] { 1, 2 };
                var expectedE = new[] { new SimpleClass(1), new SimpleClass(2) };
                var expectedF = new[] { DateTime.Today.AddDays(-1), DateTime.Today.AddDays(-2) };
                var expectedG = new List<int> { 1, 2 };
                var expectedH = new List<SimpleClass> { new SimpleClass(1), new SimpleClass(2) };
                var expectedI = new List<DateTime> { DateTime.Today.AddDays(-1), DateTime.Today.AddDays(-2) };

                var originalA = 1;
                var originalB = new SimpleClass(1);
                var originalC = DateTime.Today;
                var originalD = new[] { 1 };
                var originalE = new[] { new SimpleClass(1) };
                var originalF = new[] { DateTime.Today };
                var originalG = new List<int> { 1 };
                var originalH = new List<SimpleClass> { new SimpleClass(1) };
                var originalI = new List<DateTime> { DateTime.Today };

                var instance = new InterceptConstructorClass<DateTime>(
                    originalA, 
                    originalB, 
                    originalC, 
                    originalD,
                    originalE, 
                    originalF, 
                    originalG, 
                    originalH, 
                    originalI
                );

                Assert.Equal(expectedA, instance.IntValue);
                Assert.Equal(expectedB.Value, instance.SimpleClassValue.Value);
                Assert.Equal(expectedC, instance.TValue);
                Assert.Equal(expectedD[0], instance.IntArray[0]);
                Assert.Equal(expectedD[1], instance.IntArray[1]);
                Assert.Equal(expectedE[0].Value, instance.SimpleClassArray[0].Value);
                Assert.Equal(expectedE[1].Value, instance.SimpleClassArray[1].Value);
                Assert.Equal(expectedF[0], instance.TArray[0]);
                Assert.Equal(expectedF[1], instance.TArray[1]);
                Assert.Equal(expectedG[0], instance.IntList[0]);
                Assert.Equal(expectedG[1], instance.IntList[1]);
                Assert.Equal(expectedH[0].Value, instance.SimpleClassList[0].Value);
                Assert.Equal(expectedH[1].Value, instance.SimpleClassList[1].Value);
                Assert.Equal(expectedI[0], instance.TList[0]);
                Assert.Equal(expectedI[1], instance.TList[1]);
            });
        }

        [Fact]
        private void intercepts_constructor_when_creating_an_instance_of_InterceptConstructorClass_InnerClass()
        {
            Execute(() =>
            {
                var expectedA = DateTime.Today.AddDays(-1);
                var expectedB = "b";
                var expectedC = new[] { DateTime.Today.AddDays(-1), DateTime.Today.AddDays(-2) };
                var expectedD = new[] { "a", "b" };
                var expectedE = new List<DateTime> { DateTime.Today.AddDays(-1), DateTime.Today.AddDays(-2) };
                var expectedF = new List<string> { "a", "b" };

                var originalA = DateTime.Today;
                var originalB = "a";
                var originalC = new[] { DateTime.Today };
                var originalD = new[] { "a" };
                var originalE = new List<DateTime> { DateTime.Today };
                var originalF = new List<string> { "a" };

                var instance = new InterceptConstructorClass<DateTime>.InnerClass<string>(
                    originalA, 
                    originalB, 
                    originalC, 
                    originalD, 
                    originalE, 
                    originalF
                );

                Assert.Equal(expectedA, instance.TValue);
                Assert.Equal(expectedB, instance.T2Value);
                Assert.Equal(expectedC[0], instance.TArray[0]);
                Assert.Equal(expectedC[1], instance.TArray[1]);
                Assert.Equal(expectedD[0], instance.T2Array[0]);
                Assert.Equal(expectedD[1], instance.T2Array[1]);
                Assert.Equal(expectedE[0], instance.TList[0]);
                Assert.Equal(expectedE[1], instance.TList[1]);
                Assert.Equal(expectedF[0], instance.T2List[0]);
                Assert.Equal(expectedF[1], instance.T2List[1]);
            });
        }
    }
}
