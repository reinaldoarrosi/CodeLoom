using CodeLoom.Fody;
using Xunit;
using Fody;
using System.Reflection;
using System.Collections.Generic;
using System.IO;

namespace CodeLoom.Fody.Tests
{
    public class SimpleTests : BaseTest
    {
        public class Methods : SimpleTests
        {
            [Fact]
            private void should_return_original_result_when_no_aspect_is_registered()
            {
                var instance = WeaveResult.GetInstance("TestAssembly.TestClass");
                var remainder = 0;
                var result = instance.DivideInt(5, 2, ref remainder);

                Assert.Equal(2, result);
                Assert.Equal(1, remainder);
            }

            [Fact]
            private void should_apply_same_sideeffects_when_no_aspect_is_registered()
            {
                var instance = WeaveResult.GetInstance("TestAssembly.TestClass");

                instance.StoreString("TestString");

                Assert.Equal("TestString", instance.StoredString);
            }
        }

        public class Properties : SimpleTests
        {
            [Fact]
            private void should_set_and_get_original_value_from_regular_property_when_no_aspect_is_registered()
            {
                var instance = WeaveResult.GetInstance("TestAssembly.TestClass");
                instance.StoredString = "TestString";

                var result = instance.StoredString;

                Assert.Equal("TestString", result);
            }

            [Fact]
            private void should_set_and_get_original_value_from_indexer_property_when_no_aspect_is_registered()
            {
                var instance = WeaveResult.GetInstance("TestAssembly.TestClass");
                instance[1] = "TestString";

                var result = instance[1];

                Assert.Equal("TestString", result);
            }
        }
    }
}
