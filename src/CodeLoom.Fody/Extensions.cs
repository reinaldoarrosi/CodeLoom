using Mono.Cecil;
using Mono.Cecil.Cil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

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

        public static bool IsVoidReturnType(this MethodReference method, global::Fody.TypeSystem typeSystem)
        {
            return method.ReturnType.FullName == typeSystem.VoidReference.FullName;
        }

        public static MethodReference MakeGenericMethod(this MethodReference method, TypeReference declaringType)
        {
            var reference = new MethodReference(method.Name, method.ReturnType, declaringType)
            {
                HasThis = method.HasThis,
                ExplicitThis = method.ExplicitThis,
                CallingConvention = method.CallingConvention
            };

            foreach (var parameter in method.Parameters)
            {
                reference.Parameters.Add(new ParameterDefinition(parameter.ParameterType));
            }

            foreach (var genericParam in method.GenericParameters)
            {
                reference.GenericParameters.Add(new GenericParameter(genericParam.Name, reference));
            }

            return reference;
        }

        public static Type GetSystemType(this TypeDefinition typeDefinition)
        {
            var typeName = $"{typeDefinition.GetStandardTypeName()}, {typeDefinition.Module.Assembly.FullName}";
            var type = Type.GetType(typeName);

            return type;
        }

        public static FieldInfo GetFieldInfo(this FieldDefinition fieldDefinition)
        {
            var bindingFlags = BindingFlags.Public | BindingFlags.NonPublic;
            bindingFlags |= fieldDefinition.IsStatic ? BindingFlags.Static : BindingFlags.Instance;

            var declaringType = fieldDefinition.DeclaringType.GetSystemType();
            var fieldInfo = declaringType.GetField(fieldDefinition.Name, bindingFlags);

            return fieldInfo;
        }

        public static PropertyInfo GetPropertyInfo(this PropertyDefinition propertyDefinition)
        {
            var bindingFlags = BindingFlags.Public | BindingFlags.NonPublic;
            bindingFlags |= !propertyDefinition.HasThis ? BindingFlags.Static : BindingFlags.Instance;

            var declaringType = propertyDefinition.DeclaringType.GetSystemType();
            var propertyInfo = declaringType.GetProperty(propertyDefinition.Name, bindingFlags);

            return propertyInfo;
        }

        public static MethodBase GetMethodBase(this MethodDefinition methodDefinition)
        {
            MethodBase result = null;

            var bindingFlags = BindingFlags.Public | BindingFlags.NonPublic;
            bindingFlags |= methodDefinition.IsStatic ? BindingFlags.Static : BindingFlags.Instance;

            var declaringType = methodDefinition.DeclaringType.GetSystemType();

            foreach (var method in declaringType.GetMethods(bindingFlags))
            {
                if (method.Name != methodDefinition.Name)
                    continue;

                if (!method.CompareParameters(methodDefinition))
                    continue;

                result = method;
                break;
            }

            return result;
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
                if (parameter.ParameterType.IsGenericParameter != definitionParameter.ParameterType.IsGenericParameter) return false;
                if (parameter.ParameterType.FullName != definitionParameter.ParameterType.GetStandardTypeName()) return false;
                if (parameter.ParameterType.Assembly.FullName != definitionParameter.ParameterType.Module.Assembly.FullName) return false;
            }

            var genericParameters = method.GetGenericArguments();
            var definitionGenericParameters = methodDefinition.GenericParameters;

            if (genericParameters.Length != definitionGenericParameters.Count) return false;

            for (int i = 0; i < genericParameters.Length; i++)
            {
                var genericParameter = genericParameters[i];
                var definitionGenericParameter = definitionGenericParameters[i];

                if (genericParameter.Name != definitionGenericParameter.Name) return false;
                if (genericParameter.IsGenericParameter != definitionGenericParameter.IsGenericParameter) return false;
                if (genericParameter.FullName != definitionGenericParameter.GetStandardTypeName()) return false;
                if (genericParameter.Assembly.FullName != definitionGenericParameter.Module.Assembly.FullName) return false;
            }

            return true;
        }
    }
}
