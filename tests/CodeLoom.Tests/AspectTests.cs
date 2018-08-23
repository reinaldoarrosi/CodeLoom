using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace CodeLoom.Fody.Tests
{
    public class AspectTests : BaseTest
    {
        public class Methods : SimpleTests
        {
            [Fact]
            private void should_allow_changing_arguments_values()
            {
                var remainder = 0;
                var instance = WeaveResult.GetInstance("TestAssembly.TestClass");
                CodeLoom.AddAspect(instance, new TestAspect(i =>
                {
                    i.Arguments["a"].Value = 31;
                    i.Arguments["b"].Value = 30;
                    i.Proceed();
                }));

                var result = instance.DivideInt(8, 3, ref remainder);

                Assert.Equal(1, result);
                Assert.Equal(1, remainder);
            }

            [Fact]
            private void should_allow_changing_return_value()
            {
                var remainder = 0;
                var instance = WeaveResult.GetInstance("TestAssembly.TestClass");
                CodeLoom.AddAspect(instance, new TestAspect(i =>
                {
                    i.Proceed();
                    i.ReturnValue = 10;
                }));

                var result = instance.DivideInt(8, 3, ref remainder);

                Assert.Equal(10, result);
                Assert.Equal(2, remainder);
            }

            [Fact]
            private void should_allow_changing_ref_or_out_arguments_values()
            {
                var remainder = 0;
                var instance = WeaveResult.GetInstance("TestAssembly.TestClass");
                CodeLoom.AddAspect(instance, new TestAspect(i =>
                {
                    i.Proceed();
                    i.Arguments["remainder"].Value = 5;
                }));

                var result = instance.DivideInt(8, 3, ref remainder);

                Assert.Equal(2, result);
                Assert.Equal(5, remainder);
            }

            [Fact]
            private void should_allow_changing_method_body()
            {
                var remainder = 0;
                var instance = WeaveResult.GetInstance("TestAssembly.TestClass");
                CodeLoom.AddAspect(instance, new TestAspect(i =>
                {
                    var a = (int)i.Arguments["a"].Value;
                    var b = (int)i.Arguments["b"].Value;
                    i.ReturnValue = a + b;
                    i.Arguments["remainder"].Value = -1;
                }));

                var result = instance.DivideInt(8, 3, ref remainder);

                Assert.Equal(11, result);
                Assert.Equal(-1, remainder);
            }
        }

        public class Properties : SimpleTests
        {
            [Fact]
            private void should_allow_changing_setter_value()
            {
                var instance = WeaveResult.GetInstance("TestAssembly.TestClass");
                CodeLoom.AddAspect(instance, new TestAspect(i =>
                {
                    if (i.Source.IsPropertySetter)
                    {
                        i.Arguments["value"].Value = "NewTestString";
                    }

                    i.Proceed();
                }));

                instance.StoredString = "TestString";

                Assert.Equal("NewTestString", instance.StoredString);
            }

            [Fact]
            private void should_allow_changing_getter_value()
            {
                var instance = WeaveResult.GetInstance("TestAssembly.TestClass");
                CodeLoom.AddAspect(instance, new TestAspect(i =>
                {
                    i.Proceed();
                    i.ReturnValue = "NewTestString";
                }));

                instance.StoredString = "TestString";

                Assert.Equal("NewTestString", instance.StoredString);
            }
        }
    }
}
