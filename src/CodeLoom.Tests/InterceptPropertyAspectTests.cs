using CodeLoom.Fody;
using Xunit;
using Fody;
using System.Reflection;
using System.Collections.Generic;
using System.IO;
using System.Collections;
using TestAssembly.Aspects;

namespace CodeLoom.Fody.Tests
{
    public class InterceptPropertyAspectTests : BaseTest
    {
        [Fact]
        private void should_intercept_properties_from_class()
        {
            var type = typeof(TestAssembly.ClassesToWeave.ClassWithInterceptedProperties);
            var instance = WeaveResult.GetInstance(type.FullName);
        }
    }
}
