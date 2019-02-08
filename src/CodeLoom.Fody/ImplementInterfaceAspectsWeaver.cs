using CodeLoom.Aspects;
using CodeLoom.Helpers;
using CodeLoom.Fody.Helpers;
using Mono.Cecil;
using System;
using System.Collections.Generic;
using System.Linq;
using Mono.Cecil.Cil;
using Mono.Collections.Generic;
using static CodeLoom.Fody.Helpers.Helpers;

namespace CodeLoom.Fody
{
    internal class ImplementInterfaceAspectsWeaver
    {
        private readonly ModuleWeaver ModuleWeaver;
        private readonly Dictionary<Type, bool> WeavedTypes;

        internal ImplementInterfaceAspectsWeaver(ModuleWeaver moduleWeaver)
        {
            ModuleWeaver = moduleWeaver;
            WeavedTypes = new Dictionary<Type, bool>();
        }

        internal ModuleDefinition ModuleDefinition { get { return ModuleWeaver.ModuleDefinition; } }

        internal void Weave(TypeDefinition typeDefinition)
        {
            var type = typeDefinition.TryGetSystemType();
            if (type == null)
            {
                ModuleWeaver.LogInfo($"Type {typeDefinition.FullName} will not be weaved because it was not possible to load its corresponding System.Type");
                return;
            }

            if (type.IsInterface)
            {
                ModuleWeaver.LogInfo($"Type {typeDefinition.FullName} will not be weaved because it is an interface");
                return;
            }

            if (WeavedTypes.ContainsKey(type))
            {
                // if the type we're trying to weave was already weaved we'll skip weaving it
                // all aspects are applied the first time the type is weaved, there's no need to weave it more than once
                return;
            }
            else
            {
                // mark this method so that it won't be weaved again
                WeavedTypes.Add(type, true);
            }

            ApplyImplementInterfaceAspects(typeDefinition, type);
        }

        private void ApplyImplementInterfaceAspects(TypeDefinition typeDefinition, Type type)
        {
            var aspects = ModuleWeaver.Setup.GetImplementInterfaceAspects(type).ToArray();
            if (aspects.Length <= 0)
            {
                ModuleWeaver.LogInfo($"Type {type.FullName} will not be weaved because no aspect was applied to it");
                return;
            }

            var implementedInterfaces = new Dictionary<Type, bool>();
            var aspectsAndFieldsMap = InitializeAspectsFields(typeDefinition, aspects);

            foreach (var entry in aspectsAndFieldsMap)
            {
                var interfaces = entry.Key.GetType()
                    .GetInterfaces()
                    .ToArray();

                foreach (var interfaceType in interfaces)
                {
                    if (implementedInterfaces.ContainsKey(interfaceType))
                        continue;

                    implementedInterfaces.Add(interfaceType, true);
                    ImplementInterface(typeDefinition, interfaceType, entry.Key, entry.Value);
                }
            }
        }

        private Dictionary<IImplementInterfaceAspect, FieldDefinition> InitializeAspectsFields(TypeDefinition typeDefinition, IImplementInterfaceAspect[] aspects)
        {
            // creates the private fields that will hold the aspects instances
            Dictionary<IImplementInterfaceAspect, FieldDefinition> aspectsAndFieldsMap = new Dictionary<IImplementInterfaceAspect, FieldDefinition>();
            foreach (var aspect in aspects)
            {
                var aspectTypeRef = ModuleWeaver.ModuleDefinition.ImportReference(aspect.GetType());
                var field = new FieldDefinition(GetUniqueFieldName(typeDefinition, aspectTypeRef.Name), FieldAttributes.Private, aspectTypeRef);
                aspectsAndFieldsMap.Add(aspect, field);
                typeDefinition.Fields.Add(field);
            }

            // creates a private field that will be used to check if "InitializeAspects" was already called
            var initAspectsCalledFieldName = GetUniqueFieldName(typeDefinition, "InitializeAspectsCalled");
            var initAspectsCalledField = new FieldDefinition(initAspectsCalledFieldName, FieldAttributes.Private, ModuleWeaver.TypeSystem.BooleanReference);
            typeDefinition.Fields.Add(initAspectsCalledField);

            // creates the "InitializeAspects" method, that will instantiate all aspects and store them into the previously created private fields
            var initAspectsMethodName = GetUniqueMethodName(typeDefinition, $"InitializeAspects");
            var initAspectsMethod = new MethodDefinition(initAspectsMethodName, MethodAttributes.Private, ModuleWeaver.TypeSystem.VoidReference);
            var retInstruction = Instruction.Create(OpCodes.Ret);
            var ilProcessor = initAspectsMethod.Body.GetILProcessor();
            initAspectsMethod.Body.InitLocals = true;
            initAspectsMethod.HasThis = true;
            typeDefinition.Methods.Add(initAspectsMethod);
            ilProcessor.Append(Instruction.Create(OpCodes.Ldarg_0));
            ilProcessor.Append(Instruction.Create(OpCodes.Ldfld, initAspectsCalledField));
            ilProcessor.Append(Instruction.Create(OpCodes.Brtrue, retInstruction));
            ilProcessor.Append(Instruction.Create(OpCodes.Ldarg_0));
            ilProcessor.Append(Instruction.Create(OpCodes.Ldc_I4_1));
            ilProcessor.Append(Instruction.Create(OpCodes.Stfld, initAspectsCalledField));
            foreach (var entry in aspectsAndFieldsMap)
            {
                var ctorWithParam = entry.Key.GetType().GetConstructor(new[] { typeof(object) }); // try to get the aspect ctor that accepts a single parameter
                var ctorWithoutParam = ctorWithParam == null ? entry.Key.GetType().GetConstructor(Type.EmptyTypes) : null; // if the ctor above was not found, get the ctor that has no parameters
                var aspectCtorRef = ModuleDefinition.ImportReference(ctorWithParam ?? ctorWithoutParam);

                ilProcessor.Append(Instruction.Create(OpCodes.Ldarg_0)); // loads "this" into ths stack so that we can use Stfld later
                if (ctorWithParam != null) ilProcessor.Append(Instruction.Create(OpCodes.Ldarg_0)); // if the aspect ctor accepts a single parameter loads "this" into the stack to serve as the parameter value
                ilProcessor.Append(Instruction.Create(OpCodes.Newobj, aspectCtorRef)); // invokes the aspect ctor
                ilProcessor.Append(Instruction.Create(OpCodes.Stfld, entry.Value)); // stores the aspect instance into the private field
            }
            ilProcessor.Append(retInstruction);


            // for each constructor, add a call to InitializeAspects
            var callInitAspectsInstructions = new List<Instruction>() { Instruction.Create(OpCodes.Ldarg_0), Instruction.Create(OpCodes.Callvirt, initAspectsMethod) };
            foreach (var ctor in typeDefinition.Methods.Where(m => m.IsConstructor && !m.IsStatic))
            {
                // skips until after the first OpCodes.Call, because it represents the call to the base ctor, which must be held in place
                var insertionPoint = ctor.Body.Instructions.SkipWhile(i => i.OpCode != OpCodes.Call).Skip(1).Take(1).First();

                // inserts the call to "InitializeAspects"
                var instructions = ctor.Body.GetILProcessor();
                instructions.InsertBefore(insertionPoint, callInitAspectsInstructions);
            }

            return aspectsAndFieldsMap;
        }

        private void ImplementInterface(TypeDefinition typeDefinition, Type interfaceType, IImplementInterfaceAspect aspect, FieldDefinition aspectField)
        {
            // Ignores the "IImplementInterfaceAspect" interface that serves only as a marker interface
            if (interfaceType == typeof(IImplementInterfaceAspect))
                return;

            // Ignores the "_Attribute" interface that is automatically implemented by all attributes
            if (interfaceType.FullName == "System.Runtime.InteropServices._Attribute")
                return;

            ModuleWeaver.LogInfo($"Implementing interface {interfaceType.FullName} on {typeDefinition.FullName}");

            var implementationInfo = GetImplementationInfo(aspect.GetType(), interfaceType);
            typeDefinition.Interfaces.Add(new InterfaceImplementation(ModuleDefinition.ImportReference(interfaceType)));

            foreach (var item in implementationInfo)
            {
                if (item.Type == ImplementationInfo.Types.Method)
                    ImplementInterfaceMethod(typeDefinition, interfaceType, aspect, aspectField, item as ImplementationInfo.Method);
                else if (item.Type == ImplementationInfo.Types.Property)
                    ImplementInterfaceProperty(typeDefinition, interfaceType, aspect, aspectField, item as ImplementationInfo.Property);
            }
        }

        private MethodDefinition ImplementInterfaceMethod(TypeDefinition typeDefinition, Type interfaceType, IImplementInterfaceAspect aspect, FieldDefinition aspectField, ImplementationInfo.Method methodImplementationInfo)
        {
            var interfaceMethod = methodImplementationInfo.InterfaceMethod;
            var aspectMethod = methodImplementationInfo.AspectMethod;
            var interfaceMethodRef = ModuleDefinition.ImportReference(interfaceMethod);
            var aspectMethodDef = ModuleDefinition.ImportReference(aspectMethod).Resolve();

            ModuleWeaver.LogInfo($"Implementing method {aspectMethod.Name} in {typeDefinition.FullName}");

            // Creates a new method that will proxy to the "original" method.
            // The interfaces are always explicitly implemented, so that we don't have problems with names collisions
            var newMethodAttrs = Mono.Cecil.MethodAttributes.Private |
                    Mono.Cecil.MethodAttributes.HideBySig |
                    Mono.Cecil.MethodAttributes.NewSlot |
                    Mono.Cecil.MethodAttributes.Virtual |
                    Mono.Cecil.MethodAttributes.Final;
            var newMethodName = $"{interfaceType.Namespace}.{interfaceType.Name}.{interfaceMethod.Name}";// GetUniqueMethodName(typeDefinition, aspectMethod.Name);
            var newMethodDefinition = new MethodDefinition(newMethodName, newMethodAttrs, ModuleWeaver.TypeSystem.VoidReference);
            typeDefinition.Methods.Add(newMethodDefinition);
            newMethodDefinition.Overrides.Add(interfaceMethodRef); // This is what makes the implementation "explicit"
            newMethodDefinition.CallingConvention = aspectMethodDef.CallingConvention;
            newMethodDefinition.ExplicitThis = aspectMethodDef.ExplicitThis;
            newMethodDefinition.HasThis = aspectMethodDef.HasThis;
            newMethodDefinition.CopyGenericParameters(aspectMethodDef.GenericParameters);
            newMethodDefinition.ReturnType = ResolveTypeReference(aspectMethod.ReturnType, newMethodDefinition.GenericParameters);
            foreach (var parameter in aspectMethod.GetParameters())
            {
                var newParameter = new ParameterDefinition(parameter.Name, ParameterAttributes.None, ResolveTypeReference(parameter.ParameterType, newMethodDefinition.GenericParameters));
                newParameter.IsIn = parameter.IsIn;
                newParameter.IsLcid = parameter.IsLcid;
                newParameter.IsOptional = parameter.IsOptional;
                newParameter.IsOut = parameter.IsOut;
                newParameter.IsReturnValue = parameter.IsRetval;

                // respect parameters with "params" modifier
                if (parameter.CustomAttributes.Any(attr => attr.AttributeType == typeof(ParamArrayAttribute)))
                {
                    var ctor = typeof(ParamArrayAttribute).GetConstructor(Type.EmptyTypes);
                    var attr = new CustomAttribute(ModuleDefinition.ImportReference(ctor));
                    newParameter.CustomAttributes.Add(attr);
                }

                newMethodDefinition.Parameters.Add(newParameter);
            }

            // Writes the "code" of the new method
            // This will simply retrieve the aspect instance from the private field created in "InitializeAspectsFields"
            // and then call the corresponding method on that instance
            var variableObj = new VariableDefinition(ModuleDefinition.ImportReference(interfaceType));
            newMethodDefinition.Body.InitLocals = true;
            newMethodDefinition.Body.Instructions.Add(Instruction.Create(OpCodes.Ldarg_0));
            newMethodDefinition.Body.Instructions.Add(Instruction.Create(OpCodes.Ldfld, aspectField));
            newMethodDefinition.Body.Instructions.Add(Instruction.Create(OpCodes.Castclass, variableObj.VariableType));

            foreach (var p in newMethodDefinition.Parameters)
            {
                newMethodDefinition.Body.Instructions.Add(Instruction.Create(OpCodes.Ldarg, p));
            }

            if (interfaceMethodRef.HasGenericParameters)
                interfaceMethodRef = interfaceMethodRef.MakeGenericInstanceMethod(newMethodDefinition.GenericParameters.ToArray());

            newMethodDefinition.Body.Instructions.Add(Instruction.Create(OpCodes.Callvirt, interfaceMethodRef));
            newMethodDefinition.Body.Instructions.Add(Instruction.Create(OpCodes.Ret));

            return newMethodDefinition;
        }

        private PropertyDefinition ImplementInterfaceProperty(TypeDefinition typeDefinition, Type interfaceType, IImplementInterfaceAspect aspect, FieldDefinition aspectField, ImplementationInfo.Property propertyImplementationInfo)
        {
            var propertyInfo = propertyImplementationInfo.PropertyInfo;
            var interfaceGetter = propertyImplementationInfo.InterfaceGetter;
            var interfaceSetter = propertyImplementationInfo.InterfaceSetter;
            var aspectGetter = propertyImplementationInfo.AspectGetter;
            var aspectSetter = propertyImplementationInfo.AspectSetter;

            var newPropertyDefinition = new PropertyDefinition(propertyInfo.Name, PropertyAttributes.None, ModuleDefinition.ImportReference(propertyInfo.PropertyType));
            newPropertyDefinition.IsSpecialName = (propertyInfo.Attributes & System.Reflection.PropertyAttributes.SpecialName) == System.Reflection.PropertyAttributes.SpecialName;
            newPropertyDefinition.IsRuntimeSpecialName = (propertyInfo.Attributes & System.Reflection.PropertyAttributes.RTSpecialName) == System.Reflection.PropertyAttributes.RTSpecialName;
            newPropertyDefinition.HasDefault = (propertyInfo.Attributes & System.Reflection.PropertyAttributes.HasDefault) == System.Reflection.PropertyAttributes.HasDefault;

            if (aspectGetter != null)
            {
                var getter = ImplementInterfaceMethod(typeDefinition, interfaceType, aspect, aspectField, new ImplementationInfo.Method(interfaceGetter, aspectGetter));
                newPropertyDefinition.GetMethod = getter;
            }

            if (aspectSetter != null)
            {
                var setter = ImplementInterfaceMethod(typeDefinition, interfaceType, aspect, aspectField, new ImplementationInfo.Method(interfaceSetter, aspectSetter));
                newPropertyDefinition.SetMethod = setter;
            }

            typeDefinition.Properties.Add(newPropertyDefinition);
            return newPropertyDefinition;
        }
        
        private TypeReference ResolveTypeReference(Type type, Collection<GenericParameter> genericParameters)
        {
            if (type.IsArray)
            {
                var elementType = ResolveTypeReference(type.GetElementType(), genericParameters);
                return new ArrayType(elementType, type.GetArrayRank());
            }
            else if (type.IsByRef)
            {
                var elementType = ResolveTypeReference(type.GetElementType(), genericParameters);
                return new ByReferenceType(elementType);
            }
            else if (type.IsGenericType)
            {
                var openType = ModuleDefinition.ImportReference(type.GetGenericTypeDefinition());
                var genericArgs = type.GetGenericArguments().Select(t => ResolveTypeReference(t, genericParameters)).ToArray();
                return openType.MakeGenericInstanceType(genericArgs);
            }
            else if (type.IsGenericParameter)
            {
                return genericParameters.LastOrDefault(g => g.Name == type.Name);
            }
            else
            {
                return ModuleDefinition.ImportReference(type);
            }
        }
    }
}

