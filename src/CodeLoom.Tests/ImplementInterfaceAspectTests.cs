using CodeLoom.Fody;
using Xunit;
using Fody;
using System.Reflection;
using System.Collections.Generic;
using System.IO;
using System.Collections;

namespace CodeLoom.Fody.Tests
{
    public class ImplementInterfaceAspectTests : BaseTest
    {
        [Fact]
        private void should_add_properties_to_class()
        {
            var type = typeof(TestAssembly.ClassesToWeave.ClassWithWeavedProperties);
            var instance = WeaveResult.GetInstance(type.FullName);
            var interface1 = instance as TestAssembly.Aspects.IPropertiesAdded;
            var interface2 = instance as TestAssembly.Aspects.IGenericPropertiesAdded1<TestAssemblyReference.SimpleClass>;
            var interface3 = instance as TestAssembly.Aspects.IGenericPropertiesAdded2<int, string>;

            Assert.NotNull(interface1);
            Assert.Equal(10, interface1.IntProperty);
            Assert.Equal(new[] { "a", "b" }, interface1.StringArrayProperty);
            Assert.Equal(new ArrayList() { 1, 2, 3 }, interface1.ReferenceToNETFrameworkClassProperty);
            Assert.NotNull(interface1.ReferenceToClassInAnotherAssemblyProperty);
            Assert.Equal(2, interface1.GenericListOfClassInAnotherAssemblyProperty.Count);
            Assert.Equal(11, interface1.ReadOnlyProperty);
            Assert.NotNull(interface1[0, null]);
            Assert.NotNull(interface1[null, null]);

            Assert.NotNull(interface2);
            Assert.NotNull(interface2.GenericProperty1);
            Assert.NotNull(interface2[null]);

            Assert.NotNull(interface3);
            Assert.Equal(13, interface3.GenericProperty2_A);
            Assert.Equal("c", interface3.GenericProperty2_B);
            Assert.Equal(14, interface3.GenericProperty1);
        }

        [Fact]
        private void should_add_properties_to_generic_class()
        {
            var type1 = typeof(TestAssembly.ClassesToWeave.GenericClassWithWeavedProperties<int, string>);
            var type2 = typeof(TestAssembly.ClassesToWeave.GenericClassWithWeavedProperties<List<int>, ArrayList>);
            var instance1 = WeaveResult.GetInstance(type1.FullName);
            var instance2 = WeaveResult.GetInstance(type2.FullName);
            var interface1_1 = instance1 as TestAssembly.Aspects.IPropertiesAdded;
            var interface1_2 = instance1 as TestAssembly.Aspects.IGenericPropertiesAdded1<TestAssemblyReference.SimpleClass>;
            var interface1_3 = instance1 as TestAssembly.Aspects.IGenericPropertiesAdded2<int, string>;
            var interface2_1 = instance2 as TestAssembly.Aspects.IPropertiesAdded;
            var interface2_2 = instance2 as TestAssembly.Aspects.IGenericPropertiesAdded1<TestAssemblyReference.SimpleClass>;
            var interface2_3 = instance2 as TestAssembly.Aspects.IGenericPropertiesAdded2<int, string>;

            Assert.NotNull(interface1_1);
            Assert.Equal(10, interface1_1.IntProperty);
            Assert.Equal(new[] { "a", "b" }, interface1_1.StringArrayProperty);
            Assert.Equal(new ArrayList() { 1, 2, 3 }, interface1_1.ReferenceToNETFrameworkClassProperty);
            Assert.NotNull(interface1_1.ReferenceToClassInAnotherAssemblyProperty);
            Assert.Equal(2, interface1_1.GenericListOfClassInAnotherAssemblyProperty.Count);
            Assert.Equal(11, interface1_1.ReadOnlyProperty);
            Assert.NotNull(interface1_1[0, null]);
            Assert.NotNull(interface1_1[null, null]);

            Assert.NotNull(interface1_2);
            Assert.NotNull(interface1_2.GenericProperty1);
            Assert.NotNull(interface1_2[null]);

            Assert.NotNull(interface1_3);
            Assert.Equal(13, interface1_3.GenericProperty2_A);
            Assert.Equal("c", interface1_3.GenericProperty2_B);
            Assert.Equal(14, interface1_3.GenericProperty1);

            Assert.NotNull(interface2_1);
            Assert.Equal(10, interface2_1.IntProperty);
            Assert.Equal(new[] { "a", "b" }, interface2_1.StringArrayProperty);
            Assert.Equal(new ArrayList() { 1, 2, 3 }, interface2_1.ReferenceToNETFrameworkClassProperty);
            Assert.NotNull(interface2_1.ReferenceToClassInAnotherAssemblyProperty);
            Assert.Equal(2, interface2_1.GenericListOfClassInAnotherAssemblyProperty.Count);
            Assert.Equal(11, interface2_1.ReadOnlyProperty);
            Assert.NotNull(interface2_1[0, null]);
            Assert.NotNull(interface2_1[null, null]);

            Assert.NotNull(interface2_2);
            Assert.NotNull(interface2_2.GenericProperty1);
            Assert.NotNull(interface2_2[null]);

            Assert.NotNull(interface2_3);
            Assert.Equal(13, interface2_3.GenericProperty2_A);
            Assert.Equal("c", interface2_3.GenericProperty2_B);
            Assert.Equal(14, interface2_3.GenericProperty1);
        }

        [Fact]
        private void should_add_methods_to_class()
        {
            var type = typeof(TestAssembly.ClassesToWeave.ClassWithWeavedMethods);
            var instance = WeaveResult.GetInstance(type.FullName);
        }

        [Fact]
        private void should_add_methods_to_generic_class()
        {
            var type1 = typeof(TestAssembly.ClassesToWeave.GenericClassWithWeavedMethods<int, string>);
            var instance1 = WeaveResult.GetInstance(type1.FullName);

            var type2 = typeof(TestAssembly.ClassesToWeave.GenericClassWithWeavedMethods<List<int>, ArrayList>);
            var instance2 = WeaveResult.GetInstance(type2.FullName);
        }
    }
}
