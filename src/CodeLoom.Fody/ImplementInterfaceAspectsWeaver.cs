﻿using CodeLoom.Aspects;
using Mono.Cecil;
using Mono.Cecil.Cil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace CodeLoom.Fody
{
    class ImplementInterfaceAspectsWeaver
    {
        internal ModuleWeaver ModuleWeaver;

        public ImplementInterfaceAspectsWeaver(ModuleWeaver moduleWeaver)
        {
            ModuleWeaver = moduleWeaver;
        }

        internal ModuleDefinition ModuleDefinition { get { return ModuleWeaver.ModuleDefinition; } }

        public void Weave(TypeDefinition typeDefinition)
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

            var typeAspects = ModuleWeaver.Setup.GetAspects(type);
            foreach (var aspect in typeAspects)
            {
                try
                {
                    ModuleWeaver.LogInfo($"Weaving aspect {aspect.GetType().FullName} into {typeDefinition.FullName}");
                    ApplyAspect(typeDefinition, type, aspect);
                    ModuleWeaver.LogInfo($"Weaving OK");
                }
                catch (Exception e)
                {
                    ModuleWeaver.LogError($"Error while weaving type {type.FullName}: {e.ToString()}");
                    return;
                }
            }
        }

        private void ApplyAspect(TypeDefinition typeDefinition, Type type, ImplementInterfaceAspect aspect)
        {
            var aspectInstanceField = CreateAspectField(typeDefinition, aspect);
            typeDefinition.Fields.Add(aspectInstanceField);

            var implemented = ImplementInterfaces(typeDefinition, aspectInstanceField, aspect);
            if (!implemented) typeDefinition.Fields.Remove(aspectInstanceField);
        }

        private FieldDefinition CreateAspectField(TypeDefinition typeDefinition, ImplementInterfaceAspect aspect)
        {
            ModuleWeaver.LogInfo($"Creating private field for aspect {aspect.GetType().FullName} in {typeDefinition.FullName}");

            // Import the ctor reference but changes the declaring type so that the ctor 
            // reference points to a closed generic type (in case the interface is generic)
            var aspectTypeRef = ModuleDefinition.ImportReference(aspect.GetType());
            var aspectTypeDef = aspectTypeRef.Resolve();
            var aspectCtor = ModuleDefinition.ImportReference(aspectTypeDef.Methods.FirstOrDefault(m => m.IsConstructor && !m.IsStatic && !m.HasParameters));
            aspectCtor.DeclaringType = aspectTypeRef;

            // Creates a private field which will contain an instance of the aspect
            // This instance will then be used to implement the interfaces
            // IOW, the type that is being weaved will work as a proxy to this instance
            var aspectFieldName = Helpers.GetUniqueFieldName(typeDefinition, aspect.GetType().Name);
            var fieldDefinition = new FieldDefinition(aspectFieldName, Mono.Cecil.FieldAttributes.Private, aspectTypeRef);

            // Adds the field initialization code to every ctor present in the type that is being weaved
            // This is equivalent to initializing a field directly in its declaration e.g. "private MyAspect _aspect = new MyAspect();"
            foreach (var ctor in typeDefinition.Methods.Where(m => m.IsConstructor && !m.IsStatic))
            {
                var firstInstruction = ctor.Body.Instructions.First();
                var ilProcessor = ctor.Body.GetILProcessor();

                ilProcessor.InsertBefore(firstInstruction, new[]
                {
                    Instruction.Create(OpCodes.Ldarg_0),
                    Instruction.Create(OpCodes.Newobj, aspectCtor),
                    Instruction.Create(OpCodes.Stfld, fieldDefinition)
                });
            }

            return fieldDefinition;
        }

        private bool ImplementInterfaces(TypeDefinition typeDefinition, FieldDefinition aspectInstanceField, ImplementInterfaceAspect aspect)
        {
            var interfaces = aspect.GetType().GetInterfaces();
            var implemented = false;

            foreach (var interfaceType in interfaces)
            {
                ModuleWeaver.LogInfo($"Implementing interface {interfaceType.FullName} on {typeDefinition.FullName}");

                // Ignores the "_Attribute" interface that is automatically implemented by all attributes
                if (interfaceType.FullName == "System.Runtime.InteropServices._Attribute")
                    continue;

                var interfaceTypeRef = ModuleDefinition.ImportReference(interfaceType);
                if (typeDefinition.Interfaces.Any(i => i.InterfaceType.SameTypeAs(interfaceTypeRef)))
                {
                    ModuleWeaver.LogInfo($"Interface {interfaceType.FullName} will not be added to type {typeDefinition.FullName} because it is already implemented");
                    continue;
                }

                implemented = true;
                typeDefinition.Interfaces.Add(new InterfaceImplementation(interfaceTypeRef));
                ImplementInterfaceProperties(typeDefinition, aspectInstanceField, aspect, interfaceTypeRef);
                ImplementInterfaceMethods(typeDefinition, aspectInstanceField, aspect, interfaceTypeRef);
            }

            return implemented;
        }

        private void ImplementInterfaceProperties(TypeDefinition typeDefinition, FieldDefinition aspectInstanceField, ImplementInterfaceAspect aspect, TypeReference interfaceTypeRef)
        {
            var properties = interfaceTypeRef.Resolve().Properties;

            foreach (var prop in properties)
            {
                ModuleWeaver.LogInfo($"Implementing property {prop.Name} in {typeDefinition.FullName}");

                // For every property that is part of the interface being implemented, 
                // creates a new property with the same signature on the type being weaved
                var propTypeRef = prop.PropertyType.GetClosedGenericType(interfaceTypeRef, ModuleDefinition);
                var propDefinition = new PropertyDefinition(prop.Name, prop.Attributes, propTypeRef)
                {
                    Constant = prop.Constant,
                    HasConstant = prop.HasConstant,
                    HasDefault = prop.HasDefault,
                    HasThis = prop.HasThis,
                    IsRuntimeSpecialName = prop.IsRuntimeSpecialName,
                    IsSpecialName = prop.IsSpecialName
                };
                typeDefinition.Properties.Add(propDefinition);

                if (prop.GetMethod != null)
                {
                    // If the property has a "getter", import the method reference but changes the declaring type
                    // so that the method reference points to a closed generic type (in case the interface is generic)
                    var interfacePropGetMethod = ModuleDefinition.ImportReference(prop.GetMethod);
                    interfacePropGetMethod.DeclaringType = interfaceTypeRef;

                    // Creates a new "getter" method that will proxy to the "original" method.
                    // The interfaces are always explicitly implemented, so that we don't have problems with names collisions
                    var propGetMethodAttrs = Mono.Cecil.MethodAttributes.Private |
                        Mono.Cecil.MethodAttributes.HideBySig |
                        Mono.Cecil.MethodAttributes.NewSlot |
                        Mono.Cecil.MethodAttributes.SpecialName |
                        Mono.Cecil.MethodAttributes.Virtual |
                        Mono.Cecil.MethodAttributes.Final;
                    var propGetMethod = new MethodDefinition($"get_{prop.Name}", propGetMethodAttrs, propTypeRef);
                    propGetMethod.Overrides.Add(interfacePropGetMethod); // This is what makes the implementation "explicit"
                    propDefinition.GetMethod = propGetMethod;
                    typeDefinition.Methods.Add(propGetMethod);
                    foreach (var p in prop.GetMethod.Parameters)
                    {
                        // We also copy the parameters of the original "getter"
                        // Generally "getters" do not have parameters, but if the property is an indexer property, there will be parameters
                        var parameter = new ParameterDefinition(
                            p.Name,
                            p.Attributes,
                            p.ParameterType.GetClosedGenericType(interfaceTypeRef, ModuleDefinition)
                        )
                        {
                            Constant = p.Constant,
                            HasConstant = p.HasConstant,
                            HasDefault = p.HasDefault,
                            HasFieldMarshal = p.HasFieldMarshal,
                            IsIn = p.IsIn,
                            IsLcid = p.IsLcid,
                            IsOptional = p.IsOptional,
                            IsOut = p.IsOut,
                            IsReturnValue = p.IsReturnValue,
                            MarshalInfo = p.MarshalInfo
                        };

                        propGetMethod.Parameters.Add(parameter);
                    }

                    // Writes the "code" of the "getter"
                    // This will simply retrieve the aspect instance from the private field created in "AddAspectField"
                    // and then call the corresponding "getter" on that instance, returning its value
                    var variableObj = new VariableDefinition(interfaceTypeRef);
                    propGetMethod.Body.InitLocals = true;
                    propGetMethod.Body.Variables.Add(variableObj);
                    propGetMethod.Body.Instructions.Add(Instruction.Create(OpCodes.Ldarg_0));
                    propGetMethod.Body.Instructions.Add(Instruction.Create(OpCodes.Ldfld, aspectInstanceField));
                    propGetMethod.Body.Instructions.Add(Instruction.Create(OpCodes.Stloc, variableObj));
                    propGetMethod.Body.Instructions.Add(Instruction.Create(OpCodes.Ldloc, variableObj));
                    foreach (var p in propGetMethod.Parameters)
                    {
                        propGetMethod.Body.Instructions.Add(Instruction.Create(OpCodes.Ldarg, p));
                    }
                    propGetMethod.Body.Instructions.Add(Instruction.Create(OpCodes.Callvirt, interfacePropGetMethod));
                    propGetMethod.Body.Instructions.Add(Instruction.Create(OpCodes.Ret));
                }

                if (prop.SetMethod != null)
                {
                    // If the property has a "setter", import the method reference but changes the declaring type
                    // so that the method reference points to a closed generic type (in case the interface is generic)
                    var interfacePropSetMethod = ModuleDefinition.ImportReference(prop.SetMethod);
                    interfacePropSetMethod.DeclaringType = interfaceTypeRef;

                    // Creates a new "setter" method that will proxy to the "original" method.
                    // The interfaces are always explicitly implemented, so that we don't have problems with names collisions
                    var propSetMethodAttrs = Mono.Cecil.MethodAttributes.Private |
                        Mono.Cecil.MethodAttributes.HideBySig |
                        Mono.Cecil.MethodAttributes.NewSlot |
                        Mono.Cecil.MethodAttributes.SpecialName |
                        Mono.Cecil.MethodAttributes.Virtual |
                        Mono.Cecil.MethodAttributes.Final;
                    var propSetMethod = new MethodDefinition($"set_{prop.Name}", propSetMethodAttrs, ModuleWeaver.TypeSystem.VoidReference);
                    propSetMethod.Overrides.Add(interfacePropSetMethod); // This is what makes the implementation "explicit"
                    propDefinition.SetMethod = propSetMethod;
                    typeDefinition.Methods.Add(propSetMethod);
                    foreach (var p in prop.SetMethod.Parameters)
                    {
                        // We also copy the parameters of the original "setter"
                        // Generally "setters" have a single "value" parameter, but there may be more if the property is an indexer property
                        var parameter = new ParameterDefinition(
                            p.Name,
                            p.Attributes,
                            p.ParameterType.GetClosedGenericType(interfaceTypeRef, ModuleDefinition)
                        )
                        {
                            Constant = p.Constant,
                            HasConstant = p.HasConstant,
                            HasDefault = p.HasDefault,
                            HasFieldMarshal = p.HasFieldMarshal,
                            IsIn = p.IsIn,
                            IsLcid = p.IsLcid,
                            IsOptional = p.IsOptional,
                            IsOut = p.IsOut,
                            IsReturnValue = p.IsReturnValue,
                            MarshalInfo = p.MarshalInfo
                        };

                        propSetMethod.Parameters.Add(parameter);
                    }

                    // Writes the "code" of the "setter"
                    // This will simply retrieve the aspect instance from the private field created in "AddAspectField"
                    // and then call the corresponding "setter" on that instance
                    var variableObj = new VariableDefinition(interfaceTypeRef);
                    propSetMethod.Body.InitLocals = true;
                    propSetMethod.Body.Variables.Add(variableObj);
                    propSetMethod.Body.Instructions.Add(Instruction.Create(OpCodes.Ldarg_0));
                    propSetMethod.Body.Instructions.Add(Instruction.Create(OpCodes.Ldfld, aspectInstanceField));
                    propSetMethod.Body.Instructions.Add(Instruction.Create(OpCodes.Stloc, variableObj));
                    propSetMethod.Body.Instructions.Add(Instruction.Create(OpCodes.Ldloc, variableObj));
                    foreach (var p in propSetMethod.Parameters)
                    {
                        propSetMethod.Body.Instructions.Add(Instruction.Create(OpCodes.Ldarg, p));
                    }
                    propSetMethod.Body.Instructions.Add(Instruction.Create(OpCodes.Callvirt, interfacePropSetMethod));
                    propSetMethod.Body.Instructions.Add(Instruction.Create(OpCodes.Ret));
                }
            }
        }

        private void ImplementInterfaceMethods(TypeDefinition typeDefinition, FieldDefinition aspectInstanceField, ImplementInterfaceAspect aspect, TypeReference interfaceTypeRef)
        {
            var methods = interfaceTypeRef.Resolve().Methods;

            foreach (var method in methods)
            {
                // Ignores methods that are "getters" or "setters" because they are dealt with in "ImplementInterfaceProperties"
                if (method.IsGetter || method.IsSetter)
                    continue;

                ModuleWeaver.LogInfo($"Implementing method {method.Name} in {typeDefinition.FullName}");

                // Import the method reference but changes the declaring type so that the method 
                // reference points to a closed generic type (in case the interface is generic)
                var methodRef = ModuleDefinition.ImportReference(method);
                methodRef.DeclaringType = interfaceTypeRef;

                // Creates a new method that will proxy to the "original" method.
                // The interfaces are always explicitly implemented, so that we don't have problems with names collisions
                var methodAttrs = Mono.Cecil.MethodAttributes.Private |
                        Mono.Cecil.MethodAttributes.HideBySig |
                        Mono.Cecil.MethodAttributes.NewSlot |
                        Mono.Cecil.MethodAttributes.Virtual |
                        Mono.Cecil.MethodAttributes.Final;
                var methodReturnTypeRef = methodRef.ReturnType.GetClosedGenericType(methodRef, ModuleDefinition);
                var methodDefinition = new MethodDefinition($"{method.Name}", methodAttrs, methodReturnTypeRef)
                {
                    AggressiveInlining = method.AggressiveInlining,
                    CallingConvention = method.CallingConvention,
                    ExplicitThis = method.ExplicitThis,
                    HasSecurity = method.HasSecurity,
                    HasThis = method.HasThis,
                };
                methodDefinition.Overrides.Add(methodRef); // This is what makes the implementation "explicit"
                typeDefinition.Methods.Add(methodDefinition);
                foreach (var p in method.Parameters)
                {
                    // Copy all parameters from the original method to the one being created
                    methodDefinition.Parameters.Add(
                        new ParameterDefinition(
                            p.Name,
                            p.Attributes,
                            p.ParameterType.GetClosedGenericType(methodRef, ModuleDefinition)
                        )
                    );
                }
                foreach (var p in method.GenericParameters)
                {
                    // Also copies all generic parameters from the original method to the one being created
                    var genericParameter = new GenericParameter(p.Name, methodDefinition);
                    foreach (var constraint in p.Constraints) { genericParameter.Constraints.Add(ModuleDefinition.ImportReference(constraint)); }

                    methodDefinition.GenericParameters.Add(genericParameter);
                }

                // Writes the "code" of the new method
                // This will simply retrieve the aspect instance from the private field created in "AddAspectField"
                // and then call the corresponding method on that instance
                var variableObj = new VariableDefinition(interfaceTypeRef);
                methodDefinition.Body.InitLocals = true;
                methodDefinition.Body.Variables.Add(variableObj);
                methodDefinition.Body.Instructions.Add(Instruction.Create(OpCodes.Ldarg_0));
                methodDefinition.Body.Instructions.Add(Instruction.Create(OpCodes.Ldfld, aspectInstanceField));
                methodDefinition.Body.Instructions.Add(Instruction.Create(OpCodes.Stloc, variableObj));
                methodDefinition.Body.Instructions.Add(Instruction.Create(OpCodes.Ldloc, variableObj));

                foreach (var p in methodDefinition.Parameters)
                {
                    methodDefinition.Body.Instructions.Add(Instruction.Create(OpCodes.Ldarg, p));
                }

                if (methodRef.GenericParameters.Count > 0)
                {
                    var genericMethodRef = new GenericInstanceMethod(methodRef);
                    foreach (var p in methodRef.GenericParameters) { genericMethodRef.GenericArguments.Add(p); }
                    methodDefinition.Body.Instructions.Add(Instruction.Create(OpCodes.Callvirt, genericMethodRef));
                }
                else
                {
                    methodDefinition.Body.Instructions.Add(Instruction.Create(OpCodes.Callvirt, methodRef));
                }

                methodDefinition.Body.Instructions.Add(Instruction.Create(OpCodes.Ret));
            }
        }
    }
}

