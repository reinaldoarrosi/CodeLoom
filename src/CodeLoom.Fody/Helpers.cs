using Mono.Cecil;
using Mono.Cecil.Cil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace CodeLoom.Fody
{
    public static class Helpers
    {
        public static string GetUniqueFieldName(TypeDefinition typeDefinition, string baseName)
        {
            var partialName = $"<{baseName}>_field_";
            var index = typeDefinition.Fields.Where(f => f.Name.StartsWith(partialName)).Count();
            return partialName + index;
        }

        public static string GetUniquePropertyName(TypeDefinition typeDefinition, string baseName)
        {
            var partialName = $"<{baseName}>_prop_";
            var index = typeDefinition.Properties.Where(f => f.Name.StartsWith(partialName)).Count();
            return partialName + index;
        }

        public static string GetUniqueMethodName(TypeDefinition typeDefinition, string baseName)
        {
            var partialName = $"<{baseName}>_method_";
            var index = typeDefinition.Methods.Where(f => f.Name.StartsWith(partialName)).Count();
            return partialName + index;
        }

        public static void Append(this ILProcessor ilProcessor, IEnumerable<Instruction> instructions)
        {
            foreach (var instruction in instructions)
            {
                ilProcessor.Append(instruction);
            }
        }

        public static void InsertBefore(this ILProcessor ilProcessor, Instruction target, IEnumerable<Instruction> instructions)
        {
            foreach (var instruction in instructions)
            {
                ilProcessor.InsertBefore(target, instruction);
            }
        }

        public static void InsertAfter(this ILProcessor ilProcessor, Instruction target, IEnumerable<Instruction> instructions)
        {
            foreach (var instruction in instructions.Reverse())
            {
                ilProcessor.InsertAfter(target, instruction);
            }
        }

        public static TypeReference TryGetClosedGenericType(this TypeReference type, MethodReference closedGenericMethod, ModuleDefinition moduleDefinition)
        {
            try
            {
                return GetClosedGenericType(type, closedGenericMethod, moduleDefinition);
            }
            catch
            {
                return null;
            }
        }

        public static TypeReference GetClosedGenericType(this TypeReference type, MethodReference closedGenericMethod, ModuleDefinition moduleDefinition)
        {
            if (type.IsByReference)
            {
                var refType = (type as ByReferenceType).ElementType.GetClosedGenericType(closedGenericMethod, moduleDefinition);
                var newType = new ByReferenceType(refType);

                return newType;
            }

            if (type.IsGenericInstance)
            {
                var currentGenericType = type as GenericInstanceType;
                var newGenericType = new GenericInstanceType(currentGenericType.ElementType.GetClosedGenericType(closedGenericMethod, moduleDefinition));

                foreach (var p in currentGenericType.GenericArguments)
                {
                    newGenericType.GenericArguments.Add(p.GetClosedGenericType(closedGenericMethod, moduleDefinition));
                }

                return newGenericType;
            }

            if (type.IsGenericParameter)
            {
                var genericParameter = closedGenericMethod.GenericParameters.FirstOrDefault(t => t.FullName == type.FullName);

                if (genericParameter == null)
                    type = type.GetClosedGenericType(closedGenericMethod.DeclaringType, moduleDefinition);
            }

            if (!type.IsGenericParameter)
            {
                type = moduleDefinition.ImportReference(type);
            }

            return type;
        }

        public static TypeReference TryGetClosedGenericType(this TypeReference type, TypeReference closedGenericType, ModuleDefinition moduleDefinition)
        {
            try
            {
                return GetClosedGenericType(type, closedGenericType, moduleDefinition);
            }
            catch
            {
                return null;
            }
        }

        public static TypeReference GetClosedGenericType(this TypeReference type, TypeReference closedGenericType, ModuleDefinition moduleDefinition)
        {
            if (type.IsByReference)
            {
                var refType = (type as ByReferenceType).ElementType.GetClosedGenericType(closedGenericType, moduleDefinition);
                var newType = new ByReferenceType(refType);

                return newType;
            }

            if (type.IsGenericInstance)
            {
                var currentGenericType = type as GenericInstanceType;
                var newGenericType = new GenericInstanceType(currentGenericType.ElementType.GetClosedGenericType(closedGenericType, moduleDefinition));

                foreach (var p in currentGenericType.GenericArguments)
                {
                    newGenericType.GenericArguments.Add(p.GetClosedGenericType(closedGenericType, moduleDefinition));
                }

                return newGenericType;
            }

            if (type.IsGenericParameter)
            {
                if (!(closedGenericType is GenericInstanceType))
                    throw new ArgumentException($"{nameof(closedGenericType)} must be an instance of {typeof(GenericInstanceType).FullName}");

                var openGenericType = (closedGenericType as GenericInstanceType).ElementType;

                if (openGenericType.GenericParameters.Count <= 0)
                    throw new ArgumentException($"{closedGenericType.FullName} does not contain any generic parameters");

                var genericParameter = openGenericType.GenericParameters.FirstOrDefault(t => t.FullName == type.FullName);

                if (genericParameter == null)
                    throw new KeyNotFoundException($"Could not find generic parameter {type.FullName} in {closedGenericType.FullName}");

                type = (closedGenericType as GenericInstanceType).GenericArguments[genericParameter.Position];
            }

            if (!type.IsGenericParameter)
            {
                type = moduleDefinition.ImportReference(type);
            }

            return type;
        }

        public static Type GetSystemType(this TypeDefinition typeDefinition)
        {
            var type = TryGetSystemType(typeDefinition);

            if (type == null)
                throw new KeyNotFoundException($"Could not find Type {typeDefinition.FullName}");

            return type;
        }

        public static Type TryGetSystemType(this TypeDefinition typeDefinition)
        {
            var typeName = $"{typeDefinition.GetStandardTypeName()}, {typeDefinition.Module.Assembly.FullName}";
            var type = Type.GetType(typeName);

            return type;
        }

        public static FieldInfo GetFieldInfo(this FieldDefinition fieldDefinition)
        {
            var fieldInfo = TryGetFieldInfo(fieldDefinition);

            if (fieldInfo == null)
                throw new KeyNotFoundException($"Could not find FieldInfo for field {fieldDefinition.FullName}");

            return fieldInfo;
        }

        public static FieldInfo TryGetFieldInfo(this FieldDefinition fieldDefinition)
        {
            var bindingFlags = BindingFlags.Public | BindingFlags.NonPublic;
            bindingFlags |= fieldDefinition.IsStatic ? BindingFlags.Static : BindingFlags.Instance;

            var declaringType = fieldDefinition.DeclaringType.GetSystemType();
            var fieldInfo = declaringType.GetField(fieldDefinition.Name, bindingFlags);

            return fieldInfo;
        }

        public static PropertyInfo GetPropertyInfo(this PropertyDefinition propertyDefinition)
        {
            var propertyInfo = TryGetPropertyInfo(propertyDefinition);

            if (propertyInfo == null)
                throw new KeyNotFoundException($"Could not find PropertyInfo for property {propertyDefinition.FullName}");

            return propertyInfo;
        }

        public static PropertyInfo TryGetPropertyInfo(this PropertyDefinition propertyDefinition)
        {
            PropertyInfo propertyInfo = null;

            var bindingFlags = BindingFlags.Public | BindingFlags.NonPublic;
            bindingFlags |= !propertyDefinition.HasThis ? BindingFlags.Static : BindingFlags.Instance;

            var declaringType = propertyDefinition.DeclaringType.GetSystemType();

            foreach (var property in declaringType.GetProperties(bindingFlags))
            {
                if (property.Name != propertyDefinition.Name)
                    continue;

                var propertyParameters = property.GetMethod.GetParameters();
                if (propertyParameters.Length != propertyDefinition.Parameters.Count)
                    continue;

                var definitionReturnType = propertyDefinition.PropertyType;
                var propertyReturnType = property.PropertyType;
                if (!definitionReturnType.SameTypeAs(propertyReturnType))
                    continue;

                for (int i = 0; i < propertyDefinition.Parameters.Count; i++)
                {
                    var definitionParameterType = propertyDefinition.Parameters[i].ParameterType;
                    var propertyParameterType = propertyParameters[i].ParameterType;
                    if (!definitionParameterType.SameTypeAs(propertyParameterType))
                        continue;
                }

                propertyInfo = property;
                break;
            }

            return propertyInfo;
        }

        public static MethodBase GetMethodBase(this MethodDefinition methodDefinition)
        {
            var methodInfo = TryGetMethodBase(methodDefinition);

            if (methodInfo == null)
                throw new KeyNotFoundException($"Could not find MethodInfo for method {methodDefinition.FullName}");

            return methodInfo;
        }

        public static MethodBase TryGetMethodBase(this MethodDefinition methodDefinition)
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

        public static string GetSimpleTypeName(this TypeReference typeReference)
        {
            if (typeReference.IsByReference)
            {
                var byRefType = typeReference as ByReferenceType;
                return byRefType.ElementType.GetSimpleTypeName() + "&";
            }

            StringBuilder sb = new StringBuilder();

            if (typeReference.IsGenericParameter)
            {
                var genericParameter = typeReference as GenericParameter;

                if (!string.IsNullOrWhiteSpace(genericParameter.DeclaringMethod?.DeclaringType?.Namespace))
                    sb.AppendFormat("{0}.", genericParameter.DeclaringMethod.DeclaringType.Namespace);
                else if (!string.IsNullOrWhiteSpace(genericParameter.DeclaringType?.Namespace))
                    sb.AppendFormat("{0}.", genericParameter.DeclaringType.Namespace);
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

        public static string GetSimpleTypeName(this Type type)
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

        public static string GetStandardTypeName(this TypeReference typeReference)
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

        public static bool SameTypeAs(this TypeReference type1, TypeReference type2)
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

        public static bool SameTypeAs(this TypeReference type1, Type type2)
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
