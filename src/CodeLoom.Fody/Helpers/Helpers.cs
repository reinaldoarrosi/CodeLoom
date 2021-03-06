﻿using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Cecil.Rocks;
using Mono.Collections.Generic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;

namespace CodeLoom.Fody.Helpers
{
    internal static class Helpers
    {
        internal abstract class ImplementationInfo
        {
            internal enum Types
            {
                Method,
                Property
            }

            internal class Method : ImplementationInfo
            {
                public Method(MethodInfo interfaceMethod, MethodInfo aspectMethod)
                {

                    InterfaceMethod = interfaceMethod;
                    AspectMethod = aspectMethod;
                }

                internal MethodInfo InterfaceMethod { get; set; }
                internal MethodInfo AspectMethod { get; set; }

                internal override Types Type {  get { return Types.Method; } }
            }

            internal class Property : ImplementationInfo
            {
                public Property(PropertyInfo propertyInfo)
                {
                    PropertyInfo = propertyInfo;
                }

                internal PropertyInfo PropertyInfo { get; set; }
                internal MethodInfo InterfaceGetter { get; set; }
                internal MethodInfo AspectGetter { get; set; }
                internal MethodInfo InterfaceSetter { get; set; }
                internal MethodInfo AspectSetter { get; set; }

                internal override Types Type { get { return Types.Property; } }
            }

            internal abstract Types Type { get; }
        }

        internal static IEnumerable<ImplementationInfo> GetImplementationInfo(Type aspectType, Type interfaceType)
        {
            var map = new Dictionary<MemberInfo, ImplementationInfo>();
            var interfaceMap = aspectType.GetInterfaceMap(interfaceType);
            var propertiesGetters = aspectType.GetProperties().Where(p => p.GetMethod != null).ToDictionary(p => p.GetMethod, p => p);
            var propertiesSetters = aspectType.GetProperties().Where(p => p.SetMethod != null).ToDictionary(p => p.SetMethod, p => p);

            for (int i = 0; i < interfaceMap.InterfaceMethods.Length; i++)
            {
                var interfaceMethod = interfaceMap.InterfaceMethods[i];
                var aspectMethod = interfaceMap.TargetMethods[i];

                if (propertiesGetters.ContainsKey(aspectMethod))
                {
                    var propertyInfo = propertiesGetters[aspectMethod];

                    if (!map.ContainsKey(propertyInfo))
                        map.Add(propertyInfo, new ImplementationInfo.Property(propertyInfo));

                    var propertyImplementationInfo = map[propertyInfo] as ImplementationInfo.Property;
                    propertyImplementationInfo.InterfaceGetter = interfaceMethod;
                    propertyImplementationInfo.AspectGetter = aspectMethod;
                }
                else if (propertiesSetters.ContainsKey(aspectMethod))
                {
                    var propertyInfo = propertiesSetters[aspectMethod];

                    if (!map.ContainsKey(propertyInfo))
                        map.Add(propertyInfo, new ImplementationInfo.Property(propertyInfo));

                    var propertyImplementationInfo = map[propertyInfo] as ImplementationInfo.Property;
                    propertyImplementationInfo.InterfaceSetter = interfaceMethod;
                    propertyImplementationInfo.AspectSetter = aspectMethod;
                }
                else
                {
                    map.Add(aspectMethod, new ImplementationInfo.Method(interfaceMethod, aspectMethod));
                }
            }

            return map.Values;
        }

        internal static string GetUniqueFieldName(TypeDefinition typeDefinition, string baseName)
        {
            var partialName = $"<{baseName}>_field_";
            var index = typeDefinition.Fields.Where(f => f.Name.StartsWith(partialName)).Count();
            return partialName + index;
        }

        internal static string GetUniquePropertyName(TypeDefinition typeDefinition, string baseName)
        {
            var partialName = $"<{baseName}>_prop_";
            var index = typeDefinition.Properties.Where(f => f.Name.StartsWith(partialName)).Count();
            return partialName + index;
        }

        internal static string GetUniqueMethodName(TypeDefinition typeDefinition, string baseName)
        {
            var partialName = $"<{baseName}>_method_";
            var index = typeDefinition.Methods.Where(f => f.Name.StartsWith(partialName)).Count();
            return partialName + index;
        }

        internal static string GetUniqueBindingName(TypeDefinition typeDefinition, string baseName)
        {
            var partialName = $"<{baseName}>Binding";
            var index = typeDefinition.NestedTypes.Where(t => t.Name.StartsWith(partialName)).Count();
            return partialName + index;
        }

        internal static void Append(this ILProcessor ilProcessor, IEnumerable<Instruction> instructions)
        {
            foreach (var instruction in instructions)
            {
                ilProcessor.Append(instruction);
            }
        }

        internal static void InsertBefore(this ILProcessor ilProcessor, Instruction target, IEnumerable<Instruction> instructions)
        {
            foreach (var instruction in instructions)
            {
                ilProcessor.InsertBefore(target, instruction);
            }
        }

        internal static void InsertAfter(this ILProcessor ilProcessor, Instruction target, IEnumerable<Instruction> instructions)
        {
            foreach (var instruction in instructions.Reverse())
            {
                ilProcessor.InsertAfter(target, instruction);
            }
        }

        internal static TypeReference MakeTypeReference(this TypeReference type, params GenericParameter[] arguments)
        {
            if (type.IsDefinition)
            {
                if (type.HasGenericParameters)
                {
                    var genericType = new GenericInstanceType(type);

                    for (int i = 0; i < type.GenericParameters.Count; i++)
                    {
                        genericType.GenericArguments.Add(arguments[i]);
                    }

                    return genericType;
                }
                else
                {
                    return type;
                }
            }

            if (type.IsByReference)
            {
                var byRefType = type as ByReferenceType;
                return byRefType.ElementType.MakeTypeReference(arguments);
            }

            if (type.IsArray)
            {
                var arrayType = type as ArrayType;
                return new ArrayType(arrayType.ElementType.MakeTypeReference(arguments));
            }

            if (type.IsGenericParameter)
            {
                return arguments.LastOrDefault(a => a.Name == type.Name) ?? type;
            }

            if (type.IsGenericInstance)
            {
                var currentGenericInstanceType = type as GenericInstanceType;
                var newGenericInstanceType = new GenericInstanceType(currentGenericInstanceType.ElementType);

                foreach (var genericArgument in currentGenericInstanceType.GenericArguments)
                {
                    var arg = genericArgument.MakeTypeReference(arguments);
                    newGenericInstanceType.GenericArguments.Add(arg);
                }

                return newGenericInstanceType;
            }

            if (type.HasGenericParameters)
            {
                var genericType = new GenericInstanceType(type);

                foreach (var genericParameter in type.GenericParameters)
                {
                    var arg = genericParameter.MakeTypeReference(arguments);
                    genericType.GenericArguments.Add(arg);
                }

                type = genericType;
            }

            return type;
        }

        internal static MethodReference MakeMethodReference(this MethodReference method, TypeReference declaringType, params GenericParameter[] arguments)
        {
            var methodRef = new MethodReference(method.Name, method.ReturnType, declaringType)
            {
                HasThis = method.HasThis,
                ExplicitThis = method.ExplicitThis,
                CallingConvention = method.CallingConvention
            };

            methodRef.CopyGenericParameters(method.GenericParameters);

            foreach (var original in method.Parameters)
            {
                var clone = new ParameterDefinition(original.Name, original.Attributes, original.ParameterType);

                foreach (var customAttr in original.CustomAttributes)
                {
                    clone.CustomAttributes.Add(customAttr);
                }

                methodRef.Parameters.Add(clone);
            }

            if (method.GenericParameters.Count > 0)
            {
                var methodGenericRef = new GenericInstanceMethod(methodRef);

                foreach (var genericParameter in method.GenericParameters)
                {
                    var arg = arguments.Last(a => a.Name == genericParameter.Name);
                    methodGenericRef.GenericArguments.Add(arg);
                }

                methodRef = methodGenericRef;
            }

            return methodRef;
        }

        internal static FieldReference MakeFieldReference(this FieldReference field, TypeReference declaringType)
        {
            return new FieldReference(field.Name, field.FieldType, declaringType);
        }

        internal static GenericInstanceType MakeGenericInstanceType(this TypeReference type, params TypeReference[] arguments)
        {
            return TypeReferenceRocks.MakeGenericInstanceType(type, arguments);
        }

        internal static GenericInstanceMethod MakeGenericInstanceMethod(this MethodReference method, params TypeReference[] arguments)
        {
            var genericInstanceMethod = new GenericInstanceMethod(method);

            foreach (var arg in arguments)
            {
                genericInstanceMethod.GenericArguments.Add(arg);
            }

            return genericInstanceMethod;
        }

        internal static void CopyGenericParameters(this IGenericParameterProvider target, Collection<GenericParameter> copySource)
        {
            var originalToCloneMap = new Dictionary<GenericParameter, GenericParameter>();

            foreach (var original in copySource)
            {
                var clone = new GenericParameter(original.Name, target) { Attributes = original.Attributes };
                clone.HasDefaultConstructorConstraint = original.HasDefaultConstructorConstraint;
                clone.HasNotNullableValueTypeConstraint = original.HasNotNullableValueTypeConstraint;
                clone.HasReferenceTypeConstraint = original.HasReferenceTypeConstraint;

                foreach (var customAttr in original.CustomAttributes)
                {
                    clone.CustomAttributes.Add(customAttr);
                }

                originalToCloneMap.Add(original, clone);
            }

            foreach (var item in originalToCloneMap)
            {
                var original = item.Key;
                var clone = item.Value;

                foreach (var constraint in original.Constraints)
                {
                    var cloneConstraint = constraint;

                    if (constraint.IsGenericParameter && originalToCloneMap.ContainsKey(constraint as GenericParameter))
                        cloneConstraint = originalToCloneMap[constraint as GenericParameter];

                    clone.Constraints.Add(cloneConstraint);
                }
            }

            foreach (var item in originalToCloneMap.Values)
            {
                target.GenericParameters.Add(item);
            }
        }

        internal static void CopyDebugInformation(this MethodDefinition clone, MethodDefinition originalMethod)
        {
            clone.DebugInformation.Scope = new ScopeDebugInformation(originalMethod.Body.Instructions.First(), originalMethod.Body.Instructions.Last());
            clone.DebugInformation.Scope.Import = originalMethod.DebugInformation.Scope.Import;
            clone.DebugInformation.StateMachineKickOffMethod = originalMethod.DebugInformation.StateMachineKickOffMethod;

            foreach (var sequencePoint in originalMethod.DebugInformation.SequencePoints)
                clone.DebugInformation.SequencePoints.Add(sequencePoint);

            foreach (var customDebugInfo in originalMethod.DebugInformation.CustomDebugInformations)
                clone.DebugInformation.CustomDebugInformations.Add(customDebugInfo);           

            foreach (var constant in originalMethod.DebugInformation.Scope.Constants)
                clone.DebugInformation.Scope.Constants.Add(constant);

            foreach (var customDebugInfo in originalMethod.DebugInformation.Scope.CustomDebugInformations)
                clone.DebugInformation.Scope.CustomDebugInformations.Add(customDebugInfo);

            foreach (var scope in originalMethod.DebugInformation.Scope.Scopes)
                clone.DebugInformation.Scope.Scopes.Add(scope);

            foreach (var variable in originalMethod.DebugInformation.Scope.Variables)
                clone.DebugInformation.Scope.Variables.Add(variable);
        }

        internal static bool IsAsyncMethod(this MethodDefinition method)
        {
            var asyncStateMachineAttribute = method.CustomAttributes.FirstOrDefault(attr => attr.AttributeType.FullName == typeof(AsyncStateMachineAttribute).FullName);
            if (asyncStateMachineAttribute == null) return false;

            var stateMachineType = (asyncStateMachineAttribute.ConstructorArguments.First().Value as TypeReference).Resolve();
            if (stateMachineType == null) return false;

            return stateMachineType.CustomAttributes.Any(attr => attr.AttributeType.FullName == typeof(CompilerGeneratedAttribute).FullName);
        }

        internal static bool IsAsyncMethod(this MethodBase method)
        {
            var asyncStateMachineAttribute = method.CustomAttributes.FirstOrDefault(attr => attr.AttributeType == typeof(AsyncStateMachineAttribute));
            if (asyncStateMachineAttribute == null) return false;

            var stateMachineType = (asyncStateMachineAttribute.ConstructorArguments.First().Value as Type);
            if (stateMachineType == null) return false;

            return stateMachineType.CustomAttributes.Any(attr => attr.AttributeType == typeof(CompilerGeneratedAttribute));
        }

        internal static Type GetSystemType(this TypeDefinition typeDefinition)
        {
            var type = TryGetSystemType(typeDefinition);

            if (type == null)
                throw new KeyNotFoundException($"Could not find Type {typeDefinition.FullName}");

            return type;
        }

        internal static Type TryGetSystemType(this TypeDefinition typeDefinition)
        {
            var typeName = $"{typeDefinition.GetStandardTypeName()}, {typeDefinition.Module.Assembly.FullName}";
            var type = Type.GetType(typeName);

            return type;
        }

        internal static FieldInfo GetFieldInfo(this FieldDefinition fieldDefinition)
        {
            var fieldInfo = TryGetFieldInfo(fieldDefinition);

            if (fieldInfo == null)
                throw new KeyNotFoundException($"Could not find FieldInfo for field {fieldDefinition.FullName}");

            return fieldInfo;
        }

        internal static FieldInfo TryGetFieldInfo(this FieldDefinition fieldDefinition)
        {
            var bindingFlags = BindingFlags.Public | BindingFlags.NonPublic;
            bindingFlags |= fieldDefinition.IsStatic ? BindingFlags.Static : BindingFlags.Instance;

            var declaringType = fieldDefinition.DeclaringType.GetSystemType();
            var fieldInfo = declaringType.GetField(fieldDefinition.Name, bindingFlags);

            return fieldInfo;
        }

        internal static PropertyInfo GetPropertyInfo(this PropertyDefinition propertyDefinition)
        {
            var propertyInfo = TryGetPropertyInfo(propertyDefinition);

            if (propertyInfo == null)
                throw new KeyNotFoundException($"Could not find PropertyInfo for property {propertyDefinition.FullName}");

            return propertyInfo;
        }

        internal static PropertyInfo TryGetPropertyInfo(this PropertyDefinition propertyDefinition)
        {
            PropertyInfo propertyInfo = null;

            var bindingFlags = BindingFlags.Public | BindingFlags.NonPublic;
            bindingFlags |= !propertyDefinition.HasThis ? BindingFlags.Static : BindingFlags.Instance;

            var declaringType = propertyDefinition.DeclaringType.GetSystemType();

            foreach (var property in declaringType.GetProperties(bindingFlags))
            {
                if (property.Name != propertyDefinition.Name)
                    continue;

                if (property.GetMethod == null && propertyDefinition.GetMethod != null)
                    continue;

                if (property.SetMethod == null && propertyDefinition.SetMethod != null)
                    continue;

                var definitionReturnType = propertyDefinition.PropertyType;
                var propertyReturnType = property.PropertyType;
                if (!definitionReturnType.SameTypeAs(propertyReturnType))
                    continue;

                var propertyParameters = property.GetMethod?.GetParameters() ?? property.SetMethod?.GetParameters();
                var propertyDefParameters = propertyDefinition.GetMethod?.Parameters ?? propertyDefinition.SetMethod?.Parameters;

                if (propertyParameters == null && propertyDefParameters != null)
                    continue;

                if (propertyParameters != null && propertyDefParameters == null)
                    continue;

                if (propertyParameters != null && propertyDefParameters != null && propertyParameters.Length != propertyDefParameters.Count)
                    continue;

                for (int i = 0; i < propertyDefParameters.Count; i++)
                {
                    var definitionParameterType = propertyDefParameters[i].ParameterType;
                    var propertyParameterType = propertyParameters[i].ParameterType;
                    if (!definitionParameterType.SameTypeAs(propertyParameterType))
                        continue;
                }

                propertyInfo = property;
                break;
            }

            return propertyInfo;
        }

        internal static MethodBase GetMethodBase(this MethodDefinition methodDefinition)
        {
            var methodInfo = TryGetMethodBase(methodDefinition);

            if (methodInfo == null)
                throw new KeyNotFoundException($"Could not find MethodInfo for method {methodDefinition.FullName}");

            return methodInfo;
        }

        internal static MethodBase TryGetMethodBase(this MethodDefinition methodDefinition)
        {
            MethodBase methodInfo = null;

            var bindingFlags = BindingFlags.Public | BindingFlags.NonPublic;
            bindingFlags |= methodDefinition.IsStatic ? BindingFlags.Static : BindingFlags.Instance;

            var declaringType = methodDefinition.DeclaringType.GetSystemType();

            if (methodDefinition.IsConstructor)
            {
                foreach (var ctor in declaringType.GetConstructors(bindingFlags))
                {
                    if (!ctor.CompareParameters(methodDefinition))
                        continue;

                    methodInfo = ctor;
                    break;
                }
            }
            else
            {
                foreach (var method in declaringType.GetMethods(bindingFlags))
                {
                    if (method.Name != methodDefinition.Name)
                        continue;

                    if (!method.CompareParameters(methodDefinition))
                        continue;

                    methodInfo = method;
                    break;
                }
            }

            return methodInfo;
        }

        internal static string GetSimpleTypeName(this TypeReference typeReference)
        {
            if (typeReference.IsByReference)
            {
                var byRefType = typeReference as ByReferenceType;
                return byRefType.ElementType.GetSimpleTypeName() + "&";
            }

            if (typeReference.IsArray)
            {
                var arrayType = typeReference as ArrayType;
                return arrayType.ElementType.GetSimpleTypeName() + "[]";
            }

            StringBuilder sb = new StringBuilder();

            if (typeReference.IsGenericParameter)
            {
                var genericParameter = typeReference as GenericParameter;

                if (genericParameter.DeclaringMethod?.DeclaringType != null)
                {
                    var declaringType = genericParameter.DeclaringMethod?.DeclaringType;
                    while (declaringType != null && declaringType.IsNested) declaringType = declaringType.DeclaringType;

                    if (!string.IsNullOrWhiteSpace(declaringType?.Namespace))
                        sb.AppendFormat("{0}.", declaringType.Namespace);
                }
                else if (genericParameter.DeclaringType != null)
                {
                    var declaringType = genericParameter.DeclaringType;
                    while (declaringType != null && declaringType.IsNested) declaringType = declaringType.DeclaringType;

                    if (!string.IsNullOrWhiteSpace(declaringType?.Namespace))
                        sb.AppendFormat("{0}.", declaringType.Namespace);
                }
            }
            else if (!string.IsNullOrWhiteSpace(typeReference.Namespace))
            {
                sb.AppendFormat("{0}.", typeReference.Namespace);
            }

            sb.Append(typeReference.Name);

            var genericTypeRef = typeReference as GenericInstanceType;

            if (genericTypeRef != null && genericTypeRef.GenericArguments.Count > 0)
            {
                var genericTypeSuffix = $"`{genericTypeRef.GenericArguments.Count}";
                sb.Length -= genericTypeSuffix.Length;

                sb.Append("<");

                foreach (var genericParameter in genericTypeRef.GenericArguments)
                {
                    sb.AppendFormat("{0},", genericParameter.GetSimpleTypeName());
                }

                sb.Length -= 1;
                sb.Append(">");
            }

            return sb.ToString();
        }

        internal static string GetSimpleTypeName(this Type type)
        {
            if (type.IsByRef)
            {
                return type.GetElementType().GetSimpleTypeName() + "&";
            }

            StringBuilder sb = new StringBuilder();

            if (!string.IsNullOrWhiteSpace(type.Namespace))
                sb.AppendFormat("{0}.", type.Namespace);

            sb.Append(type.Name);

            if (type.GenericTypeArguments.Length > 0)
            {
                var genericTypeSuffix = $"`{type.GenericTypeArguments.Length}";
                sb.Length -= genericTypeSuffix.Length;

                sb.Append("<");

                foreach (var genericParameter in type.GenericTypeArguments)
                {
                    sb.AppendFormat("{0},", genericParameter.GetSimpleTypeName());
                }

                sb.Length -= 1;
                sb.Append(">");
            }

            return sb.ToString();
        }

        internal static string GetStandardTypeName(this TypeReference typeReference)
        {
            if (typeReference.FullName == null || typeReference.IsGenericParameter)
                return null;

            if (typeReference.IsGenericInstance)
            {
                var genericInstance = (GenericInstanceType)typeReference;
                return string.Format("{0}.{1}[{2}]", genericInstance.Namespace, typeReference.Name, String.Join(",", genericInstance.GenericArguments.Select(p => p.GetStandardTypeName()).ToArray()));
            }
            else
            {
                return typeReference.FullName.Replace('/', '+');
            }
        }

        internal static bool SameTypeAs(this TypeReference type1, TypeReference type2)
        {
            if (type1.FullName != type2.FullName) return false;

            string type1Assembly = null;
            if (type1.Scope is ModuleDefinition)
                type1Assembly = (type1.Scope as ModuleDefinition).Assembly.Name.FullName;
            else if (type1.Scope is AssemblyNameReference)
                type1Assembly = (type1.Scope as AssemblyNameReference).FullName;

            string type2Assembly = null;
            if (type2.Scope is ModuleDefinition)
                type2Assembly = (type2.Scope as ModuleDefinition).Assembly.Name.FullName;
            else if (type2.Scope is AssemblyNameReference)
                type2Assembly = (type2.Scope as AssemblyNameReference).FullName;

            if (type1Assembly == null || type2Assembly == null || type1Assembly != type2Assembly) return false;

            return true;
        }

        internal static bool SameTypeAs(this TypeReference type1, Type type2)
        {
            if (type1.GetSimpleTypeName() != type2.GetSimpleTypeName())
                return false;

            string type1Assembly = null;
            if (type1.Scope is ModuleDefinition)
                type1Assembly = (type1.Scope as ModuleDefinition).Assembly.Name.FullName;
            else if (type1.Scope is AssemblyNameReference)
                type1Assembly = (type1.Scope as AssemblyNameReference).FullName;

            if (type1Assembly == null || type1Assembly != type2.Assembly.FullName)
                return false;

            if (type1.IsByReference && !type2.IsByRef)
                return false;

            if (!type1.IsByReference && type2.IsByRef)
                return false;

            if (type1.IsGenericInstance && !type2.IsGenericType)
                return false;

            if (!type1.IsGenericInstance && type2.IsGenericType)
                return false;

            if (type1.IsGenericParameter && !type2.IsGenericParameter)
                return false;

            if (!type1.IsGenericParameter && type2.IsGenericParameter)
                return false;

            if (type1.IsGenericInstance)
            {
                var genericType1 = type1 as GenericInstanceType;
                var type1Args = genericType1.GenericArguments;
                var type2Args = type2.GetGenericArguments();

                if (type1Args.Count != type2Args.Length)
                    return false;

                for (int i = 0; i < type1Args.Count; i++)
                {
                    var type1Arg = type1Args[i];
                    var type2Arg = type2Args[i];

                    if (!type1Arg.SameTypeAs(type2Arg)) return false;
                }
            }

            return true;
        }

        private static bool CompareParameters(this MethodBase method, MethodDefinition methodDefinition)
        {
            var parameters = method.GetParameters();
            var definitionParameters = methodDefinition.Parameters;

            if (parameters.Length != definitionParameters.Count) return false;

            for (int i = 0; i < parameters.Length; i++)
            {
                var parameter = parameters[i];
                var definitionParameter = definitionParameters[i];

                if (parameter.Name != definitionParameter.Name) return false;
                if (!definitionParameter.ParameterType.SameTypeAs(parameter.ParameterType)) return false;
            }

            if (method.IsGenericMethod)
            {
                var genericParameters = method.GetGenericArguments();
                var definitionGenericParameters = methodDefinition.GenericParameters;

                if (genericParameters.Length != definitionGenericParameters.Count) return false;

                for (int i = 0; i < genericParameters.Length; i++)
                {
                    var genericParameterType = genericParameters[i];
                    var definitionGenericParameter = definitionGenericParameters[i];

                    if (!definitionGenericParameter.SameTypeAs(genericParameterType)) return false;
                }
            }

            return true;
        }
    }
}
