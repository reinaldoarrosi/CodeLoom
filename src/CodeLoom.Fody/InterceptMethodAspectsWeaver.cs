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
using CodeLoom.Helpers;
using System.Diagnostics;
using Mono.Collections.Generic;

namespace CodeLoom.Fody
{
    internal class InterceptMethodAspectsWeaver
    {
        internal ModuleWeaver ModuleWeaver;

        internal InterceptMethodAspectsWeaver(ModuleWeaver moduleWeaver)
        {
            ModuleWeaver = moduleWeaver;
            WeavedMethods = new Dictionary<System.Reflection.MethodBase, bool>();
        }

        internal ModuleDefinition ModuleDefinition { get { return ModuleWeaver.ModuleDefinition; } }
        internal Dictionary<System.Reflection.MethodBase, bool> WeavedMethods { get; set; }

        internal virtual void Weave(TypeDefinition typeDefinition)
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
                // do not weave property getters/setters because this is done by the IntercetPropertyAspectsWeaver
                if (originalMethod.IsGetter || originalMethod.IsSetter)
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
                    // if the method we're trying to weave was already weaved we'll skip weaving it
                    // all aspects are applied the first time the method is weaved, there's no need to weave it more than once
                    continue;
                }
                else
                {
                    // mark this method so that it won't be weaved again
                    WeavedMethods.Add(method, true);
                }

                if (method.IsAsyncMethod())
                {
                    // if the method is async (uses the async keyword) we use a different way of getting the aspects of these methods
                    // this is done because aspects of async methods must implement IInterceptAsyncMethodAspect instead of IInterceptMethodAspect
                    var aspects = ModuleWeaver.Setup.GetAsyncMethodAspects(method).ToArray();
                    if (aspects.Length <= 0)
                    {
                        ModuleWeaver.LogInfo($"Method {originalMethod.Name} from type {typeDefinition.FullName} will not be weaved because no aspect was applied to it");
                        continue;
                    }

                    // clone the original method because we're going to rewrite it
                    var clonedMethod = CloneMethod(typeDefinition, originalMethod, true);
                    typeDefinition.Methods.Add(clonedMethod);

                    // creates a Binding class that inherits from AsyncMethodBinding
                    // this class is used to invoke the aspects in a way that works with generic methods
                    var methodBindingTypeDef = CreateBindingTypeDef(typeDefinition, typeof(AsyncMethodBinding), originalMethod.Name, originalMethod.GenericParameters);

                    // creates the static INSTANCE field that will hold the Binding instance, and adds it to our class
                    var instanceField = CreateBindingInstanceField(methodBindingTypeDef);
                    methodBindingTypeDef.Fields.Add(instanceField);

                    // creates the constructor of and adds it to our Binding class
                    var ctor = CreateBindingCtor(methodBindingTypeDef, typeof(IInterceptAsyncMethodAspect));
                    methodBindingTypeDef.Methods.Add(ctor);

                    // creates the static constructor and adds it to our Binding class 
                    var staticCtor = CreateBindingStaticCtor(methodBindingTypeDef, instanceField, ctor, typeof(IInterceptAsyncMethodAspect), aspects);
                    methodBindingTypeDef.Methods.Add(staticCtor);

                    // creates the Proceed method that overrides the abstract method MethodBinding.Proceed/AsyncMethodBinding.Proceed
                    var bindingFlags = System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic;
                    var abstractProceedMethod = ModuleDefinition.ImportReference(typeof(MethodBinding).GetMethod("Proceed", bindingFlags));
                    var proceedMethod = CreateBindingProceedMethod(typeDefinition, methodBindingTypeDef, typeof(AsyncMethodContext), clonedMethod, abstractProceedMethod, true);
                    methodBindingTypeDef.Methods.Add(proceedMethod);

                    // adds our Binding class as a nested type of the class that we're weaving
                    typeDefinition.NestedTypes.Add(methodBindingTypeDef);

                    // rewrites the original method so that it calls AsyncMethodBinding.Run
                    RewriteOriginalMethod(typeDefinition, methodBindingTypeDef, typeof(AsyncMethodContext), originalMethod, true);
                }
                else
                {
                    // if the method is NOT async get the aspects that implement IInterceptMethodAspect
                    var aspects = ModuleWeaver.Setup.GetMethodAspects(method).ToArray();
                    if (aspects.Length <= 0)
                    {
                        ModuleWeaver.LogInfo($"Method {originalMethod.Name} from type {typeDefinition.FullName} will not be weaved because no aspect was applied to it");
                        continue;
                    }

                    // clone the original method because we're going to rewrite it
                    var clonedMethod = CloneMethod(typeDefinition, originalMethod, false);
                    typeDefinition.Methods.Add(clonedMethod);

                    // creates a Binding class that inherits from MethodBinding
                    // this class is used to invoke the aspects in a way that works with generic methods
                    var methodBindingTypeDef = CreateBindingTypeDef(typeDefinition, typeof(MethodBinding), originalMethod.Name, originalMethod.GenericParameters);

                    // creates the static INSTANCE field that will hold the Binding instance, and adds it to our class
                    var instanceField = CreateBindingInstanceField(methodBindingTypeDef);
                    methodBindingTypeDef.Fields.Add(instanceField);

                    // creates the constructor of and adds it to our Binding class
                    var ctor = CreateBindingCtor(methodBindingTypeDef, typeof(IInterceptMethodAspect));
                    methodBindingTypeDef.Methods.Add(ctor);

                    // creates the static constructor and adds it to our Binding class 
                    var staticCtor = CreateBindingStaticCtor(methodBindingTypeDef, instanceField, ctor, typeof(IInterceptMethodAspect), aspects);
                    methodBindingTypeDef.Methods.Add(staticCtor);

                    // creates the Proceed method that overrides the abstract method MethodBinding.Proceed/AsyncMethodBinding.Proceed
                    var bindingFlags = System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic;
                    var abstractProceedMethod = ModuleDefinition.ImportReference(typeof(MethodBinding).GetMethod("Proceed", bindingFlags));
                    var proceedMethod = CreateBindingProceedMethod(typeDefinition, methodBindingTypeDef, typeof(MethodContext), clonedMethod, abstractProceedMethod, false);
                    methodBindingTypeDef.Methods.Add(proceedMethod);

                    // adds our Binding class as a nested type of the class that we're weaving
                    typeDefinition.NestedTypes.Add(methodBindingTypeDef);

                    // rewrites the original method so that it calls MethodBinding.Run
                    RewriteOriginalMethod(typeDefinition, methodBindingTypeDef, typeof(MethodContext), originalMethod, false);
                }
            }
        }

        protected MethodDefinition CloneMethod(TypeDefinition typeDefinition, MethodDefinition originalMethod, bool async)
        {
            // prepares the attributes of the cloned method
            var attributes = originalMethod.Attributes;
            attributes &= ~MethodAttributes.Public;
            attributes &= ~MethodAttributes.SpecialName;
            attributes &= ~MethodAttributes.RTSpecialName;
            attributes |= MethodAttributes.Private;

            // creates a new MethodDefinition that will represent out cloned method
            var methodName = Helpers.GetUniqueMethodName(typeDefinition, $"{originalMethod.Name}_original");
            var clone = new MethodDefinition(methodName, attributes, originalMethod.ReturnType);
            clone.Body.InitLocals = true;
            clone.AggressiveInlining = originalMethod.AggressiveInlining;
            clone.HasThis = originalMethod.HasThis;
            clone.ExplicitThis = originalMethod.ExplicitThis;
            clone.CallingConvention = originalMethod.CallingConvention;

            // adds the CompilerGeneratedAttribute we're creating a new method
            var compilerGeneratedAttrCtor = ModuleDefinition.ImportReference(typeof(CompilerGeneratedAttribute).GetConstructors().First());
            clone.CustomAttributes.Add(new CustomAttribute(compilerGeneratedAttrCtor));

            // copies attributes of the original method
            foreach (var attr in originalMethod.CustomAttributes)
                clone.CustomAttributes.Add(attr);

            // copies parameters of the original method
            foreach (var parameter in originalMethod.Parameters)
                clone.Parameters.Add(parameter);

            // copies variables of the original method
            foreach (var variable in originalMethod.Body.Variables)
                clone.Body.Variables.Add(variable);

            // copies try/catch/finally blocks of the original method
            foreach (var exceptionHandler in originalMethod.Body.ExceptionHandlers)
                clone.Body.ExceptionHandlers.Add(exceptionHandler);

            // copies generic parameters from the original method
            clone.CopyGenericParameters(originalMethod.GenericParameters);

            // copies debug symbols from the original method
            clone.CopyDebugInformation(originalMethod);

            var ilProcessor = clone.Body.GetILProcessor();
            var instructions = originalMethod.Body.Instructions.AsEnumerable();
            if (originalMethod.IsConstructor && !originalMethod.IsStatic)
            {
                // if the method is a ctor, skips instructions up to the first "OpCodes.Call"
                // because these instructions represent the call to the base ctor, 
                // which will be kept at the original method
                instructions = instructions.SkipWhile(i => i.OpCode != OpCodes.Call).Skip(1);
            }

            // copies all the instructions of the original method
            foreach (var instruction in instructions)
                ilProcessor.Append(instruction);

            return clone;
        }

        protected TypeDefinition CreateBindingTypeDef(TypeDefinition typeDefinition, Type bindingType, string originalName, Collection<GenericParameter> originalGenericParameters)
        {
            // creates new TypeDefinition that represents a class that inherits from MethodBinding/AsyncMethodBinding
            var methodBindingTypeRef = ModuleDefinition.ImportReference(bindingType);
            var typeAttributes = TypeAttributes.Sealed | TypeAttributes.NestedPrivate;
            var typeName = Helpers.GetUniqueBindingName(typeDefinition, originalName);
            var typeDef = new TypeDefinition(typeDefinition.Namespace, typeName, typeAttributes, methodBindingTypeRef);

            // adds the CompilerGeneratedAttribute to the type that we are creating
            var compilerGeneratedAttrCtor = ModuleDefinition.ImportReference(typeof(CompilerGeneratedAttribute).GetConstructors().First());
            typeDef.CustomAttributes.Add(new CustomAttribute(compilerGeneratedAttrCtor));

            // adds the DebuggerStepThroughAttribute so that the debugger skips the generated MethodBinding class
            var debuggerStepThroughAttribute = ModuleDefinition.ImportReference(typeof(DebuggerStepThroughAttribute).GetConstructors().First());
            typeDef.CustomAttributes.Add(new CustomAttribute(debuggerStepThroughAttribute));

            // this new type will be nested inside the declaring type of the original method, so we need to copy its generic parameters
            typeDef.CopyGenericParameters(typeDefinition.GenericParameters);

            // we also copy the generic parameters of the original method itself, so that we have access to them inside our newly created type
            typeDef.CopyGenericParameters(originalGenericParameters);

            return typeDef;
        }

        protected FieldDefinition CreateBindingInstanceField(TypeDefinition methodBindingTypeDef)
        {
            // creates a static field that will hold an instance of our "method binding"
            // this effectively representes the singleton pattern, meaning that we will only have one single instance of a particular binding
            // which gives us better performance when invoking the method
            var methodBindingTypeRef = methodBindingTypeDef.MakeTypeReference(methodBindingTypeDef.GenericParameters.ToArray());
            var instanceFieldAttributes = FieldAttributes.Static | FieldAttributes.Public;
            var instanceField = new FieldDefinition("INSTANCE", instanceFieldAttributes, methodBindingTypeRef);

            return instanceField;
        }

        protected MethodDefinition CreateBindingCtor(TypeDefinition methodBindingTypeDef, Type aspectsType)
        {
            // create a private constructor for our Binding
            // this constructor receives an array of aspects
            // which corresponds to all the aspects that were applied to the original member
            var methodBindingBaseTypeDef = methodBindingTypeDef.BaseType.Resolve();
            var methodBindingCtorRef = ModuleDefinition.ImportReference(methodBindingBaseTypeDef.Methods.First(m => m.IsConstructor));
            var ctorAttributes = MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.SpecialName | MethodAttributes.RTSpecialName;
            var ctor = new MethodDefinition(".ctor", ctorAttributes, ModuleWeaver.TypeSystem.VoidReference);
            ctor.Body.InitLocals = true;
            TypeReference interceptMethodAspectTypeRef = ModuleDefinition.ImportReference(aspectsType);
            ctor.Parameters.Add(new ParameterDefinition(new ArrayType(interceptMethodAspectTypeRef)));

            // creates the MSIL instructions that calls the base constructor of our Binding class
            var ilProcessor = ctor.Body.GetILProcessor();
            ilProcessor.Append(Instruction.Create(OpCodes.Ldarg_0));
            ilProcessor.Append(Instruction.Create(OpCodes.Ldarg_1));
            ilProcessor.Append(Instruction.Create(OpCodes.Call, methodBindingCtorRef));
            ilProcessor.Append(Instruction.Create(OpCodes.Ret));

            return ctor;
        }

        protected MethodDefinition CreateBindingStaticCtor(TypeDefinition methodBindingTypeDef, FieldDefinition instanceField, MethodDefinition ctor, Type aspectsType, object[] aspects)
        {
            // get type and method reference that we'll need later
            var methodBindingTypeRef = methodBindingTypeDef.MakeTypeReference(methodBindingTypeDef.GenericParameters.ToArray());
            var ctorRef = ctor.MakeMethodReference(methodBindingTypeRef, ctor.GenericParameters.ToArray());
            var instanceFieldRef = instanceField.MakeFieldReference(methodBindingTypeRef);

            // creates a static constructor on our "method binding"
            // this will be used to instantiate our singleton instance
            var staticCtorAttributes = MethodAttributes.Private | MethodAttributes.HideBySig | MethodAttributes.SpecialName | MethodAttributes.RTSpecialName | MethodAttributes.Static;
            var staticCtor = new MethodDefinition(".cctor", staticCtorAttributes, ModuleWeaver.TypeSystem.VoidReference);
            staticCtor.Body.InitLocals = true;

            // create a local variable that will hold the IInterceptMethodAspect/IInterceptAsyncMethodAspect array
            // this array will be used to instantiate the "method binding" instance
            var baseAspectTypeRef = ModuleDefinition.ImportReference(aspectsType);
            var aspectsArrayVar = new VariableDefinition(new ArrayType(baseAspectTypeRef));
            staticCtor.Body.Variables.Add(aspectsArrayVar);

            // create a new array with length equals to the number of aspects associated with the original method
            var ilProcessor = staticCtor.Body.GetILProcessor();
            ilProcessor.Append(Instruction.Create(OpCodes.Ldc_I4, aspects.Length));
            ilProcessor.Append(Instruction.Create(OpCodes.Newarr, baseAspectTypeRef));
            ilProcessor.Append(Instruction.Create(OpCodes.Stloc, aspectsArrayVar));

            // for each associated aspect, invokes the default constructor of it and stores it into the previously created array
            for (int i = 0; i < aspects.Length; i++)
            {
                var aspect = aspects[i];
                var aspectCtor = ModuleDefinition.ImportReference(aspect.GetType().GetConstructor(Type.EmptyTypes));

                ilProcessor.Append(Instruction.Create(OpCodes.Ldloc, aspectsArrayVar));
                ilProcessor.Append(Instruction.Create(OpCodes.Ldc_I4, i));
                ilProcessor.Append(Instruction.Create(OpCodes.Newobj, aspectCtor));
                ilProcessor.Append(Instruction.Create(OpCodes.Stelem_Ref));
            }

            // invokes the constructor of the "method binding" passing the array of aspects as argument
            // stores the instance into the static field created when we called CreateMethodBindingInstanceField earlier
            ilProcessor.Append(Instruction.Create(OpCodes.Ldloc, aspectsArrayVar));
            ilProcessor.Append(Instruction.Create(OpCodes.Newobj, ctorRef));
            ilProcessor.Append(Instruction.Create(OpCodes.Stsfld, instanceFieldRef));
            ilProcessor.Append(Instruction.Create(OpCodes.Ret));

            return staticCtor;
        }

        protected MethodDefinition CreateBindingProceedMethod(TypeDefinition typeDefinition, TypeDefinition methodBindingTypeDef, Type contextType, MethodDefinition clonedMethod, MethodReference methodToOverrideRef, bool async)
        {
            // get type ad method references that we'll use later
            var availableGenericParameters = methodBindingTypeDef.GenericParameters.ToArray();
            var typeDefinitionRef = typeDefinition.MakeTypeReference(typeDefinition.GenericParameters.ToArray());
            var clonedMethodRef = clonedMethod.MakeMethodReference(typeDefinitionRef, availableGenericParameters);
            var proceedMethodReturnType = async ? ModuleDefinition.ImportReference(typeof(Task)) : ModuleWeaver.TypeSystem.VoidReference;

            // creates a new MethodDefinition representing the "Proceed" method in our "method binding"
            // the "Proceed" method is responsible for invoking the original code of the method that we're intercepting
            var proceedMethodAttributes = MethodAttributes.Family | MethodAttributes.HideBySig | MethodAttributes.Virtual;
            var proceedMethod = new MethodDefinition("Proceed", proceedMethodAttributes, proceedMethodReturnType);
            proceedMethod.Body.InitLocals = true;
            proceedMethod.Overrides.Add(methodToOverrideRef);

            // the "Proceed" method receives a "context" parameter
            var methodContextTypeRef = ModuleDefinition.ImportReference(contextType);
            var methodContextParam = new ParameterDefinition(methodContextTypeRef);
            proceedMethod.Parameters.Add(methodContextParam);

            var ilProcessor = proceedMethod.Body.GetILProcessor();

            if (clonedMethodRef.HasThis)
            {
                // if the method we're intercepting is an instance method (not static) we'll get the target instance 
                // from MethodContext.Instance and store it into a local variable so that we can use it later
                var instanceVar = new VariableDefinition(typeDefinitionRef);
                proceedMethod.Body.Variables.Add(instanceVar);

                var getInstanceMethod = ModuleDefinition.ImportReference(contextType.GetProperty("Instance").GetMethod);
                ilProcessor.Append(Instruction.Create(OpCodes.Ldarg_1));
                ilProcessor.Append(Instruction.Create(OpCodes.Callvirt, getInstanceMethod));

                // MethodContext.Instance returns an object, so cast it to the correct type
                if (typeDefinitionRef.IsValueType)
                    ilProcessor.Append(Instruction.Create(OpCodes.Unbox_Any, typeDefinitionRef));
                else
                    ilProcessor.Append(Instruction.Create(OpCodes.Castclass, typeDefinitionRef));

                ilProcessor.Append(Instruction.Create(OpCodes.Stloc, instanceVar));
            }

            // creates a local variable that will store the MethodContext.Arguments value because it will be used multiple times
            var argumentsTypeRef = ModuleDefinition.ImportReference(typeof(Arguments));
            var argumentsVar = new VariableDefinition(argumentsTypeRef);
            proceedMethod.Body.Variables.Add(argumentsVar);

            // gets the Arguments from MethodContext.Arguments and store it into the previously created local variable 
            var getArgumentsMethod = ModuleDefinition.ImportReference(contextType.GetProperty("Arguments").GetMethod);
            ilProcessor.Append(Instruction.Create(OpCodes.Ldarg_1));
            ilProcessor.Append(Instruction.Create(OpCodes.Callvirt, getArgumentsMethod));
            ilProcessor.Append(Instruction.Create(OpCodes.Stloc, argumentsVar));

            // loads the value of each parameter into a local variable
            var getArgumentValueMethod = ModuleDefinition.ImportReference(typeof(Arguments).GetMethod(nameof(Arguments.GetArgument), new Type[] { typeof(int) }));
            var localParametersVariables = new List<VariableDefinition>();
            foreach (var parameter in clonedMethodRef.Parameters)
            {
                // creates a local variable to store the value of the parameter
                var realParameterType = parameter.ParameterType.MakeTypeReference(availableGenericParameters);
                var parameterVar = new VariableDefinition(realParameterType);
                localParametersVariables.Add(parameterVar);
                proceedMethod.Body.Variables.Add(parameterVar);

                if (!parameter.IsOut) // if the parameter is an "out" parameter it does not have a value yet
                {
                    // calls Arguments.GetArgument to the value of the parameter
                    ilProcessor.Append(Instruction.Create(OpCodes.Ldloc, argumentsVar));
                    ilProcessor.Append(Instruction.Create(OpCodes.Ldc_I4, parameter.Index));
                    ilProcessor.Append(Instruction.Create(OpCodes.Callvirt, getArgumentValueMethod));

                    // Arguments.GetArgument returns an object, so we need to cast it to the correct type
                    if (realParameterType.IsValueType || realParameterType.IsGenericParameter)
                        ilProcessor.Append(Instruction.Create(OpCodes.Unbox_Any, realParameterType));
                    else
                        ilProcessor.Append(Instruction.Create(OpCodes.Castclass, realParameterType));

                    // stores the value into a local variable
                    ilProcessor.Append(Instruction.Create(OpCodes.Stloc, parameterVar));
                }
            }

            // prepares to call the original method, loading "this" into the stack
            if (clonedMethodRef.HasThis)
                ilProcessor.Append(Instruction.Create(OpCodes.Ldloc_0));

            // prepares to call the original method, loading all local variables that represents the values of the parameters into the stack
            foreach (var parameter in clonedMethodRef.Parameters)
            {
                if (parameter.ParameterType.IsByReference)
                    ilProcessor.Append(Instruction.Create(OpCodes.Ldloca, localParametersVariables[parameter.Index]));
                else
                    ilProcessor.Append(Instruction.Create(OpCodes.Ldloc, localParametersVariables[parameter.Index]));
            }

            // calls the original method (checking if it is a static method or not)
            if (clonedMethodRef.HasThis)
                ilProcessor.Append(Instruction.Create(OpCodes.Callvirt, clonedMethodRef));
            else
                ilProcessor.Append(Instruction.Create(OpCodes.Call, clonedMethodRef));

            // set the MethodContext.ReturnValue with the value returned from the originalMethod call
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
                    var setReturnValue = ModuleDefinition.ImportReference(contextType.GetMethod("SetReturnValue", new Type[] { typeof(object) }));
                    ilProcessor.Append(Instruction.Create(OpCodes.Ldarg_1));
                    ilProcessor.Append(Instruction.Create(OpCodes.Ldloc, returnValueVar));

                    // MethodContext.ReturnValue is an object, so we may need to box the value
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

        protected void RewriteOriginalMethod(TypeDefinition typeDefinition, TypeDefinition methodBindingTypeDef, Type contextType, MethodDefinition originalMethod, bool async)
        {
            // get type and method references that we'll need later
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

            // clear all instructions, variables and debug information from the original method because we're going to rewrite it from scratch
            var ilProcessor = originalMethod.Body.GetILProcessor();
            originalMethod.Body.Instructions.Clear();
            originalMethod.Body.Variables.Clear();
            originalMethod.DebugInformation.CustomDebugInformations.Clear();
            originalMethod.DebugInformation.SequencePoints.Clear();
            originalMethod.DebugInformation.Scope = null;
            originalMethod.Body.InitLocals = true;

            // removes the AsyncStateMachineAttribute from the original method because, after it is rewritten, it no longer is async
            var asyncStateMachineAttribute = originalMethod.CustomAttributes.FirstOrDefault(a => a.AttributeType.FullName == typeof(AsyncStateMachineAttribute).FullName);
            if (asyncStateMachineAttribute != null) originalMethod.CustomAttributes.Remove(asyncStateMachineAttribute);

            // removes the IteratorStateMachineAttribute from the original method because, after it is rewritten, it no longer an iterator
            var iteratorStateMachineAttribute = originalMethod.CustomAttributes.FirstOrDefault(a => a.AttributeType.FullName == typeof(IteratorStateMachineAttribute).FullName);
            if (iteratorStateMachineAttribute != null) originalMethod.CustomAttributes.Remove(iteratorStateMachineAttribute);

            // adds the CompilerGeneratedAttribute to the original method since it will be rewritten and will not resemble the user code at all
            var compilerGeneratedAttrCtor = ModuleDefinition.ImportReference(typeof(CompilerGeneratedAttribute).GetConstructors().First());
            originalMethod.CustomAttributes.Add(new CustomAttribute(compilerGeneratedAttrCtor));

            // adds the DebuggerStepThroughAttribute so that the debugger skips this method, becaus it no longer represente user's code
            var debuggerStepThroughAttribute = ModuleDefinition.ImportReference(typeof(DebuggerStepThroughAttribute).GetConstructors().First());
            originalMethod.CustomAttributes.Add(new CustomAttribute(debuggerStepThroughAttribute));

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

                // stores the value of each parameter into the object[] that was created before
                foreach (var parameter in originalMethod.Parameters)
                {
                    var realParameterType = parameter.ParameterType.MakeTypeReference(availableGenericParameters);

                    ilProcessor.Append(Instruction.Create(OpCodes.Dup));
                    ilProcessor.Append(Instruction.Create(OpCodes.Ldc_I4, parameter.Index));
                    ilProcessor.Append(Instruction.Create(OpCodes.Ldarg, parameter));

                    if (parameter.ParameterType.IsByReference)
                    {
                        // if the parameter is by reference we need to load a pointer to the value
                        if (realParameterType.IsValueType || realParameterType.IsGenericParameter)
                            ilProcessor.Append(Instruction.Create(OpCodes.Ldobj, realParameterType));
                        else
                            ilProcessor.Append(Instruction.Create(OpCodes.Ldind_Ref));
                    }

                    // since we're storing the values into an object[], we may need to box these values before storing them
                    if (realParameterType.IsValueType || realParameterType.IsGenericParameter)
                        ilProcessor.Append(Instruction.Create(OpCodes.Box, realParameterType));

                    ilProcessor.Append(Instruction.Create(OpCodes.Stelem_Ref));
                }

                // stores our object[] into the local variable we previously created
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


            // creates the MethodContext instance
            var contextTypeRef = ModuleDefinition.ImportReference(contextType);
            var contextCtor = ModuleDefinition.ImportReference(contextType.GetConstructors().First());
            var contextVar = new VariableDefinition(contextTypeRef);
            originalMethod.Body.Variables.Add(contextVar);
            if (originalMethod.HasThis) ilProcessor.Append(Instruction.Create(OpCodes.Ldarg_0));
            if (!originalMethod.HasThis) ilProcessor.Append(Instruction.Create(OpCodes.Ldnull));
            ilProcessor.Append(Instruction.Create(OpCodes.Ldtoken, typeDefintionRef));
            ilProcessor.Append(Instruction.Create(OpCodes.Ldtoken, originalMethodRef));
            ilProcessor.Append(Instruction.Create(OpCodes.Ldloc, argumentsVar));
            ilProcessor.Append(Instruction.Create(OpCodes.Newobj, contextCtor));
            ilProcessor.Append(Instruction.Create(OpCodes.Stloc, contextVar));

            // invoke MethodBinding.Run which will then invoke each of the aspects associated with the method we're intercepting
            var methodBindingTypeRef = methodBindingTypeDef.MakeTypeReference(availableGenericParameters);
            var methodBindingInstanceFieldDef = methodBindingTypeDef.Fields.First(f => f.IsStatic && f.Name == "INSTANCE");
            var methodBindingInstanceFieldRef = methodBindingInstanceFieldDef.MakeFieldReference(methodBindingTypeRef);
            var methodBindingRunMethodRef = ModuleDefinition.ImportReference(methodBindingTypeDef.Methods.First(m => m.Name == "Run"));
            
            ilProcessor.Append(Instruction.Create(OpCodes.Ldsfld, methodBindingInstanceFieldRef));
            ilProcessor.Append(Instruction.Create(OpCodes.Ldloc, contextVar));
            ilProcessor.Append(Instruction.Create(OpCodes.Callvirt, methodBindingRunMethodRef));

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

            // if the method is async AsyncMethodBinding.Run returns a Task into the stack
            // we could just pop the stack but we may need this Task later, so we create
            // a local variable and store the Task into it
            var runTaskTypeRef = ModuleDefinition.ImportReference(typeof(Task));
            var runTaskVar = new VariableDefinition(runTaskTypeRef);
            if (async)
            {
                originalMethod.Body.Variables.Add(runTaskVar);
                ilProcessor.Append(Instruction.Create(OpCodes.Stloc, runTaskVar));
            }

            // prepares the return value
            if (!originalMethod.ReturnType.SameTypeAs(ModuleWeaver.TypeSystem.VoidReference))
            {
                if (async)
                {
                    if (originalMethod.ReturnType.IsGenericInstance)
                    {
                        // method is async and return Task<T>, so we get the type of T
                        var typeOfT = (originalMethod.ReturnType as GenericInstanceType).GenericArguments[0];

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
                    // method is not async, so get the return value from MethodContext.ReturnValue and put it into the stack
                    var getReturnValue = ModuleDefinition.ImportReference(contextType.GetProperty("ReturnValue").GetMethod);
                    ilProcessor.Append(Instruction.Create(OpCodes.Ldloc, contextVar));
                    ilProcessor.Append(Instruction.Create(OpCodes.Callvirt, getReturnValue));

                    // since MethodContext.ReturnValue is an object, cast it to the correct type
                    if (originalMethod.ReturnType.IsValueType || originalMethod.ReturnType.IsGenericParameter)
                        ilProcessor.Append(Instruction.Create(OpCodes.Unbox_Any, originalMethod.ReturnType));
                    else
                        ilProcessor.Append(Instruction.Create(OpCodes.Castclass, originalMethod.ReturnType));
                }
            }
            
            // returns (if the method is NOT void, the return value will already be on the stack)
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
    }
}
