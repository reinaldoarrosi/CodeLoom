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
    public class InterceptPropertiesTest : BaseTest
    {
        public class SimpleValues
        {
            [Fact]
            private void returns_original_value_when_OriginalValueType_is_called()
            {
                Execute(() =>
                {
                    var instance = new InterceptPropertiesClass<InheritsFromSimpleClass>();
                    var result = instance.OriginalValueType;

                    Assert.Equal(1, result);
                });
            }

            [Fact]
            private void sets_original_value_when_OriginalValueType_is_called()
            {
                Execute(() =>
                {
                    var instance = new InterceptPropertiesClass<InheritsFromSimpleClass>();

                    instance.OriginalValueType = 3;
                    var result = instance.OriginalValueType;

                    Assert.Equal(3, result);
                });
            }

            [Fact]
            private void returns_intercepted_value_when_InterceptedValueType_is_called()
            {
                Execute(() =>
                {
                    var instance = new InterceptPropertiesClass<InheritsFromSimpleClass>();
                    var result = instance.InterceptedValueType;

                    Assert.Equal(2, result);
                });
            }

            [Fact]
            private void sets_intercepted_value_when_InterceptedValueType_is_called()
            {
                Execute(() =>
                {
                    var instance = new InterceptPropertiesClass<InheritsFromSimpleClass>();

                    instance.InterceptedValueType = 3;
                    var result = instance.InterceptedValueType;

                    Assert.Equal(4, result);
                });
            }

            [Fact]
            private void returns_original_value_when_OriginalRefType_is_called()
            {
                Execute(() =>
                {
                    var instance = new InterceptPropertiesClass<InheritsFromSimpleClass>();
                    var result = instance.OriginalRefType;

                    Assert.Equal("a", result);
                });
            }

            [Fact]
            private void sets_original_value_when_OriginalRefType_is_called()
            {
                Execute(() =>
                {
                    var instance = new InterceptPropertiesClass<InheritsFromSimpleClass>();

                    instance.OriginalRefType = "c";
                    var result = instance.OriginalRefType;

                    Assert.Equal("c", result);
                });
            }

            [Fact]
            private void returns_intercepted_value_when_InterceptedRefType_is_called()
            {
                Execute(() =>
                {
                    var instance = new InterceptPropertiesClass<InheritsFromSimpleClass>();
                    var result = instance.InterceptedRefType;

                    Assert.Equal("ab", result);
                });
            }

            [Fact]
            private void sets_intercepted_value_when_InterceptedRefType_is_called()
            {
                Execute(() =>
                {
                    var instance = new InterceptPropertiesClass<InheritsFromSimpleClass>();

                    instance.InterceptedRefType = "c";
                    var result = instance.InterceptedRefType;

                    Assert.Equal("ca", result);
                });
            }

            [Fact]
            private void returns_original_value_when_OriginalExternalValueType_is_called()
            {
                Execute(() =>
                {
                    var instance = new InterceptPropertiesClass<InheritsFromSimpleClass>();
                    var result = instance.OriginalExternalValueType;

                    Assert.Equal(1, result.Value);
                });
            }

            [Fact]
            private void sets_original_value_when_OriginalExternalValueType_is_called()
            {
                Execute(() =>
                {
                    var instance = new InterceptPropertiesClass<InheritsFromSimpleClass>();

                    instance.OriginalExternalValueType = new SimpleStruct(3);
                    var result = instance.OriginalExternalValueType;

                    Assert.Equal(3, result.Value);
                });
            }

            [Fact]
            private void returns_intercepted_value_when_InterceptedExternalValueType_is_called()
            {
                Execute(() =>
                {
                    var instance = new InterceptPropertiesClass<InheritsFromSimpleClass>();
                    var result = instance.InterceptedExternalValueType;

                    Assert.Equal(2, result.Value);
                });
            }

            [Fact]
            private void sets_intercepted_value_when_InterceptedExternalValueType_is_called()
            {
                Execute(() =>
                {
                    var instance = new InterceptPropertiesClass<InheritsFromSimpleClass>();

                    instance.InterceptedExternalValueType = new SimpleStruct(3);
                    var result = instance.InterceptedExternalValueType;

                    Assert.Equal(4, result.Value);
                });
            }

            [Fact]
            private void returns_original_value_when_OriginalExternalRefType_is_called()
            {
                Execute(() =>
                {
                    var instance = new InterceptPropertiesClass<InheritsFromSimpleClass>();
                    var result = instance.OriginalExternalRefType;

                    Assert.Equal(1, result.Value);
                });
            }

            [Fact]
            private void sets_original_value_when_OriginalExternalRefType_is_called()
            {
                Execute(() =>
                {
                    var instance = new InterceptPropertiesClass<InheritsFromSimpleClass>();

                    instance.OriginalExternalRefType = new SimpleClass(3);
                    var result = instance.OriginalExternalRefType;

                    Assert.Equal(3, result.Value);
                });
            }

            [Fact]
            private void returns_intercepted_value_when_InterceptedExternalRefType_is_called()
            {
                Execute(() =>
                {
                    var instance = new InterceptPropertiesClass<InheritsFromSimpleClass>();
                    var result = instance.InterceptedExternalRefType;

                    Assert.Equal(2, result.Value);
                });
            }

            [Fact]
            private void sets_intercepted_value_when_InterceptedExternalRefType_is_called()
            {
                Execute(() =>
                {
                    var instance = new InterceptPropertiesClass<InheritsFromSimpleClass>();

                    instance.InterceptedExternalRefType = new SimpleClass(3);
                    var result = instance.InterceptedExternalRefType;

                    Assert.Equal(4, result.Value);
                });
            }

            [Fact]
            private void returns_original_value_when_OriginalGenericType_is_called()
            {
                Execute(() =>
                {
                    var instance = new InterceptPropertiesClass<InheritsFromSimpleClass>();
                    var result = instance.OriginalGenericType;

                    Assert.Equal(1, result.Value);
                });
            }

            [Fact]
            private void sets_original_value_when_OriginalGenericType_is_called()
            {
                Execute(() =>
                {
                    var instance = new InterceptPropertiesClass<InheritsFromSimpleClass>();

                    instance.OriginalGenericType = new InheritsFromSimpleClass(3);
                    var result = instance.OriginalGenericType;

                    Assert.Equal(3, result.Value);
                });
            }

            [Fact]
            private void returns_intercepted_value_when_InterceptedGenericType_is_called()
            {
                Execute(() =>
                {
                    var instance = new InterceptPropertiesClass<InheritsFromSimpleClass>();
                    var result = instance.InterceptedGenericType;

                    Assert.Equal(2, result.Value);
                });
            }

            [Fact]
            private void sets_intercepted_value_when_InterceptedGenericType_is_called()
            {
                Execute(() =>
                {
                    var instance = new InterceptPropertiesClass<InheritsFromSimpleClass>();

                    instance.InterceptedGenericType = new InheritsFromSimpleClass(3);
                    var result = instance.InterceptedGenericType;

                    Assert.Equal(4, result.Value);
                });
            }
        }

        public class Arrays
        {
            [Fact]
            private void returns_original_value_when_OriginalValueTypeArray_is_called()
            {
                Execute(() =>
                {
                    var instance = new InterceptPropertiesClass<InheritsFromSimpleClass>();
                    var result = instance.OriginalValueTypeArray;

                    Assert.Single(result);
                    Assert.Equal(1, result[0]);
                });
            }

            [Fact]
            private void sets_original_value_when_OriginalValueTypeArray_is_called()
            {
                Execute(() =>
                {
                    var instance = new InterceptPropertiesClass<InheritsFromSimpleClass>();

                    instance.OriginalValueTypeArray = new[] { 3 };
                    var result = instance.OriginalValueTypeArray;

                    Assert.Single(result);
                    Assert.Equal(3, result[0]);
                });
            }

            [Fact]
            private void returns_intercepted_value_when_InterceptedValueTypeArray_is_called()
            {
                Execute(() =>
                {
                    var instance = new InterceptPropertiesClass<InheritsFromSimpleClass>();
                    var result = instance.InterceptedValueTypeArray;

                    Assert.Equal(2, result.Length);
                    Assert.Equal(2, result[0]);
                    Assert.Equal(1, result[1]);
                });
            }

            [Fact]
            private void sets_intercepted_value_when_InterceptedValueTypeArray_is_called()
            {
                Execute(() =>
                {
                    var instance = new InterceptPropertiesClass<InheritsFromSimpleClass>();

                    instance.InterceptedValueTypeArray = new[] { 3 };
                    var result = instance.InterceptedValueTypeArray;

                    Assert.Equal(2, result.Length);
                    Assert.Equal(4, result[0]);
                    Assert.Equal(3, result[1]);
                });
            }

            [Fact]
            private void returns_original_value_when_OriginalRefTypeArray_is_called()
            {
                Execute(() =>
                {
                    var instance = new InterceptPropertiesClass<InheritsFromSimpleClass>();
                    var result = instance.OriginalRefTypeArray;

                    Assert.Single(result);
                    Assert.Equal("a", result[0]);
                });
            }

            [Fact]
            private void sets_original_value_when_OriginalRefTypeArray_is_called()
            {
                Execute(() =>
                {
                    var instance = new InterceptPropertiesClass<InheritsFromSimpleClass>();

                    instance.OriginalRefTypeArray = new[] { "c" };
                    var result = instance.OriginalRefTypeArray;

                    Assert.Single(result);
                    Assert.Equal("c", result[0]);
                });
            }

            [Fact]
            private void returns_intercepted_value_when_InterceptedRefTypeArray_is_called()
            {
                Execute(() =>
                {
                    var instance = new InterceptPropertiesClass<InheritsFromSimpleClass>();
                    var result = instance.InterceptedRefTypeArray;

                    Assert.Equal(2, result.Length);
                    Assert.Equal("cd", result[0]);
                    Assert.Equal("ab", result[1]);
                });
            }

            [Fact]
            private void sets_intercepted_value_when_InterceptedRefTypeArray_is_called()
            {
                Execute(() =>
                {
                    var instance = new InterceptPropertiesClass<InheritsFromSimpleClass>();

                    instance.InterceptedRefTypeArray = new[] { "c" };
                    var result = instance.InterceptedRefTypeArray;

                    Assert.Equal(2, result.Length);
                    Assert.Equal("ca", result[0]);
                    Assert.Equal("cb", result[1]);
                });
            }

            [Fact]
            private void returns_original_value_when_OriginalExternalValueTypeArray_is_called()
            {
                Execute(() =>
                {
                    var instance = new InterceptPropertiesClass<InheritsFromSimpleClass>();
                    var result = instance.OriginalExternalValueTypeArray;

                    Assert.Single(result);
                    Assert.Equal(1, result[0].Value);
                });
            }

            [Fact]
            private void sets_original_value_when_OriginalExternalValueTypeArray_is_called()
            {
                Execute(() =>
                {
                    var instance = new InterceptPropertiesClass<InheritsFromSimpleClass>();

                    instance.OriginalExternalValueTypeArray = new[] { new SimpleStruct(3) };
                    var result = instance.OriginalExternalValueTypeArray;

                    Assert.Single(result);
                    Assert.Equal(3, result[0].Value);
                });
            }

            [Fact]
            private void returns_intercepted_value_when_InterceptedExternalValueTypeArray_is_called()
            {
                Execute(() =>
                {
                    var instance = new InterceptPropertiesClass<InheritsFromSimpleClass>();
                    var result = instance.InterceptedExternalValueTypeArray;

                    Assert.Equal(2, result.Length);
                    Assert.Equal(2, result[0].Value);
                    Assert.Equal(1, result[1].Value);
                });
            }

            [Fact]
            private void sets_intercepted_value_when_InterceptedExternalValueTypeArray_is_called()
            {
                Execute(() =>
                {
                    var instance = new InterceptPropertiesClass<InheritsFromSimpleClass>();

                    instance.InterceptedExternalValueTypeArray = new[] { new SimpleStruct(3) };
                    var result = instance.InterceptedExternalValueTypeArray;

                    Assert.Equal(2, result.Length);
                    Assert.Equal(4, result[0].Value);
                    Assert.Equal(3, result[1].Value);
                });
            }

            [Fact]
            private void returns_original_value_when_OriginalExternalRefTypeArray_is_called()
            {
                Execute(() =>
                {
                    var instance = new InterceptPropertiesClass<InheritsFromSimpleClass>();
                    var result = instance.OriginalExternalRefTypeArray;

                    Assert.Single(result);
                    Assert.Equal(1, result[0].Value);
                });
            }

            [Fact]
            private void sets_original_value_when_OriginalExternalRefTypeArray_is_called()
            {
                Execute(() =>
                {
                    var instance = new InterceptPropertiesClass<InheritsFromSimpleClass>();

                    instance.OriginalExternalRefTypeArray = new[] { new SimpleClass(3) };
                    var result = instance.OriginalExternalRefTypeArray;

                    Assert.Single(result);
                    Assert.Equal(3, result[0].Value);
                });
            }

            [Fact]
            private void returns_intercepted_value_when_InterceptedExternalRefTypeArray_is_called()
            {
                Execute(() =>
                {
                    var instance = new InterceptPropertiesClass<InheritsFromSimpleClass>();
                    var result = instance.InterceptedExternalRefTypeArray;

                    Assert.Equal(2, result.Length);
                    Assert.Equal(2, result[0].Value);
                    Assert.Equal(1, result[1].Value);
                });
            }

            [Fact]
            private void sets_intercepted_value_when_InterceptedExternalRefTypeArray_is_called()
            {
                Execute(() =>
                {
                    var instance = new InterceptPropertiesClass<InheritsFromSimpleClass>();

                    instance.InterceptedExternalRefTypeArray = new[] { new SimpleClass(3) };
                    var result = instance.InterceptedExternalRefTypeArray;

                    Assert.Equal(2, result.Length);
                    Assert.Equal(4, result[0].Value);
                    Assert.Equal(3, result[1].Value);
                });
            }

            [Fact]
            private void returns_original_value_when_OriginalGenericTypeArray_is_called()
            {
                Execute(() =>
                {
                    var instance = new InterceptPropertiesClass<InheritsFromSimpleClass>();
                    var result = instance.OriginalGenericTypeArray;

                    Assert.Single(result);
                    Assert.Equal(1, result[0].Value);
                });
            }

            [Fact]
            private void sets_original_value_when_OriginalGenericTypeArray_is_called()
            {
                Execute(() =>
                {
                    var instance = new InterceptPropertiesClass<InheritsFromSimpleClass>();

                    instance.OriginalGenericTypeArray = new[] { new InheritsFromSimpleClass(3) };
                    var result = instance.OriginalGenericTypeArray;

                    Assert.Single(result);
                    Assert.Equal(3, result[0].Value);
                });
            }

            [Fact]
            private void returns_intercepted_value_when_InterceptedGenericTypeArray_is_called()
            {
                Execute(() =>
                {
                    var instance = new InterceptPropertiesClass<InheritsFromSimpleClass>();
                    var result = instance.InterceptedGenericTypeArray;

                    Assert.Equal(2, result.Length);
                    Assert.Equal(2, result[0].Value);
                    Assert.Equal(1, result[1].Value);
                });
            }

            [Fact]
            private void sets_intercepted_value_when_InterceptedGenericTypeArray_is_called()
            {
                Execute(() =>
                {
                    var instance = new InterceptPropertiesClass<InheritsFromSimpleClass>();

                    instance.InterceptedGenericTypeArray = new[] { new InheritsFromSimpleClass(3) };
                    var result = instance.InterceptedGenericTypeArray;

                    Assert.Equal(2, result.Length);
                    Assert.Equal(4, result[0].Value);
                    Assert.Equal(3, result[1].Value);
                });
            }
        }

        public class Lists
        {
            [Fact]
            private void returns_original_value_when_OriginalValueTypeList_is_called()
            {
                Execute(() =>
                {
                    var instance = new InterceptPropertiesClass<InheritsFromSimpleClass>();
                    var result = instance.OriginalValueTypeList;

                    Assert.Single(result);
                    Assert.Equal(1, result[0]);
                });
            }

            [Fact]
            private void sets_original_value_when_OriginalValueTypeList_is_called()
            {
                Execute(() =>
                {
                    var instance = new InterceptPropertiesClass<InheritsFromSimpleClass>();

                    instance.OriginalValueTypeList = new List<int> { 3 };
                    var result = instance.OriginalValueTypeList;

                    Assert.Single(result);
                    Assert.Equal(3, result[0]);
                });
            }

            [Fact]
            private void returns_intercepted_value_when_InterceptedValueTypeList_is_called()
            {
                Execute(() =>
                {
                    var instance = new InterceptPropertiesClass<InheritsFromSimpleClass>();
                    var result = instance.InterceptedValueTypeList;

                    Assert.Equal(2, result.Count);
                    Assert.Equal(2, result[0]);
                    Assert.Equal(1, result[1]);
                });
            }

            [Fact]
            private void sets_intercepted_value_when_InterceptedValueTypeList_is_called()
            {
                Execute(() =>
                {
                    var instance = new InterceptPropertiesClass<InheritsFromSimpleClass>();

                    instance.InterceptedValueTypeList = new List<int> { 3 };
                    var result = instance.InterceptedValueTypeList;

                    Assert.Equal(2, result.Count);
                    Assert.Equal(4, result[0]);
                    Assert.Equal(3, result[1]);
                });
            }

            [Fact]
            private void returns_original_value_when_OriginalRefTypeList_is_called()
            {
                Execute(() =>
                {
                    var instance = new InterceptPropertiesClass<InheritsFromSimpleClass>();
                    var result = instance.OriginalRefTypeList;

                    Assert.Single(result);
                    Assert.Equal("a", result[0]);
                });
            }

            [Fact]
            private void sets_original_value_when_OriginalRefTypeList_is_called()
            {
                Execute(() =>
                {
                    var instance = new InterceptPropertiesClass<InheritsFromSimpleClass>();

                    instance.OriginalRefTypeList = new List<string> { "c" };
                    var result = instance.OriginalRefTypeList;

                    Assert.Single(result);
                    Assert.Equal("c", result[0]);
                });
            }

            [Fact]
            private void returns_intercepted_value_when_InterceptedRefTypeList_is_called()
            {
                Execute(() =>
                {
                    var instance = new InterceptPropertiesClass<InheritsFromSimpleClass>();
                    var result = instance.InterceptedRefTypeList;

                    Assert.Equal(2, result.Count);
                    Assert.Equal("cd", result[0]);
                    Assert.Equal("ab", result[1]);
                });
            }

            [Fact]
            private void sets_intercepted_value_when_InterceptedRefTypeList_is_called()
            {
                Execute(() =>
                {
                    var instance = new InterceptPropertiesClass<InheritsFromSimpleClass>();

                    instance.InterceptedRefTypeList = new List<string> { "c" };
                    var result = instance.InterceptedRefTypeList;

                    Assert.Equal(2, result.Count);
                    Assert.Equal("ca", result[0]);
                    Assert.Equal("cb", result[1]);
                });
            }

            [Fact]
            private void returns_original_value_when_OriginalExternalValueTypeList_is_called()
            {
                Execute(() =>
                {
                    var instance = new InterceptPropertiesClass<InheritsFromSimpleClass>();
                    var result = instance.OriginalExternalValueTypeList;

                    Assert.Single(result);
                    Assert.Equal(1, result[0].Value);
                });
            }

            [Fact]
            private void sets_original_value_when_OriginalExternalValueTypeList_is_called()
            {
                Execute(() =>
                {
                    var instance = new InterceptPropertiesClass<InheritsFromSimpleClass>();

                    instance.OriginalExternalValueTypeList = new List<SimpleStruct> { new SimpleStruct(3) };
                    var result = instance.OriginalExternalValueTypeList;

                    Assert.Single(result);
                    Assert.Equal(3, result[0].Value);
                });
            }

            [Fact]
            private void returns_intercepted_value_when_InterceptedExternalValueType_is_called()
            {
                Execute(() =>
                {
                    var instance = new InterceptPropertiesClass<InheritsFromSimpleClass>();
                    var result = instance.InterceptedExternalValueTypeList;

                    Assert.Equal(2, result.Count);
                    Assert.Equal(2, result[0].Value);
                    Assert.Equal(1, result[1].Value);
                });
            }

            [Fact]
            private void sets_intercepted_value_when_InterceptedExternalValueType_is_called()
            {
                Execute(() =>
                {
                    var instance = new InterceptPropertiesClass<InheritsFromSimpleClass>();

                    instance.InterceptedExternalValueTypeList = new List<SimpleStruct> { new SimpleStruct(3) };
                    var result = instance.InterceptedExternalValueTypeList;

                    Assert.Equal(2, result.Count);
                    Assert.Equal(4, result[0].Value);
                    Assert.Equal(3, result[1].Value);
                });
            }

            [Fact]
            private void returns_original_value_when_OriginalExternalRefType_is_called()
            {
                Execute(() =>
                {
                    var instance = new InterceptPropertiesClass<InheritsFromSimpleClass>();
                    var result = instance.OriginalExternalRefTypeList;

                    Assert.Single(result);
                    Assert.Equal(1, result[0].Value);
                });
            }

            [Fact]
            private void sets_original_value_when_OriginalExternalRefType_is_called()
            {
                Execute(() =>
                {
                    var instance = new InterceptPropertiesClass<InheritsFromSimpleClass>();

                    instance.OriginalExternalRefTypeList = new List<SimpleClass> { new SimpleClass(3) };
                    var result = instance.OriginalExternalRefTypeList;

                    Assert.Single(result);
                    Assert.Equal(3, result[0].Value);
                });
            }

            [Fact]
            private void returns_intercepted_value_when_InterceptedExternalRefTypeList_is_called()
            {
                Execute(() =>
                {
                    var instance = new InterceptPropertiesClass<InheritsFromSimpleClass>();
                    var result = instance.InterceptedExternalRefTypeList;

                    Assert.Equal(2, result.Count);
                    Assert.Equal(2, result[0].Value);
                    Assert.Equal(1, result[1].Value);
                });
            }

            [Fact]
            private void sets_intercepted_value_when_InterceptedExternalRefTypeList_is_called()
            {
                Execute(() =>
                {
                    var instance = new InterceptPropertiesClass<InheritsFromSimpleClass>();

                    instance.InterceptedExternalRefTypeList = new List<SimpleClass> { new SimpleClass(3) };
                    var result = instance.InterceptedExternalRefTypeList;

                    Assert.Equal(2, result.Count);
                    Assert.Equal(4, result[0].Value);
                    Assert.Equal(3, result[1].Value);
                });
            }

            [Fact]
            private void returns_original_value_when_OriginalGenericTypeList_is_called()
            {
                Execute(() =>
                {
                    var instance = new InterceptPropertiesClass<InheritsFromSimpleClass>();
                    var result = instance.OriginalGenericTypeList;

                    Assert.Single(result);
                    Assert.Equal(1, result[0].Value);
                });
            }

            [Fact]
            private void sets_original_value_when_OriginalGenericTypeList_is_called()
            {
                Execute(() =>
                {
                    var instance = new InterceptPropertiesClass<InheritsFromSimpleClass>();

                    instance.OriginalGenericTypeList = new List<InheritsFromSimpleClass> { new InheritsFromSimpleClass(3) };
                    var result = instance.OriginalGenericTypeList;

                    Assert.Single(result);
                    Assert.Equal(3, result[0].Value);
                });
            }

            [Fact]
            private void returns_intercepted_value_when_InterceptedGenericTypeList_is_called()
            {
                Execute(() =>
                {
                    var instance = new InterceptPropertiesClass<InheritsFromSimpleClass>();
                    var result = instance.InterceptedGenericTypeList;

                    Assert.Equal(2, result.Count);
                    Assert.Equal(2, result[0].Value);
                    Assert.Equal(1, result[1].Value);
                });
            }

            [Fact]
            private void sets_intercepted_value_when_InterceptedGenericTypeList_is_called()
            {
                Execute(() =>
                {
                    var instance = new InterceptPropertiesClass<InheritsFromSimpleClass>();

                    instance.InterceptedGenericTypeList = new List<InheritsFromSimpleClass> { new InheritsFromSimpleClass(3) };
                    var result = instance.InterceptedGenericTypeList;

                    Assert.Equal(2, result.Count);
                    Assert.Equal(4, result[0].Value);
                    Assert.Equal(3, result[1].Value);
                });
            }
        }

        public class IndexerProperty
        {
            [Fact]
            private void returns_original_value_when_IndexerProperty_is_called()
            {
                Execute(() =>
                {
                    var instance = new InterceptPropertiesClass<InheritsFromSimpleClass>();
                    instance._indexerValue = new Tuple<int, string, SimpleStruct, SimpleClass, InheritsFromSimpleClass, object>(2, "b", new SimpleStruct(2), new SimpleClass(2), new InheritsFromSimpleClass(2), null);

                    var result = instance[1, "a", new SimpleStruct(1), new SimpleClass(1), new InheritsFromSimpleClass(1)];

                    Assert.NotNull(result);
                    Assert.Equal(2, result.Item1);
                    Assert.Equal("b", result.Item2);
                    Assert.Equal(2, result.Item3.Value);
                    Assert.Equal(2, result.Item4.Value);
                    Assert.Equal(2, result.Item5.Value);
                    Assert.Null(result.Item6);
                });
            }

            [Fact]
            private void sets_original_value_when_IndexerProperty_is_called()
            {
                Execute(() =>
                {
                    var instance = new InterceptPropertiesClass<InheritsFromSimpleClass>();

                    instance[1, "a", new SimpleStruct(1), new SimpleClass(1), new InheritsFromSimpleClass(1)] = new Tuple<int, string, SimpleStruct, SimpleClass, InheritsFromSimpleClass, object>(2, "b", new SimpleStruct(2), new SimpleClass(2), new InheritsFromSimpleClass(2), null);
                    var result = instance._indexerValue;

                    Assert.NotNull(result);
                    Assert.Equal(1, result.Item1);
                    Assert.Equal("a", result.Item2);
                    Assert.Equal(1, result.Item3.Value);
                    Assert.Equal(1, result.Item4.Value);
                    Assert.Equal(1, result.Item5.Value);
                    Assert.NotNull(result.Item6);

                    var item6 = result.Item6 as Tuple<int, string, SimpleStruct, SimpleClass, InheritsFromSimpleClass, object>;
                    Assert.Equal(2, item6.Item1);
                    Assert.Equal("b", item6.Item2);
                    Assert.Equal(2, item6.Item3.Value);
                    Assert.Equal(2, item6.Item4.Value);
                    Assert.Equal(2, item6.Item5.Value);
                    Assert.Null(item6.Item6);
                });
            }

            [Fact]
            private void returns_intercepted_value_when_IndexerProperty_is_called()
            {
                Execute(() =>
                {
                    var instance = new InterceptPropertiesClass<InheritsFromSimpleClass>();
                    instance._indexerValue = new Tuple<int, string, SimpleStruct, SimpleClass, InheritsFromSimpleClass, object>(1, "a", new SimpleStruct(1), new SimpleClass(1), new InheritsFromSimpleClass(1), null);

                    var result = instance[2, "b", new SimpleStruct(2), new SimpleClass(2), new InheritsFromSimpleClass(2)];

                    Assert.NotNull(result);
                    Assert.Equal(3, result.Item1);
                    Assert.Equal("c", result.Item2);
                    Assert.Equal(3, result.Item3.Value);
                    Assert.Equal(3, result.Item4.Value);
                    Assert.Equal(3, result.Item5.Value);
                    Assert.NotNull(result.Item6);

                    var item6 = result.Item6 as Tuple<int, string, SimpleStruct, SimpleClass, InheritsFromSimpleClass, object>;
                    Assert.Equal(4, item6.Item1);
                    Assert.Equal("d", item6.Item2);
                    Assert.Equal(4, item6.Item3.Value);
                    Assert.Equal(4, item6.Item4.Value);
                    Assert.Equal(4, item6.Item5.Value);
                    Assert.Null(item6.Item6);
                });
            }

            [Fact]
            private void sets_intercepted_value_when_IndexerProperty_is_called()
            {
                Execute(() =>
                {
                    var instance = new InterceptPropertiesClass<InheritsFromSimpleClass>();

                    instance[2, "b", new SimpleStruct(2), new SimpleClass(2), new InheritsFromSimpleClass(2)] = new Tuple<int, string, SimpleStruct, SimpleClass, InheritsFromSimpleClass, object>(1, "a", new SimpleStruct(1), new SimpleClass(1), new InheritsFromSimpleClass(1), null);
                    var result = instance._indexerValue;

                    Assert.NotNull(result);
                    Assert.Equal(3, result.Item1);
                    Assert.Equal("c", result.Item2);
                    Assert.Equal(3, result.Item3.Value);
                    Assert.Equal(3, result.Item4.Value);
                    Assert.Equal(3, result.Item5.Value);
                    Assert.NotNull(result.Item6);

                    var item6 = result.Item6 as Tuple<int, string, SimpleStruct, SimpleClass, InheritsFromSimpleClass, object>;
                    Assert.Equal(4, item6.Item1);
                    Assert.Equal("d", item6.Item2);
                    Assert.Equal(4, item6.Item3.Value);
                    Assert.Equal(4, item6.Item4.Value);
                    Assert.Equal(4, item6.Item5.Value);
                    Assert.Null(item6.Item6);
                });
            }
        }

        public class GetOnlyYieldProperty
        {
            [Fact]
            private void returns_original_value_when_OriginalGetOnlyYieldProperty_is_called()
            {
                Execute(() =>
                {
                    var instance = new InterceptPropertiesClass<InheritsFromSimpleClass>();
                    var result = instance.OriginalGetOnlyYieldProperty.ToArray();

                    Assert.Equal(3, result.Length);
                    Assert.Equal(0, result[0]);
                    Assert.Equal(1, result[1]);
                    Assert.Equal(2, result[2]);
                });
            }

            [Fact]
            private void returns_intercepted_value_when_InterceptedGetOnlyYieldProperty_is_called()
            {
                Execute(() =>
                {
                    var instance = new InterceptPropertiesClass<InheritsFromSimpleClass>();
                    var result = instance.InterceptedGetOnlyYieldProperty.ToArray();

                    Assert.Equal(4, result.Length);
                    Assert.Equal(5, result[0]);
                    Assert.Equal(6, result[1]);
                    Assert.Equal(7, result[2]);
                    Assert.Equal(8, result[3]);
                });
            }
        }

        public class SetOnlyProperty
        {
            [Fact]
            private void sets_original_value_when_OriginalSetOnlyProperty_is_called()
            {
                Execute(() =>
                {
                    var instance = new InterceptPropertiesClass<InheritsFromSimpleClass>();

                    instance.OriginalSetOnlyProperty = 3;

                    Assert.Equal(3, instance._setOnlyPropertyValue);
                });
            }

            [Fact]
            private void sets_intercepted_value_when_InterceptedSetOnlyProperty_is_called()
            {
                Execute(() =>
                {
                    var instance = new InterceptPropertiesClass<InheritsFromSimpleClass>();

                    instance.InterceptedSetOnlyProperty = 3;

                    Assert.Equal(4, instance._setOnlyPropertyValue);
                });
            }
        }

        public class InterceptedByMultipleAspects
        {
            [Fact]
            private void returns_intercepted_value_when_InterceptedByMultipleAspects_is_called()
            {
                Execute(() =>
                {
                    var instance = new InterceptPropertiesClass<InheritsFromSimpleClass>();
                    instance._interceptedByMultipleAspectsValue = 1;

                    var result = instance.InterceptedByMultipleAspects;

                    Assert.Equal(4, result);
                });
            }

            [Fact]
            private void sets_intercepted_value_when_InterceptedByMultipleAspects_is_called()
            {
                Execute(() =>
                {
                    var instance = new InterceptPropertiesClass<InheritsFromSimpleClass>();

                    instance.InterceptedByMultipleAspects = 2;
                    var result = instance._interceptedByMultipleAspectsValue;

                    Assert.Equal(13, result);
                });
            }
        }
    }
}
