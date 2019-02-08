using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestAssembly.Aspects;
using TestAssembly.Aspects.ImplementInterface;
using TestAssembly.ClassesToWeave;
using TestAssemblyReference;
using Xunit;

namespace CodeLoom.Tests
{
    public class ImplementInterfacesTest : BaseTest
    {
        public class ImplementSimpleMethodsClassTest
        {
            [Fact]
            private void should_implement_ISimpleMethods()
            {
                Execute(async () =>
                {
                    var @object = new ImplementSimpleMethodsClass();
                    var @interface = @object as ISimpleMethods;

                    var valueType = @interface.ReturnValueType();
                    var refType = @interface.ReturnRefType();
                    var externalValueType = @interface.ReturnExternalValueType();
                    var externalRefType = @interface.ReturnExternalRefType();
                    var asyncInt = await @interface.ReturnAsyncInt(2);

                    Assert.Equal(1, valueType);
                    Assert.Equal("a", refType);
                    Assert.Equal(1, externalValueType.Value);
                    Assert.Equal(1, externalRefType.Value);
                    Assert.Equal(2, asyncInt);
                });
            }

            [Fact]
            private void should_implement_ISimpleArrayMethods()
            {
                Execute(() =>
                {
                    var @object = new ImplementSimpleMethodsClass();
                    var @interface = @object as ISimpleArrayMethods;

                    var valueTypeArray = @interface.ReturnValueTypeArray();
                    var refTypeArray = @interface.ReturnRefTypeArray();
                    var externalValueTypeArray = @interface.ReturnExternalValueTypeArray();
                    var externalRefTypeArray = @interface.ReturnExternalRefTypeArray();

                    Assert.Equal(1, valueTypeArray[0]);
                    Assert.Equal(2, valueTypeArray[1]);
                    Assert.Equal("a", refTypeArray[0]);
                    Assert.Equal("b", refTypeArray[1]);
                    Assert.Equal(1, externalValueTypeArray[0].Value);
                    Assert.Equal(2, externalValueTypeArray[1].Value);
                    Assert.Equal(1, externalRefTypeArray[0].Value);
                    Assert.Equal(2, externalRefTypeArray[1].Value);
                });
            }

            [Fact]
            private void should_implement_ISimpleListMethods()
            {
                Execute(() =>
                {
                    var @object = new ImplementSimpleMethodsClass();
                    var @interface = @object as ISimpleListMethods;

                    var valueTypeList = @interface.ReturnValueTypeList();
                    var refTypeList = @interface.ReturnRefTypeList();
                    var externalValueTypeList = @interface.ReturnExternalValueTypeList();
                    var externalRefTypeList = @interface.ReturnExternalRefTypeList();
                    var yieldIntList = @interface.YieldIntList(4,5,6).ToArray();

                    Assert.Equal(1, valueTypeList[0]);
                    Assert.Equal(2, valueTypeList[1]);
                    Assert.Equal("a", refTypeList[0]);
                    Assert.Equal("b", refTypeList[1]);
                    Assert.Equal(1, externalValueTypeList[0].Value);
                    Assert.Equal(2, externalValueTypeList[1].Value);
                    Assert.Equal(1, externalRefTypeList[0].Value);
                    Assert.Equal(2, externalRefTypeList[1].Value);
                    Assert.Equal(4, yieldIntList[0]);
                    Assert.Equal(5, yieldIntList[1]);
                    Assert.Equal(6, yieldIntList[2]);
                });
            }

            [Fact]
            private void should_implement_ISimpleMethodsWithRefParameters()
            {
                Execute(() =>
                {
                    var @object = new ImplementSimpleMethodsClass();
                    var @interface = @object as ISimpleMethodsWithRefParameters;

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

                    var result = @interface.ReturnRefParametersAsString(ref p1, ref p2, ref p3, ref p4, ref p5, ref p6, ref p7, ref p8, ref p9, ref p10, ref p11, ref p12);

                    Assert.Equal(expected, result);
                });
            }

            [Fact]
            private void should_implement_ISimpleMethodsWithOutParameters()
            {
                Execute(() =>
                {
                    var @object = new ImplementSimpleMethodsClass();
                    var @interface = @object as ISimpleMethodsWithOutParameters;

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

                    int out1;
                    string out2;
                    SimpleStruct out3;
                    SimpleClass out4;
                    int[] out5;
                    string[] out6;
                    SimpleStruct[] out7;
                    SimpleClass[] out8;
                    List<int> out9;
                    List<string> out10;
                    List<SimpleStruct> out11;
                    List<SimpleClass> out12;

                    @interface.ReturnOutParameters(out out1, out out2, out out3, out out4, out out5, out out6, out out7, out out8, out out9, out out10, out out11, out out12);

                    Assert.Equal(p1, out1);
                    Assert.Equal(p2, out2);
                    Assert.Equal(p3.Value, out3.Value);
                    Assert.Equal(p4.Value, out4.Value);
                    Assert.Equal(p5[0], out5[0]);
                    Assert.Equal(p6[0], out6[0]);
                    Assert.Equal(p7[0].Value, out7[0].Value);
                    Assert.Equal(p8[0].Value, out8[0].Value);
                    Assert.Equal(p9[0], out9[0]);
                    Assert.Equal(p10[0], out10[0]);
                    Assert.Equal(p11[0].Value, out11[0].Value);
                    Assert.Equal(p12[0].Value, out12[0].Value);
                });
            }

            [Fact]
            private void should_implement_ISimpleProperties()
            {
                Execute(() =>
                {
                    var @object = new ImplementSimpleMethodsClass();
                    var @interface = @object as ISimpleProperties;

                    @interface.ValueType = 2;
                    var valueType = @interface.ValueType;

                    @interface.RefTypeArray = new[] { "a", "b" };
                    var refTypeArray = @interface.RefTypeArray;

                    @interface.ExternalRefTypeList = new List<SimpleClass> { new SimpleClass(1), new SimpleClass(2) };
                    var externalValueTypeList = @interface.ExternalRefTypeList;

                    @interface._readOnlyPropBackingField = 3;
                    var readOnly = @interface.ReadOnly;

                    @interface.WriteOnly = 4;
                    var writeOnly = @interface._writeOnlyPropBackingField;

                    @interface[1] = 2;
                    var indexer1 = @interface._indexer1PropBackingField;

                    @interface[1,2] = 3;
                    var indexer2 = @interface._indexer2PropBackingField;

                    Assert.Equal(2, valueType);
                    Assert.Equal("a", refTypeArray[0]);
                    Assert.Equal("b", refTypeArray[1]);
                    Assert.Equal(1, externalValueTypeList[0].Value);
                    Assert.Equal(2, externalValueTypeList[1].Value);
                    Assert.Equal(3, readOnly);
                    Assert.Equal(4, writeOnly);
                    Assert.Equal(1, indexer1.Item1);
                    Assert.Equal(2, indexer1.Item2);
                    Assert.Equal(1, indexer2.Item1);
                    Assert.Equal(2, indexer2.Item2);
                    Assert.Equal(3, indexer2.Item3);
                });
            }
        }

        public class ImplementGenericMethodsClassTest
        {
            [Fact]
            private void should_implement_IMethodsFromGenericInterface_with_generic_params_int_and_string()
            {
                Execute(() =>
                {
                    var @object = new ImplementGenericMethodsClass();
                    var @interface = @object as IMethodsFromGenericInterface<int, string>;

                    var value = @interface.ReturnValueFromFirstParameter(1, "a");

                    int ref1 = 2;
                    string ref2 = "b";
                    var refAsString = @interface.ReturnRefParametersAsString(ref ref1, ref ref2);

                    int out1;
                    string out2;
                    int[] out3;
                    string[] out4;
                    List<int> out5;
                    List<string> out6;
                    @interface.ReturnOutParameters(out out1, out out2, out out3, out out4, out out5, out out6);

                    Assert.Equal(1, value);
                    Assert.Equal(Helper.AsString(ref1, ref2), refAsString);
                    Assert.Equal(0, out1);
                    Assert.Null(out2);
                    Assert.Equal(0, out3[0]);
                    Assert.Null(out4[0]);
                    Assert.Equal(0, out5[0]);
                    Assert.Null(out6[0]);
                });
            }

            [Fact]
            private void should_implement_IGenericMethodsFromGenericInterface_with_generic_params_int_and_string()
            {
                Execute(() =>
                {
                    var @object = new ImplementGenericMethodsClass();
                    var @interface = @object as IGenericMethodsFromGenericInterface<int, string>;

                    var value = @interface.GenericReturnValueFromFirstParameter(1, "a", "b", "c");

                    int ref1 = 2;
                    string ref2 = "b";
                    string ref3 = "bb";
                    string ref4 = "bbb";
                    var refAsString = @interface.GenericReturnRefParametersAsString(ref ref1, ref ref2, ref ref3, ref ref4);

                    int out1;
                    string out2;
                    string out3;
                    string out4;
                    int[] out5;
                    string[] out6;
                    string[] out7;
                    string[] out8;
                    List<int> out9;
                    List<string> out10;
                    List<string> out11;
                    List<string> out12;
                    @interface.GenericReturnOutParameters(out out1, out out2, out out3, out out4, out out5, out out6, out out7, out out8, out out9, out out10, out out11, out out12);

                    Assert.Equal(1, value);
                    Assert.Equal(Helper.AsString(ref1, ref2, ref3, ref4), refAsString);
                    Assert.Equal(0, out1);
                    Assert.Null(out2);
                    Assert.Null(out3);
                    Assert.Null(out4);
                    Assert.Equal(0, out5[0]);
                    Assert.Null(out6[0]);
                    Assert.Null(out7[0]);
                    Assert.Null(out8[0]);
                    Assert.Equal(0, out9[0]);
                    Assert.Null(out10[0]);
                    Assert.Null(out11[0]);
                    Assert.Null(out12[0]);
                });
            }

            [Fact]
            private void should_implement_IGenericMethodsFromGenericInterfaceWithRepeatedGenericParameter_with_generic_params_int_and_string()
            {
                Execute(() =>
                {
                    var @object = new ImplementGenericMethodsClass();
                    var @interface = @object as IGenericMethodsFromGenericInterfaceWithRepeatedGenericParameter<int, string>;

                    var value = @interface.GenericWithRepeatedGenericParameterReturnValueFromFirstParameter(1, "a", "b");

                    int ref1 = 2;
                    string ref2 = "b";
                    string ref3 = "bb";
                    var refAsString = @interface.GenericWithRepeatedGenericParameterReturnRefParametersAsString(ref ref1, ref ref2, ref ref3);

                    int out1;
                    string out2;
                    string out3;
                    int[] out4;
                    string[] out5;
                    string[] out6;
                    List<int> out7;
                    List<string> out8;
                    List<string> out9;
                    @interface.GenericWithRepeatedGenericParameterReturnOutParameters(out out1, out out2, out out3, out out4, out out5, out out6, out out7, out out8, out out9);

                    Assert.Equal(1, value);
                    Assert.Equal(Helper.AsString(ref1, ref2, ref3), refAsString);
                    Assert.Equal(0, out1);
                    Assert.Null(out2);
                    Assert.Null(out3);
                    Assert.Equal(0, out4[0]);
                    Assert.Null(out5[0]);
                    Assert.Null(out6[0]);
                    Assert.Equal(0, out7[0]);
                    Assert.Null(out8[0]);
                    Assert.Null(out9[0]);
                });
            }

            [Fact]
            private void should_implement_IGenericProperties_with_generic_params_int_and_string()
            {
                Execute(() =>
                {
                    var @object = new ImplementGenericMethodsClass();
                    var @interface = @object as IGenericProperties<int, string>;

                    @interface.T1Value = 2;
                    var t1Value = @interface.T1Value;

                    @interface.T1List = new List<int> { 3, 4 };
                    var t1List = @interface.T1List;

                    @interface.T1T2TupleList = new List<Tuple<int, string>> { new Tuple<int, string>(5, "a"), new Tuple<int, string>(6, "b") };
                    var t1t2TupleList = @interface.T1T2TupleList;

                    Assert.Equal(2, t1Value);
                    Assert.Equal(3, t1List[0]);
                    Assert.Equal(4, t1List[1]);
                    Assert.Equal(5, t1t2TupleList[0].Item1);
                    Assert.Equal("a", t1t2TupleList[0].Item2);
                    Assert.Equal(6, t1t2TupleList[1].Item1);
                    Assert.Equal("b", t1t2TupleList[1].Item2);
                });
            }

            [Fact]
            private void should_implement_IGenericConstraints_with_generic_params_int_and_string()
            {
                Execute(() =>
                {
                    var @object = new ImplementGenericMethodsClass();
                    var @interface = @object as IGenericConstraints<int, string>;

                    var value1 = @interface.GenericConstraintMethod(1, "a");
                    var value2 = @interface.GenericConstraintMethodWithRepeatedGenericParameter(2, new SimpleStruct(3));

                    Assert.Equal(1, value1);
                    Assert.Equal(2, value2);
                });
            }

            [Fact]
            private void should_implement_IMethodsFromGenericInterface_with_generic_params_SimpleStruct_and_SimpleClass()
            {
                Execute(() =>
                {
                    var @object = new ImplementGenericMethodsClass();
                    var @interface = @object as IMethodsFromGenericInterface<SimpleStruct, SimpleClass>;

                    var value = @interface.ReturnValueFromFirstParameter(new SimpleStruct(1), new SimpleClass(2));

                    var ref1 = new SimpleStruct(1);
                    var ref2 = new SimpleClass(1);
                    var refAsString = @interface.ReturnRefParametersAsString(ref ref1, ref ref2);

                    SimpleStruct out1;
                    SimpleClass out2;
                    SimpleStruct[] out3;
                    SimpleClass[] out4;
                    List<SimpleStruct> out5;
                    List<SimpleClass> out6;
                    @interface.ReturnOutParameters(out out1, out out2, out out3, out out4, out out5, out out6);

                    Assert.Equal(1, value.Value);
                    Assert.Equal(Helper.AsString(ref1, ref2), refAsString);
                    Assert.Equal(new SimpleStruct(), out1);
                    Assert.Null(out2);
                    Assert.Equal(new SimpleStruct(), out3[0]);
                    Assert.Null(out4[0]);
                    Assert.Equal(new SimpleStruct(), out5[0]);
                    Assert.Null(out6[0]);
                });
            }

            [Fact]
            private void should_implement_IGenericMethodsFromGenericInterface_with_generic_params_SimpleStruct_and_SimpleClass()
            {
                Execute(() =>
                {
                    var @object = new ImplementGenericMethodsClass();
                    var @interface = @object as IGenericMethodsFromGenericInterface<SimpleStruct, SimpleClass>;

                    var value = @interface.GenericReturnValueFromFirstParameter(new SimpleStruct(1), new SimpleClass(2), "b", "c");

                    SimpleStruct ref1 = new SimpleStruct(1);
                    SimpleClass ref2 = new SimpleClass(1);
                    string ref3 = "bb";
                    string ref4 = "bbb";
                    var refAsString = @interface.GenericReturnRefParametersAsString(ref ref1, ref ref2, ref ref3, ref ref4);

                    SimpleStruct out1;
                    SimpleClass out2;
                    string out3;
                    string out4;
                    SimpleStruct[] out5;
                    SimpleClass[] out6;
                    string[] out7;
                    string[] out8;
                    List<SimpleStruct> out9;
                    List<SimpleClass> out10;
                    List<string> out11;
                    List<string> out12;
                    @interface.GenericReturnOutParameters(out out1, out out2, out out3, out out4, out out5, out out6, out out7, out out8, out out9, out out10, out out11, out out12);

                    Assert.Equal(1, value.Value);
                    Assert.Equal(Helper.AsString(ref1, ref2, ref3, ref4), refAsString);
                    Assert.Equal(new SimpleStruct(), out1);
                    Assert.Null(out2);
                    Assert.Null(out3);
                    Assert.Null(out4);
                    Assert.Equal(new SimpleStruct(), out5[0]);
                    Assert.Null(out6[0]);
                    Assert.Null(out7[0]);
                    Assert.Null(out8[0]);
                    Assert.Equal(new SimpleStruct(), out9[0]);
                    Assert.Null(out10[0]);
                    Assert.Null(out11[0]);
                    Assert.Null(out12[0]);
                });
            }

            [Fact]
            private void should_implement_IGenericMethodsFromGenericInterfaceWithRepeatedGenericParameter_with_generic_params_SimpleStruct_and_SimpleClass()
            {
                Execute(() =>
                {
                    var @object = new ImplementGenericMethodsClass();
                    var @interface = @object as IGenericMethodsFromGenericInterfaceWithRepeatedGenericParameter<SimpleStruct, SimpleClass>;

                    var value = @interface.GenericWithRepeatedGenericParameterReturnValueFromFirstParameter(new SimpleStruct(1), "a", "b");

                    SimpleStruct ref1 = new SimpleStruct(1);
                    string ref2 = "b";
                    string ref3 = "bb";
                    var refAsString = @interface.GenericWithRepeatedGenericParameterReturnRefParametersAsString(ref ref1, ref ref2, ref ref3);

                    SimpleStruct out1;
                    string out2;
                    string out3;
                    SimpleStruct[] out4;
                    string[] out5;
                    string[] out6;
                    List<SimpleStruct> out7;
                    List<string> out8;
                    List<string> out9;
                    @interface.GenericWithRepeatedGenericParameterReturnOutParameters(out out1, out out2, out out3, out out4, out out5, out out6, out out7, out out8, out out9);

                    Assert.Equal(1, value.Value);
                    Assert.Equal(Helper.AsString(ref1, ref2, ref3), refAsString);
                    Assert.Equal(new SimpleStruct(), out1);
                    Assert.Null(out2);
                    Assert.Null(out3);
                    Assert.Equal(new SimpleStruct(), out4[0]);
                    Assert.Null(out5[0]);
                    Assert.Null(out6[0]);
                    Assert.Equal(new SimpleStruct(), out7[0]);
                    Assert.Null(out8[0]);
                    Assert.Null(out9[0]);
                });
            }

            [Fact]
            private void should_implement_IGenericProperties_with_generic_params_SimpleStruct_and_SimpleClass()
            {
                Execute(() =>
                {
                    var @object = new ImplementGenericMethodsClass();
                    var @interface = @object as IGenericProperties<SimpleStruct, SimpleClass>;

                    @interface.T1Value = new SimpleStruct(1);
                    var t1Value = @interface.T1Value;

                    @interface.T1List = new List<SimpleStruct> { new SimpleStruct(1), new SimpleStruct(2) };
                    var t1List = @interface.T1List;

                    @interface.T1T2TupleList = new List<Tuple<SimpleStruct, SimpleClass>> { new Tuple<SimpleStruct, SimpleClass>(new SimpleStruct(1), new SimpleClass(2)), new Tuple<SimpleStruct, SimpleClass>(new SimpleStruct(1), new SimpleClass(2)) };
                    var t1t2TupleList = @interface.T1T2TupleList;

                    Assert.Equal(1, t1Value.Value);
                    Assert.Equal(1, t1List[0].Value);
                    Assert.Equal(2, t1List[1].Value);
                    Assert.Equal(1, t1t2TupleList[0].Item1.Value);
                    Assert.Equal(2, t1t2TupleList[0].Item2.Value);
                    Assert.Equal(1, t1t2TupleList[1].Item1.Value);
                    Assert.Equal(2, t1t2TupleList[1].Item2.Value);
                });
            }

            [Fact]
            private void should_implement_IGenericConstraints_with_generic_params_SimpleStruct_and_SimpleClass()
            {
                Execute(() =>
                {
                    var @object = new ImplementGenericMethodsClass();
                    var @interface = @object as IGenericConstraints<SimpleStruct, SimpleClass>;

                    var value1 = @interface.GenericConstraintMethod(new SimpleStruct(1), new SimpleClass(1));
                    var value2 = @interface.GenericConstraintMethodWithRepeatedGenericParameter(new SimpleStruct(2), 1);

                    Assert.Equal(1, value1.Value);
                    Assert.Equal(2, value2.Value);
                });
            }
        }
    }
}
