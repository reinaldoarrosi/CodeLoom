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
        [Fact]
        private void test()
        {
            var instance = WeaveResult.GetInstance("TestAssembly.TestClass");
        }
    }
}
