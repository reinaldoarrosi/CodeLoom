using CodeLoom.Aspects;
using CodeLoom.Contexts;
using CodeLoom.Bindings;
using Mono.Cecil;
using Mono.Cecil.Cil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Mono.Cecil.Rocks;
using CodeLoom.Helpers;

namespace CodeLoom.Fody
{
    class InterceptMethodAspectsWeaver
    {
        internal ModuleWeaver ModuleWeaver;

        public InterceptMethodAspectsWeaver(ModuleWeaver moduleWeaver)
        {
            ModuleWeaver = moduleWeaver;
            WeavedMethods = new Dictionary<System.Reflection.MethodBase, bool>();
        }

        internal ModuleDefinition ModuleDefinition { get { return ModuleWeaver.ModuleDefinition; } }
        internal Dictionary<System.Reflection.MethodBase, bool> WeavedMethods { get; set; }

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

            WeaveTypeMethods(typeDefinition);
        }

        private void WeaveTypeMethods(TypeDefinition typeDefinition)
        {
            var methods = typeDefinition.Methods.ToArray();

            foreach (var originalMethod in methods)
            {
                if (originalMethod.IsGetter || originalMethod.IsSetter || originalMethod.IsConstructor)
                    continue;

                var method = originalMethod.TryGetMethodBase();
                if (method == null)
                {
                    ModuleWeaver.LogInfo($"Method {originalMethod.Name} from type {typeDefinition.FullName} will not be weaved because it was not possible to load its corresponding MethodBase");
                    continue;
                }

                if (method.IsAbstract)
                {
                    ModuleWeaver.LogInfo($"Method {originalMethod.Name} from type {typeDefinition.FullName} will not be weaved because it is an abstract method");
                    continue;
                }

                if (WeavedMethods.ContainsKey(method))
                {
                    continue;
                }

                if (method.IsAsyncMethod())
                {
                    var aspects = ModuleWeaver.Setup.GetAsyncMethodAspects(method).ToArray();
                    if (aspects.Length <= 0)
                    {
                        ModuleWeaver.LogInfo($"Method {originalMethod.Name} from type {typeDefinition.FullName} will not be weaved because no aspect was applied to it");
                        continue;
                    }

                    var clonedMethod = CloneMethod(typeDefinition, originalMethod, true);
                    typeDefinition.Methods.Add(clonedMethod);

                    var methodBindingType = CreateMethodBinding(typeDefinition, originalMethod, clonedMethod, aspects, true);
                    typeDefinition.NestedTypes.Add(methodBindingType);

                    RewriteOriginalMethod(typeDefinition, originalMethod, methodBindingType, true);
                }
                else
                {
                    var aspects = ModuleWeaver.Setup.GetMethodAspects(method).ToArray();
                    if (aspects.Length <= 0)
                    {
                        ModuleWeaver.LogInfo($"Method {originalMethod.Name} from type {typeDefinition.FullName} will not be weaved because no aspect was applied to it");
                        continue;
                    }

                    var clonedMethod = CloneMethod(typeDefinition, originalMethod, false);
                    typeDefinition.Methods.Add(clonedMethod);

                    var methodBindingType = CreateMethodBinding(typeDefinition, originalMethod, clonedMethod, aspects, false);
                    typeDefinition.NestedTypes.Add(methodBindingType);

                    RewriteOriginalMethod(typeDefinition, originalMethod, methodBindingType, false);
                }

                WeavedMethods.Add(method, true);
            }
        }

        private MethodDefinition CloneMethod(TypeDefinition typeDefinition, MethodDefinition originalMethod, bool async)
        {
            var attributes = originalMethod.Attributes;
            attributes &= ~MethodAttributes.Public;
            attributes &= ~MethodAttributes.SpecialName;
            attributes &= ~MethodAttributes.RTSpecialName;
            attributes |= MethodAttributes.Private;

            var methodName = Helpers.GetUniqueMethodName(typeDefinition, $"{originalMethod.Name}_original");
            var clone = new MethodDefinition(methodName, attributes, originalMethod.ReturnType);
            clone.Body.InitLocals = true;
            clone.AggressiveInlining = originalMethod.AggressiveInlining;
            clone.HasThis = originalMethod.HasThis;
            clone.ExplicitThis = originalMethod.ExplicitThis;
            clone.CallingConvention = originalMethod.CallingConvention;

            var compilerGeneratedAttrCtor = ModuleDefinition.ImportReference(typeof(CompilerGeneratedAttribute).GetConstructors().First());
            clone.CustomAttributes.Add(new CustomAttribute(compilerGeneratedAttrCtor));

            foreach (var attr in originalMethod.CustomAttributes)
                clone.CustomAttributes.Add(attr);

            foreach (var parameter in originalMethod.Parameters)
                clone.Parameters.Add(parameter);

            foreach (var variable in originalMethod.Body.Variables)
                clone.Body.Variables.Add(variable);

            foreach (var exceptionHandler in originalMethod.Body.ExceptionHandlers)
                clone.Body.ExceptionHandlers.Add(exceptionHandler);

            clone.CopyGenericParameters(originalMethod.GenericParameters);

            if (originalMethod.DebugInformation.HasSequencePoints)
            {
                foreach (var sequencePoint in originalMethod.DebugInformation.SequencePoints)
                    clone.DebugInformation.SequencePoints.Add(sequencePoint);

                clone.DebugInformation.Scope = new ScopeDebugInformation(originalMethod.Body.Instructions.First(), originalMethod.Body.Instructions.Last());
            }

            var ilProcessor = clone.Body.GetILProcessor();
            var instructions = originalMethod.Body.Instructions.AsEnumerable();
            if (originalMethod.IsConstructor && !originalMethod.IsStatic)
            {
                // if the method is a ctor, skips instructions up to the first "OpCodes.Call"
                // because these instructions represent the call to the base ctor, 
                // which will be kept at the original method
                instructions = instructions.SkipWhile(i => i.OpCode != OpCodes.Call).Skip(1);
            }

            foreach (var instruction in instructions)
                ilProcessor.Append(instruction);

            return clone;
        }

        private TypeDefinition CreateMethodBinding(TypeDefinition typeDefinition, MethodDefinition originalMethod, MethodDefinition clonedMethod, object[] aspects, bool async)
        {
            var methodBindingTypeDef = CreateMethodBindingTypeDef(typeDefinition, originalMethod, async);

            var instanceField = CreateMethodBindingInstanceField(methodBindingTypeDef, async);
            methodBindingTypeDef.Fields.Add(instanceField);

            var ctor = CreateMethodBindingCtor(methodBindingTypeDef, async);
            methodBindingTypeDef.Methods.Add(ctor);

            var staticCtor = CreateMethodBindingStaticCtor(methodBindingTypeDef, instanceField, ctor, aspects, async);
            methodBindingTypeDef.Methods.Add(staticCtor);

            var proceedMethod = CreateMethodBindingProceedMethod(typeDefinition, clonedMethod, methodBindingTypeDef, async);
            methodBindingTypeDef.Methods.Add(proceedMethod);

            return methodBindingTypeDef;
        }

        private TypeDefinition CreateMethodBindingTypeDef(TypeDefinition typeDefinition, MethodDefinition originalMethod, bool async)
        {
            var methodBindingTypeRef = ModuleDefinition.ImportReference(GetMethodBindingType(async));
            var typeAttributes = TypeAttributes.Sealed | TypeAttributes.NestedPrivate;
            var typeName = Helpers.GetUniqueBindingName(typeDefinition, originalMethod.Name);
            var typeDef = new TypeDefinition(typeDefinition.Namespace, typeName, typeAttributes, methodBindingTypeRef);

            typeDef.CopyGenericParameters(typeDefinition.GenericParameters);
            typeDef.CopyGenericParameters(originalMethod.GenericParameters);

            return typeDef;
        }

        private FieldDefinition CreateMethodBindingInstanceField(TypeDefinition methodBindingTypeDef, bool async)
        {
            var methodBindingTypeRef = methodBindingTypeDef.MakeTypeReference(methodBindingTypeDef.GenericParameters.ToArray());
            var instanceFieldAttributes = FieldAttributes.Static | FieldAttributes.Public;
            var instanceField = new FieldDefinition("INSTANCE", instanceFieldAttributes, methodBindingTypeRef);

            return instanceField;
        }

        private MethodDefinition CreateMethodBindingCtor(TypeDefinition methodBindingTypeDef, bool async)
        {
            var methodBindingType = GetMethodBindingType(async);
            var methodBindingCtorRef = ModuleDefinition.ImportReference(methodBindingType.GetConstructors().First());
            var ctorAttributes = MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.SpecialName | MethodAttributes.RTSpecialName;
            var ctor = new MethodDefinition(".ctor", ctorAttributes, ModuleWeaver.TypeSystem.VoidReference);
            ctor.Body.InitLocals = true;
            TypeReference interceptMethodAspectTypeRef = GetAspectTypeRef(async);
            ctor.Parameters.Add(new ParameterDefinition(new ArrayType(interceptMethodAspectTypeRef)));

            var ilProcessor = ctor.Body.GetILProcessor();
            ilProcessor.Append(Instruction.Create(OpCodes.Ldarg_0));
            ilProcessor.Append(Instruction.Create(OpCodes.Ldarg_1));
            ilProcessor.Append(Instruction.Create(OpCodes.Call, methodBindingCtorRef));
            ilProcessor.Append(Instruction.Create(OpCodes.Ret));

            return ctor;
        }

        private MethodDefinition CreateMethodBindingStaticCtor(TypeDefinition methodBindingTypeDef, FieldDefinition instanceField, MethodDefinition ctor, object[] aspects, bool async)
        {
            var methodBindingTypeRef = methodBindingTypeDef.MakeTypeReference(methodBindingTypeDef.GenericParameters.ToArray());
            var ctorRef = ctor.MakeMethodReference(methodBindingTypeRef, ctor.GenericParameters.ToArray());
            var instanceFieldRef = instanceField.MakeFieldReference(methodBindingTypeRef);

            var staticCtorAttributes = MethodAttributes.Private | MethodAttributes.HideBySig | MethodAttributes.SpecialName | MethodAttributes.RTSpecialName | MethodAttributes.Static;
            var staticCtor = new MethodDefinition(".cctor", staticCtorAttributes, ModuleWeaver.TypeSystem.VoidReference);
            staticCtor.Body.InitLocals = true;

            var baseAspectTypeRef = GetAspectTypeRef(async);
            var aspectsArrayVar = new VariableDefinition(new ArrayType(baseAspectTypeRef));
            staticCtor.Body.Variables.Add(aspectsArrayVar);

            var ilProcessor = staticCtor.Body.GetILProcessor();

            ilProcessor.Append(Instruction.Create(OpCodes.Ldc_I4, aspects.Length));
            ilProcessor.Append(Instruction.Create(OpCodes.Newarr, baseAspectTypeRef));
            ilProcessor.Append(Instruction.Create(OpCodes.Stloc, aspectsArrayVar));

            for (int i = 0; i < aspects.Length; i++)
            {
                var aspect = aspects[i];
                var aspectCtor = ModuleDefinition.ImportReference(aspect.GetType().GetConstructor(Type.EmptyTypes));

                ilProcessor.Append(Instruction.Create(OpCodes.Ldloc, aspectsArrayVar));
                ilProcessor.Append(Instruction.Create(OpCodes.Ldc_I4, i));
                ilProcessor.Append(Instruction.Create(OpCodes.Newobj, aspectCtor));
                ilProcessor.Append(Instruction.Create(OpCodes.Stelem_Ref));
            }

            ilProcessor.Append(Instruction.Create(OpCodes.Ldloc, aspectsArrayVar));
            ilProcessor.Append(Instruction.Create(OpCodes.Newobj, ctorRef));
            ilProcessor.Append(Instruction.Create(OpCodes.Stsfld, instanceFieldRef));
            ilProcessor.Append(Instruction.Create(OpCodes.Ret));

            return staticCtor;
        }

        private MethodDefinition CreateMethodBindingProceedMethod(TypeDefinition typeDefinition, MethodDefinition clonedMethod, TypeDefinition methodBindingTypeDef, bool async)
        {
            var availableGenericParameters = methodBindingTypeDef.GenericParameters.ToArray();
            var typeDefinitionRef = typeDefinition.MakeTypeReference(typeDefinition.GenericParameters.ToArray());
            var clonedMethodRef = clonedMethod.MakeMethodReference(typeDefinitionRef, availableGenericParameters);
            var bindingFlags = System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic;
            var methodBindingType = GetMethodBindingType(async);
            var methodBindingProceedMethodRef = ModuleDefinition.ImportReference(methodBindingType.GetMethod("Proceed", bindingFlags));
            var proceedMethodAttributes = MethodAttributes.Family | MethodAttributes.HideBySig | MethodAttributes.Virtual;
            var proceedMethodReturnType = async ? ModuleDefinition.ImportReference(typeof(Task)) : ModuleWeaver.TypeSystem.VoidReference;
            var proceedMethod = new MethodDefinition("Proceed", proceedMethodAttributes, proceedMethodReturnType);
            proceedMethod.Body.InitLocals = true;
            proceedMethod.Overrides.Add(methodBindingProceedMethodRef);

            var methodContextType = GetMethodContextType(async);
            var methodContextTypeRef = ModuleDefinition.ImportReference(methodContextType);
            var methodContextParam = new ParameterDefinition(methodContextTypeRef);
            proceedMethod.Parameters.Add(methodContextParam);

            var ilProcessor = proceedMethod.Body.GetILProcessor();

            if (clonedMethodRef.HasThis)
            {
                var instanceVar = new VariableDefinition(typeDefinitionRef);
                proceedMethod.Body.Variables.Add(instanceVar);

                var getInstanceMethod = ModuleDefinition.ImportReference(methodContextType.GetProperty("Instance").GetMethod);
                ilProcessor.Append(Instruction.Create(OpCodes.Ldarg_1));
                ilProcessor.Append(Instruction.Create(OpCodes.Callvirt, getInstanceMethod));

                if (typeDefinitionRef.IsValueType)
                    ilProcessor.Append(Instruction.Create(OpCodes.Unbox_Any, typeDefinitionRef));
                else
                    ilProcessor.Append(Instruction.Create(OpCodes.Castclass, typeDefinitionRef));

                ilProcessor.Append(Instruction.Create(OpCodes.Stloc, instanceVar));
            }

            // gets the Argument property from the MethodContext parameter
            var argumentsTypeRef = ModuleDefinition.ImportReference(typeof(Arguments));
            var argumentsVar = new VariableDefinition(argumentsTypeRef);
            proceedMethod.Body.Variables.Add(argumentsVar);

            var getArgumentsMethod = ModuleDefinition.ImportReference(methodContextType.GetProperty("Arguments").GetMethod);
            ilProcessor.Append(Instruction.Create(OpCodes.Ldarg_1));
            ilProcessor.Append(Instruction.Create(OpCodes.Callvirt, getArgumentsMethod));
            ilProcessor.Append(Instruction.Create(OpCodes.Stloc, argumentsVar));

            // loads the arguments into local variables
            var getArgumentValueMethod = ModuleDefinition.ImportReference(typeof(Arguments).GetMethod(nameof(Arguments.GetArgument), new Type[] { typeof(int) }));
            var localParametersVariables = new List<VariableDefinition>();
            foreach (var parameter in clonedMethodRef.Parameters)
            {
                var realParameterType = parameter.ParameterType.MakeTypeReference(availableGenericParameters);
                var parameterVar = new VariableDefinition(realParameterType);
                localParametersVariables.Add(parameterVar);
                proceedMethod.Body.Variables.Add(parameterVar);

                if (!parameter.IsOut)
                {
                    ilProcessor.Append(Instruction.Create(OpCodes.Ldloc, argumentsVar));
                    ilProcessor.Append(Instruction.Create(OpCodes.Ldc_I4, parameter.Index));
                    ilProcessor.Append(Instruction.Create(OpCodes.Callvirt, getArgumentValueMethod));

                    if (realParameterType.IsValueType || realParameterType.IsGenericParameter)
                        ilProcessor.Append(Instruction.Create(OpCodes.Unbox_Any, realParameterType));
                    else
                        ilProcessor.Append(Instruction.Create(OpCodes.Castclass, realParameterType));

                    ilProcessor.Append(Instruction.Create(OpCodes.Stloc, parameterVar));
                }
            }

            // prepares to call the original method, loading "this" and the local variables into the stack
            if (clonedMethodRef.HasThis)
                ilProcessor.Append(Instruction.Create(OpCodes.Ldloc_0));

            foreach (var parameter in clonedMethodRef.Parameters)
            {
                if (parameter.ParameterType.IsByReference)
                    ilProcessor.Append(Instruction.Create(OpCodes.Ldloca, localParametersVariables[parameter.Index]));
                else
                    ilProcessor.Append(Instruction.Create(OpCodes.Ldloc, localParametersVariables[parameter.Index]));
            }

            // calls the original method
            if (clonedMethodRef.HasThis)
                ilProcessor.Append(Instruction.Create(OpCodes.Callvirt, clonedMethodRef));
            else
                ilProcessor.Append(Instruction.Create(OpCodes.Call, clonedMethodRef));

            // set the MethodContext.ReturnValue with the value returned from the originalMethod
            if (!clonedMethodRef.ReturnType.SameTypeAs(ModuleWeaver.TypeSystem.VoidReference))
            {
                // stores the originalMethod return value into a local variable
                var returnType = clonedMethodRef.ReturnType.MakeTypeReference(availableGenericParameters);
                var returnValueVar = new VariableDefinition(returnType);
                proceedMethod.Body.Variables.Add(returnValueVar);
                ilProcessor.Append(Instruction.Create(OpCodes.Stloc, returnValueVar));

                if (async)
                {
                    if (returnType.IsGenericInstance)
                    {
                        // the method is async and return a Task<T>, so get the type of T
                        var typeOfT = (returnType as GenericInstanceType).GenericArguments[0];

                        // creates an instance of AsyncMethodBinding.Continuation
                        var continuationType = typeof(AsyncMethodBinding.ProceedContinuation);
                        var continuationCtor = ModuleDefinition.ImportReference(continuationType.GetConstructors().First());
                        var continuationTypeRef = ModuleDefinition.ImportReference(continuationType);
                        var continuationVar = new VariableDefinition(continuationTypeRef);
                        proceedMethod.Body.Variables.Add(continuationVar);
                        ilProcessor.Append(Instruction.Create(OpCodes.Ldarg_1));
                        ilProcessor.Append(Instruction.Create(OpCodes.Newobj, continuationCtor));
                        ilProcessor.Append(Instruction.Create(OpCodes.Stloc, continuationVar));

                        // creates an instance of Action<T> that will be used as the callback for ContinueWith
                        var continueWithCallbackType = typeof(Action<>);
                        var continueWithCallbackTypeRef = ModuleDefinition.ImportReference(continueWithCallbackType).MakeGenericInstanceType(returnType);
                        var continueWithCallbackCtor = ModuleDefinition.ImportReference(continueWithCallbackType.GetConstructors().First()).MakeMethodReference(continueWithCallbackTypeRef, continueWithCallbackTypeRef.GenericParameters.ToArray());
                        var continueMethodRef = ModuleDefinition.ImportReference(continuationType.GetMethod("Continue")).MakeGenericInstanceMethod(typeOfT);
                        var continueWithCallbackVar = new VariableDefinition(continueWithCallbackTypeRef);
                        proceedMethod.Body.Variables.Add(continueWithCallbackVar);
                        ilProcessor.Append(Instruction.Create(OpCodes.Ldloc, continuationVar));
                        ilProcessor.Append(Instruction.Create(OpCodes.Ldftn, continueMethodRef));
                        ilProcessor.Append(Instruction.Create(OpCodes.Newobj, continueWithCallbackCtor));
                        ilProcessor.Append(Instruction.Create(OpCodes.Stloc, continueWithCallbackVar));

                        // calls the ContinueWith method in the Task<T> returned by the originalMethod call
                        // this leaves a Task in the stack, that will be used as the return value
                        var continueWithMethodRef = ModuleDefinition.ImportReference(GetTaskOfTContinueWithMethod()).MakeMethodReference(returnType, returnType.GenericParameters.ToArray());
                        var getSynchronizationContextMethodRef = ModuleDefinition.ImportReference(typeof(SynchronizationContextHelper).GetMethod("GetSynchronizationContext", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static));
                        ilProcessor.Append(Instruction.Create(OpCodes.Ldloc, returnValueVar));
                        ilProcessor.Append(Instruction.Create(OpCodes.Ldloc, continueWithCallbackVar));
                        ilProcessor.Append(Instruction.Create(OpCodes.Call, getSynchronizationContextMethodRef));
                        ilProcessor.Append(Instruction.Create(OpCodes.Callvirt, continueWithMethodRef));
                    }
                    else
                    {
                        // the method is async but returns a Task instead of a Task<T>
                        // we will load this Task into the stack so that it is used as the return value
                        ilProcessor.Append(Instruction.Create(OpCodes.Ldloc, returnValueVar));
                    }
                }
                else
                {
                    // the method is NOT async, so we store the return value of the originalMethod call into the MethodContext.ReturnValue
                    var setReturnValue = ModuleDefinition.ImportReference(methodContextType.GetMethod("SetReturnValue", new Type[] { typeof(object) }));
                    ilProcessor.Append(Instruction.Create(OpCodes.Ldarg_1));
                    ilProcessor.Append(Instruction.Create(OpCodes.Ldloc, returnValueVar));

                    if (returnValueVar.VariableType.IsValueType || returnValueVar.VariableType.IsGenericParameter)
                        ilProcessor.Append(Instruction.Create(OpCodes.Box, returnValueVar.VariableType));

                    ilProcessor.Append(Instruction.Create(OpCodes.Callvirt, setReturnValue));
                }
            }
            else
            {
                if (async)
                {
                    // if the originalMethod is an "async void" method returns a dummy Task that is already completed
                    // to do this, we load Task.CompletedTask into the stack which will then be used as the return value
                    var getCompletedTaskMethodRef = ModuleDefinition.ImportReference(typeof(Task).GetProperty(nameof(Task.CompletedTask)).GetMethod);
                    ilProcessor.Append(Instruction.Create(OpCodes.Call, getCompletedTaskMethodRef));
                }
            }

            // sets the values of the arguments back into the MethodContext, because they can be modified when they're "out" or "ref" parameters
            var setArgumentValueMethod = ModuleDefinition.ImportReference(typeof(Arguments).GetMethod(nameof(Arguments.SetArgument), new Type[] { typeof(int), typeof(object) }));
            foreach (var parameter in clonedMethodRef.Parameters)
            {
                if (!parameter.ParameterType.IsByReference) continue;

                var localParameterVar = localParametersVariables[parameter.Index];
                ilProcessor.Append(Instruction.Create(OpCodes.Ldloc, argumentsVar));
                ilProcessor.Append(Instruction.Create(OpCodes.Ldc_I4, parameter.Index));
                ilProcessor.Append(Instruction.Create(OpCodes.Ldloc, localParameterVar));

                if (localParameterVar.VariableType.IsValueType || localParameterVar.VariableType.IsGenericParameter)
                    ilProcessor.Append(Instruction.Create(OpCodes.Box, localParameterVar.VariableType));

                ilProcessor.Append(Instruction.Create(OpCodes.Callvirt, setArgumentValueMethod));
            }

            ilProcessor.Append(Instruction.Create(OpCodes.Ret));

            return proceedMethod;
        }

        private void RewriteOriginalMethod(TypeDefinition typeDefinition, MethodDefinition originalMethod, TypeDefinition methodBindingTypeDef, bool async)
        {
            var typeDefintionRef = typeDefinition.MakeTypeReference(typeDefinition.GenericParameters.ToArray());
            var originalMethodRef = originalMethod.MakeMethodReference(typeDefintionRef, originalMethod.GenericParameters.ToArray());
            var availableGenericParameters = typeDefinition.GenericParameters.Concat(originalMethod.GenericParameters).ToArray();

            List<Instruction> baseCallInstructions = null;
            if (originalMethod.IsConstructor && !originalMethod.IsStatic)
            {
                // if the method is a constructor stores all instruction up to the first "OpCodes.Call"
                // because these instructions represente the call to the base ctor and must stay inside the original ctor
                baseCallInstructions = originalMethod.Body.Instructions.TakeWhile(i => i.OpCode != OpCodes.Call).ToList();
                baseCallInstructions.Add(originalMethod.Body.Instructions.Skip(baseCallInstructions.Count).First());
            }

            var ilProcessor = originalMethod.Body.GetILProcessor();
            originalMethod.Body.Instructions.Clear();
            originalMethod.Body.Variables.Clear();
            originalMethod.Body.InitLocals = true;

            // removes the AsyncStateMachineAttribute from the original method because, after it is rewritten, it no longer is async
            var asyncStateMachineAttribute = originalMethod.CustomAttributes.FirstOrDefault(a => a.AttributeType.FullName == typeof(AsyncStateMachineAttribute).FullName);
            if (asyncStateMachineAttribute != null) originalMethod.CustomAttributes.Remove(asyncStateMachineAttribute);

            // if baseCallInstructions is different from null, adds these instructions to the start of the method
            if (baseCallInstructions != null)
            {
                foreach (var instruction in baseCallInstructions)
                {
                    ilProcessor.Append(instruction);
                }
            }

            // creates a variable that will hold the Arguments instance
            var argumentsTypeRef = ModuleDefinition.ImportReference(typeof(Arguments));
            var argumentsCtor = ModuleDefinition.ImportReference(typeof(Arguments).GetConstructors().First());
            var argumentsVar = new VariableDefinition(argumentsTypeRef);
            originalMethod.Body.Variables.Add(argumentsVar);

            if (originalMethod.Parameters.Count > 0)
            {
                // creates an object[] containing the value of the parameters (this will be used to instantiate Arguments)
                var parameterValuesArrayVar = new VariableDefinition(new ArrayType(ModuleWeaver.TypeSystem.ObjectReference));
                originalMethod.Body.Variables.Add(parameterValuesArrayVar);
                ilProcessor.Append(Instruction.Create(OpCodes.Ldc_I4, originalMethod.Parameters.Count));
                ilProcessor.Append(Instruction.Create(OpCodes.Newarr, ModuleWeaver.TypeSystem.ObjectReference));
                foreach (var parameter in originalMethod.Parameters)
                {
                    var realParameterType = parameter.ParameterType.MakeTypeReference(availableGenericParameters);

                    ilProcessor.Append(Instruction.Create(OpCodes.Dup));
                    ilProcessor.Append(Instruction.Create(OpCodes.Ldc_I4, parameter.Index));
                    ilProcessor.Append(Instruction.Create(OpCodes.Ldarg, parameter));

                    if (parameter.ParameterType.IsByReference)
                    {
                        if (realParameterType.IsValueType || realParameterType.IsGenericParameter)
                            ilProcessor.Append(Instruction.Create(OpCodes.Ldobj, realParameterType));
                        else
                            ilProcessor.Append(Instruction.Create(OpCodes.Ldind_Ref));
                    }

                    if (realParameterType.IsValueType || realParameterType.IsGenericParameter)
                        ilProcessor.Append(Instruction.Create(OpCodes.Box, realParameterType));

                    ilProcessor.Append(Instruction.Create(OpCodes.Stelem_Ref));
                }
                ilProcessor.Append(Instruction.Create(OpCodes.Stloc, parameterValuesArrayVar));

                // creates the Arguments instance and stores it into the argumentsVar
                ilProcessor.Append(Instruction.Create(OpCodes.Ldloc, parameterValuesArrayVar));
                ilProcessor.Append(Instruction.Create(OpCodes.Newobj, argumentsCtor));
                ilProcessor.Append(Instruction.Create(OpCodes.Stloc, argumentsVar));
            }
            else
            {
                // stores the value of Arguments.Empty into the argumentsVar
                var emptyArgumentsField = ModuleDefinition.ImportReference(typeof(Arguments).GetField("Empty"));
                ilProcessor.Append(Instruction.Create(OpCodes.Ldsfld, emptyArgumentsField));
                ilProcessor.Append(Instruction.Create(OpCodes.Stloc, argumentsVar));
            }

            // creates a variable holding the reference to the MethodBase that represents the method being executed (this will be used to instantiate the MethodContext)
            var getMethodFromHandle = ModuleDefinition.ImportReference(typeof(System.Reflection.MethodBase).GetMethod(nameof(System.Reflection.MethodBase.GetMethodFromHandle), new[] { typeof(RuntimeMethodHandle), typeof(RuntimeTypeHandle) }));
            var methodBaseTypeRef = ModuleDefinition.ImportReference(typeof(System.Reflection.MethodBase));
            var methodBaseVar = new VariableDefinition(methodBaseTypeRef);
            originalMethod.Body.Variables.Add(methodBaseVar);
            ilProcessor.Append(Instruction.Create(OpCodes.Ldtoken, originalMethodRef));
            ilProcessor.Append(Instruction.Create(OpCodes.Ldtoken, typeDefintionRef));
            ilProcessor.Append(Instruction.Create(OpCodes.Call, getMethodFromHandle));
            ilProcessor.Append(Instruction.Create(OpCodes.Stloc, methodBaseVar));

            // creates the MethodContext instance
            var methodContextType = GetMethodContextType(async);
            var contextTypeRef = ModuleDefinition.ImportReference(methodContextType);
            var contextCtor = ModuleDefinition.ImportReference(methodContextType.GetConstructors().First());
            var contextVar = new VariableDefinition(contextTypeRef);
            originalMethod.Body.Variables.Add(contextVar);
            if (originalMethod.HasThis) ilProcessor.Append(Instruction.Create(OpCodes.Ldarg_0));
            if (!originalMethod.HasThis) ilProcessor.Append(Instruction.Create(OpCodes.Ldnull));
            ilProcessor.Append(Instruction.Create(OpCodes.Ldloc, methodBaseVar));
            ilProcessor.Append(Instruction.Create(OpCodes.Ldloc, argumentsVar));
            ilProcessor.Append(Instruction.Create(OpCodes.Newobj, contextCtor));
            ilProcessor.Append(Instruction.Create(OpCodes.Stloc, contextVar));

            // invoke the aspects
            var methodBindingType = GetMethodBindingType(async);
            var methodBindingTypeRef = methodBindingTypeDef.MakeTypeReference(availableGenericParameters);
            var methodBindingInstanceFieldDef = methodBindingTypeDef.Fields.First(f => f.IsStatic && f.Name == "INSTANCE");
            var methodBindingInstanceFieldRef = methodBindingInstanceFieldDef.MakeFieldReference(methodBindingTypeRef);
            var methodBindingRunMethodRef = ModuleDefinition.ImportReference(methodBindingType.GetMethod("Run"));
            var runTaskTypeRef = ModuleDefinition.ImportReference(typeof(Task));
            var runTaskVar = new VariableDefinition(runTaskTypeRef);
            ilProcessor.Append(Instruction.Create(OpCodes.Ldsfld, methodBindingInstanceFieldRef));
            ilProcessor.Append(Instruction.Create(OpCodes.Ldloc, contextVar));
            ilProcessor.Append(Instruction.Create(OpCodes.Callvirt, methodBindingRunMethodRef));

            if (async)
            {
                originalMethod.Body.Variables.Add(runTaskVar);
                ilProcessor.Append(Instruction.Create(OpCodes.Stloc, runTaskVar));
            }

            // sets the values of MethodContext.Arguments back into the method arguments, because they can be modified when they're "out" or "ref" parameters
            var getArgumentValueMethod = ModuleDefinition.ImportReference(typeof(Arguments).GetMethod(nameof(Arguments.GetArgument), new Type[] { typeof(int) }));
            foreach (var parameter in originalMethod.Parameters)
            {
                if (!parameter.ParameterType.IsByReference) continue;

                var realParameterType = (parameter.ParameterType as TypeSpecification).ElementType;
                ilProcessor.Append(Instruction.Create(OpCodes.Ldarg, parameter));
                ilProcessor.Append(Instruction.Create(OpCodes.Ldloc, argumentsVar));
                ilProcessor.Append(Instruction.Create(OpCodes.Ldc_I4, parameter.Index));
                ilProcessor.Append(Instruction.Create(OpCodes.Callvirt, getArgumentValueMethod));

                if (realParameterType.IsValueType || realParameterType.IsGenericParameter)
                {
                    ilProcessor.Append(Instruction.Create(OpCodes.Unbox_Any, realParameterType));
                    ilProcessor.Append(Instruction.Create(OpCodes.Stobj, realParameterType));
                }
                else
                {
                    ilProcessor.Append(Instruction.Create(OpCodes.Castclass, realParameterType));
                    ilProcessor.Append(Instruction.Create(OpCodes.Stind_Ref));
                }
            }

            // prepares the return value
            if (!originalMethod.ReturnType.SameTypeAs(ModuleWeaver.TypeSystem.VoidReference))
            {
                if (async)
                {
                    if (originalMethod.ReturnType.IsGenericInstance)
                    {
                        // method is async and return Task<T>
                        var returnType = originalMethod.ReturnType;
                        var typeOfT = (returnType as GenericInstanceType).GenericArguments[0];

                        // creates an instance of AsyncMethodBinding.Continuation
                        var continuationType = typeof(AsyncMethodBinding.RunContinuation);
                        var continuationCtor = ModuleDefinition.ImportReference(continuationType.GetConstructors().First());
                        var continuationTypeRef = ModuleDefinition.ImportReference(continuationType);
                        var continuationVar = new VariableDefinition(continuationTypeRef);
                        originalMethod.Body.Variables.Add(continuationVar);
                        ilProcessor.Append(Instruction.Create(OpCodes.Ldloc, contextVar));
                        ilProcessor.Append(Instruction.Create(OpCodes.Newobj, continuationCtor));
                        ilProcessor.Append(Instruction.Create(OpCodes.Stloc, continuationVar));

                        // creates an instance of Func<Task, T> that will be used as the callback for ContinueWith
                        var continueWithCallbackType = typeof(Func<,>);
                        var taskTypeRef = ModuleDefinition.ImportReference(typeof(Task));
                        var continueWithCallbackTypeRef = ModuleDefinition.ImportReference(continueWithCallbackType).MakeGenericInstanceType(taskTypeRef, typeOfT);
                        var continueWithCallbackCtor = ModuleDefinition.ImportReference(continueWithCallbackType.GetConstructors().First()).MakeMethodReference(continueWithCallbackTypeRef, continueWithCallbackTypeRef.GenericParameters.ToArray());
                        var continueMethodRef = ModuleDefinition.ImportReference(continuationType.GetMethod("Continue")).MakeGenericInstanceMethod(typeOfT);
                        var continueWithCallbackVar = new VariableDefinition(continueWithCallbackTypeRef);
                        originalMethod.Body.Variables.Add(continueWithCallbackVar);
                        ilProcessor.Append(Instruction.Create(OpCodes.Ldloc, continuationVar));
                        ilProcessor.Append(Instruction.Create(OpCodes.Ldftn, continueMethodRef));
                        ilProcessor.Append(Instruction.Create(OpCodes.Newobj, continueWithCallbackCtor));
                        ilProcessor.Append(Instruction.Create(OpCodes.Stloc, continueWithCallbackVar));

                        // calls the ContinueWith method in the Task returned by the Run call
                        // this leaves a Task<T> in the stack, that will be used as the return value
                        var continueWithMethodRef = ModuleDefinition.ImportReference(GetTaskContinueWithMethod()).MakeGenericInstanceMethod(typeOfT);
                        var getSynchronizationContextMethodRef = ModuleDefinition.ImportReference(typeof(SynchronizationContextHelper).GetMethod("GetSynchronizationContext", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static));
                        ilProcessor.Append(Instruction.Create(OpCodes.Ldloc, runTaskVar));
                        ilProcessor.Append(Instruction.Create(OpCodes.Ldloc, continueWithCallbackVar));
                        ilProcessor.Append(Instruction.Create(OpCodes.Call, getSynchronizationContextMethodRef));
                        ilProcessor.Append(Instruction.Create(OpCodes.Callvirt, continueWithMethodRef));

                    }
                    else
                    {
                        // method is async but return Task instead of Task<T>
                        // we will load the Task returned by the Run call to be used as the return value
                        ilProcessor.Append(Instruction.Create(OpCodes.Ldloc, runTaskVar));
                    }
                }
                else
                {
                    var getReturnValue = ModuleDefinition.ImportReference(methodContextType.GetProperty("ReturnValue").GetMethod);
                    ilProcessor.Append(Instruction.Create(OpCodes.Ldloc, contextVar));
                    ilProcessor.Append(Instruction.Create(OpCodes.Callvirt, getReturnValue));

                    if (originalMethod.ReturnType.IsValueType || originalMethod.ReturnType.IsGenericParameter)
                        ilProcessor.Append(Instruction.Create(OpCodes.Unbox_Any, originalMethod.ReturnType));
                    else
                        ilProcessor.Append(Instruction.Create(OpCodes.Castclass, originalMethod.ReturnType));
                }
            }

            ilProcessor.Append(Instruction.Create(OpCodes.Ret));
        }

        private System.Reflection.MethodInfo GetTaskOfTContinueWithMethod()
        {
            return typeof(Task<>).GetMethods().First(m =>
            {
                if (m.Name != "ContinueWith") return false;

                var parameters = m.GetParameters();
                if (parameters.Length != 2) return false;
                if (!parameters[0].ParameterType.IsGenericType) return false;
                if (parameters[0].ParameterType.GetGenericTypeDefinition() != typeof(Action<>)) return false;
                if (parameters[1].ParameterType != typeof(TaskScheduler)) return false;

                return true;
            });
        }

        private System.Reflection.MethodInfo GetTaskContinueWithMethod()
        {
            return typeof(Task).GetMethods().First(m =>
            {
                if (m.Name != "ContinueWith") return false;

                var parameters = m.GetParameters();
                if (parameters.Length != 2) return false;
                if (!parameters[0].ParameterType.IsGenericType) return false;
                if (parameters[0].ParameterType.GetGenericTypeDefinition() != typeof(Func<,>)) return false;
                if (parameters[1].ParameterType != typeof(TaskScheduler)) return false;

                return true;
            });
        }

        private static Type GetMethodContextType(bool async)
        {
            if (async)
                return typeof(AsyncMethodContext);
            else
                return typeof(MethodContext);
        }

        private Type GetMethodBindingType(bool async)
        {
            if (async)
                return typeof(AsyncMethodBinding);
            else
                return typeof(MethodBinding);
        }

        private TypeReference GetAspectTypeRef(bool async)
        {
            if (async)
                return ModuleDefinition.ImportReference(typeof(InterceptAsyncMethodAspect));
            else
                return ModuleDefinition.ImportReference(typeof(InterceptMethodAspect));
        }
    }
}
