using Mono.Cecil;
using Mono.Cecil.Cil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace CodeLoom.Fody
{
    public static class Extensions
    {
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

        public static TypeReference GetClosedGenericType(this TypeReference type, MethodReference closedGenericMethod, ModuleDefinition moduleDefinition)
        {
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

        public static TypeReference GetClosedGenericType(this TypeReference type, TypeReference closedGenericType, ModuleDefinition moduleDefinition)
        {
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

        public static Type TryGetSystemType(this TypeDefinition typeDefinition)
        {
            var typeName = $"{typeDefinition.GetStandardTypeName()}, {typeDefinition.Module.Assembly.FullName}";
            var type = Type.GetType(typeName);

            return type;
        }

        public static Type GetSystemType(this TypeDefinition typeDefinition)
        {
            var type = TryGetSystemType(typeDefinition);

            if (type == null)
                throw new KeyNotFoundException($"Could not find Type {typeDefinition.FullName}");

            return type;
        }

        public static FieldInfo GetFieldInfo(this FieldDefinition fieldDefinition)
        {
            var bindingFlags = BindingFlags.Public | BindingFlags.NonPublic;
            bindingFlags |= fieldDefinition.IsStatic ? BindingFlags.Static : BindingFlags.Instance;

            var declaringType = fieldDefinition.DeclaringType.GetSystemType();
            var fieldInfo = declaringType.GetField(fieldDefinition.Name, bindingFlags);

            if (fieldInfo == null)
                throw new KeyNotFoundException($"Could not find FieldInfo for field {fieldDefinition.FullName}");

            return fieldInfo;
        }

        public static PropertyInfo GetPropertyInfo(this PropertyDefinition propertyDefinition)
        {
            var bindingFlags = BindingFlags.Public | BindingFlags.NonPublic;
            bindingFlags |= !propertyDefinition.HasThis ? BindingFlags.Static : BindingFlags.Instance;

            var declaringType = propertyDefinition.DeclaringType.GetSystemType();
            var propertyInfo = declaringType.GetProperty(propertyDefinition.Name, bindingFlags);

            if (propertyInfo == null)
                throw new KeyNotFoundException($"Could not find PropertyInfo for property {propertyDefinition.FullName}");

            return propertyInfo;
        }

        public static MethodBase GetMethodBase(this MethodDefinition methodDefinition)
        {
            MethodBase result = null;

            var bindingFlags = BindingFlags.Public | BindingFlags.NonPublic;
            bindingFlags |= methodDefinition.IsStatic ? BindingFlags.Static : BindingFlags.Instance;

            var declaringType = methodDefinition.DeclaringType.GetSystemType();

            if (methodDefinition.IsConstructor)
            {
                foreach (var ctor in declaringType.GetConstructors(bindingFlags))
                {
                    if (!ctor.CompareParameters(methodDefinition))
                        continue;

                    result = ctor;
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

                    result = method;
                    break;
                }
            }

            if (result == null)
                throw new KeyNotFoundException($"Could not find MethodInfo for method {methodDefinition.FullName}");

            return result;
        }

        public static string GetSimpleTypeName(this TypeReference typeReference)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("{0}.{1}", typeReference.Namespace, typeReference.Name);

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
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("{0}.{1}", type.Namespace, type.Name);

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

        private static bool CompareParameters(this MethodBase method, MethodDefinition methodDefinition)
        {
            var parameters = method.GetParameters();
            var definitionParameters = methodDefinition.Parameters;

            if (parameters.Length != definitionParameters.Count) return false;

            for (int i = 0; i < parameters.Length; i++)
            {
                var parameter = parameters[i];
                var definitionParameter = definitionParameters[i];
                var parameterTypeRef = methodDefinition.Module.ImportReference(parameter.ParameterType);

                if (parameter.Name != definitionParameter.Name) return false;
                if (!parameterTypeRef.SameTypeAs(definitionParameter.ParameterType)) return false;
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
                    var genericParameterTypeRef = methodDefinition.Module.ImportReference(genericParameterType);

                    if (!genericParameterTypeRef.SameTypeAs(definitionGenericParameter)) return false;
                }
            }

            return true;
        }
    }
}
