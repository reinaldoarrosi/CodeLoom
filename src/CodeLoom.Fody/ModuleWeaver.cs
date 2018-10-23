using CodeLoom.Aspects;
using Fody;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Cecil.Rocks;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace CodeLoom.Fody
{
    public class ModuleWeaver : BaseModuleWeaver
    {
        internal CodeLoomSetup Setup;
        internal ImplementInterfaceAspectsWeaver ImplementInterfaceAspectsWeaver;
        internal InterceptMethodAspectsWeaver InterceptMethodAspectsWeaver;
        internal InterceptPropertyAspectsWeaver InterceptPropertyAspectsWeaver;

        public override void Execute()
        {
            try
            {
                SetupMissingAssemblyResolutionStrategy();

                LogInfo($"Finding \"CodeLoomSetup\" definition in assembly {ModuleDefinition.Assembly.FullName}.");
                Setup = GetSetup(ModuleDefinition);
                LogInfo("\"CodeLoomSetup\" found.");

                LogInfo("Initializing weavers.");
                ImplementInterfaceAspectsWeaver = new ImplementInterfaceAspectsWeaver(this);
                InterceptMethodAspectsWeaver = new InterceptMethodAspectsWeaver(this);
                InterceptPropertyAspectsWeaver = new InterceptPropertyAspectsWeaver(this);
                LogInfo("Initialization OK.");

                foreach (var type in ModuleDefinition.Assembly.MainModule.Types)
                {
                    WeaveType(type);
                }
            }
            finally
            {
                TeardownMissingAssemblyResolutionStrategy();
            }
        }

        private void WeaveType(TypeDefinition type)
        {
            if (type.IsInterface) return;

            LogInfo($"Weaving type {type.FullName}");

            LogInfo($"Weaving {nameof(InterceptMethodAspect)}");
            InterceptMethodAspectsWeaver.Weave(type);

            LogInfo($"Weaving {nameof(InterceptPropertyAspect)}");
            InterceptPropertyAspectsWeaver.Weave(type);

            LogInfo($"Weaving {nameof(ImplementInterfaceAspect)}");
            ImplementInterfaceAspectsWeaver.Weave(type);

            #region OLD
            //if (Setup.ShouldWeaveType(type.GetSystemType()))
            //{
            //    //var originalFields = type.Fields.ToArray();
            //    var originalProperties = type.Properties.ToArray();
            //    var originalMethods = type.Methods.ToArray();

            //    foreach (var property in originalProperties)
            //    {
            //        LogInfo($"Weaving property {property.FullName}");

            //        var propertyInfo = property.GetPropertyInfo();

            //        if (propertyInfo == null)
            //        {
            //            LogInfo($"Property will not be weaved because its PropertyInfo was not found.");
            //        }
            //        else if (!Setup.ShouldWeaveProperty(propertyInfo))
            //        {
            //            LogInfo($"Property will not be weaved because \"ShouldWeaveProperty\" returned false.");
            //        }
            //        else
            //        {
            //            if (property.GetMethod != null && Setup.ShouldWeavePropertyGetter(propertyInfo))
            //            {
            //                WeaveMethod(property.GetMethod);
            //            }
            //            else
            //            {
            //                LogInfo($"Property Getter will not be weaved because it does not exists or \"ShouldWeavePropertyGetter\" returned false.");
            //            }

            //            if (property.SetMethod != null && Setup.ShouldWeavePropertySetter(propertyInfo))
            //            {
            //                WeaveMethod(property.SetMethod);
            //            }
            //            else
            //            {
            //                LogInfo($"Property Setter will not be weaved because it does not exists or \"ShouldWeavePropertySetter\" returned false.");
            //            }

            //        }
            //    }

            //    foreach (var method in originalMethods)
            //    {
            //        LogInfo($"Weaving method {method.FullName}.");

            //        if (method.IsGetter || method.IsSetter)
            //        {
            //            LogInfo($"Method will not be weaved because it is a Getter/Setter method.");
            //            continue;
            //        }

            //        var methodBase = method.GetMethodBase();
            //        if (methodBase == null)
            //        {
            //            LogInfo($"Method will not be weaved because its MethodBase was not found.");
            //        }
            //        else if (!Setup.ShouldWeaveMethod(methodBase))
            //        {
            //            LogInfo($"Method will not be weaved because \"ShouldWeaveMethod\" returned false.");
            //        }
            //        else
            //        {
            //            WeaveMethod(method);
            //        }
            //    }

            //    //foreach (var field in originalFields)
            //    //{
            //    //    LogInfo($"Weaving field {field.FullName}.");

            //    //    var fieldInfo = field.GetFieldInfo();

            //    //    if (fieldInfo == null)
            //    //    {
            //    //        LogInfo($"Field will not be weaved because its FieldInfo was not found.");
            //    //    }
            //    //    if (!Setup.ShouldWeaveField(fieldInfo))
            //    //    {
            //    //        LogInfo($"Field will not be weaved because \"ShouldWeaveField\" returned false.");
            //    //    }
            //    //    else
            //    //    {
            //    //        WeaveField(field);
            //    //    }
            //    //}
            //}
            //else
            //{
            //    LogInfo($"Type will not be weaved because \"ShouldWeaveType\" returned false.");
            //} 
            #endregion

            foreach (var nestedType in type.NestedTypes)
            {
                WeaveType(nestedType);
            }
        }

        //private void WeaveMethod(MethodDefinition method)
        //{
        //    if (method.IsAbstract) return;

        //    var originalMethodClone = CreateOriginalMethodClone(method);
        //    method.DeclaringType.Methods.Add(originalMethodClone);

        //    var proceedMethod = CreateProceedMethod(method, originalMethodClone);
        //    method.DeclaringType.Methods.Add(proceedMethod);

        //    RewriteOriginalMethod(method, proceedMethod);
        //}

        //private MethodDefinition CreateOriginalMethodClone(MethodDefinition method)
        //{
        //    var attributes = method.Attributes;
        //    attributes &= ~Mono.Cecil.MethodAttributes.Public;
        //    attributes &= ~Mono.Cecil.MethodAttributes.SpecialName;
        //    attributes &= ~Mono.Cecil.MethodAttributes.RTSpecialName;
        //    attributes |= Mono.Cecil.MethodAttributes.Private;

        //    var clone = new MethodDefinition($"$_original_{method.Name}", attributes, method.ReturnType)
        //    {
        //        AggressiveInlining = true,
        //        HasThis = method.HasThis,
        //        ExplicitThis = method.ExplicitThis,
        //        CallingConvention = method.CallingConvention
        //    };
        //    clone.Body.InitLocals = true;

        //    var compilerGeneratedAttrCtor = ModuleDefinition.ImportReference(typeof(CompilerGeneratedAttribute).GetConstructors().First());
        //    clone.CustomAttributes.Add(new CustomAttribute(compilerGeneratedAttrCtor));

        //    foreach (var parameter in method.Parameters)
        //        clone.Parameters.Add(parameter);

        //    foreach (var variable in method.Body.Variables)
        //        clone.Body.Variables.Add(variable);

        //    foreach (var exceptionHandler in method.Body.ExceptionHandlers)
        //        clone.Body.ExceptionHandlers.Add(exceptionHandler);

        //    var ilProcessor = clone.Body.GetILProcessor();
        //    var instructions = method.Body.Instructions.AsEnumerable();
        //    if (method.IsConstructor && !method.IsStatic)
        //    {
        //        // if the method is a ctor, skips instructions up to the first "OpCodes.Call"
        //        // because these instructions represent the call to the base ctor, 
        //        // which will be kept at the original method
        //        instructions = instructions.SkipWhile(i => i.OpCode != OpCodes.Call).Skip(1);
        //    }

        //    foreach (var instruction in instructions)
        //    {
        //        ilProcessor.Append(instruction);
        //    }

        //    if (method.HasGenericParameters)
        //    {
        //        foreach (var genericParameter in method.GenericParameters)
        //            clone.GenericParameters.Add(new GenericParameter(genericParameter.Name, clone));
        //    }

        //    if (method.DebugInformation.HasSequencePoints)
        //    {
        //        foreach (var sequencePoint in method.DebugInformation.SequencePoints)
        //        {
        //            clone.DebugInformation.SequencePoints.Add(sequencePoint);
        //        }
        //    }

        //    clone.DebugInformation.Scope = new ScopeDebugInformation(method.Body.Instructions.First(), method.Body.Instructions.Last());

        //    return clone;
        //}

        //private MethodDefinition CreateProceedMethod(MethodDefinition method, MethodDefinition originalMethodClone)
        //{
        //    var attributes = method.Attributes;
        //    attributes &= ~Mono.Cecil.MethodAttributes.Public;
        //    attributes &= ~Mono.Cecil.MethodAttributes.SpecialName;
        //    attributes &= ~Mono.Cecil.MethodAttributes.RTSpecialName;
        //    attributes |= Mono.Cecil.MethodAttributes.Private;

        //    var proceedMethod = new MethodDefinition($"$_proceed_{method.Name}", attributes, TypeSystem.VoidReference);
        //    proceedMethod.Body.InitLocals = true;

        //    var debuggerStepThroughAttrCtor = ModuleDefinition.ImportReference(typeof(DebuggerStepThroughAttribute).GetConstructors().First());
        //    proceedMethod.CustomAttributes.Add(new CustomAttribute(debuggerStepThroughAttrCtor));

        //    var compilerGeneratedAttrCtor = ModuleDefinition.ImportReference(typeof(CompilerGeneratedAttribute).GetConstructors().First());
        //    proceedMethod.CustomAttributes.Add(new CustomAttribute(compilerGeneratedAttrCtor));

        //    var invocationParameterTypeRef = ModuleDefinition.ImportReference(typeof(Invocation));
        //    var invocationParameter = new ParameterDefinition("invocation", Mono.Cecil.ParameterAttributes.None, invocationParameterTypeRef);
        //    proceedMethod.Parameters.Add(invocationParameter);

        //    if (method.HasGenericParameters)
        //    {
        //        foreach (var genericParameter in method.GenericParameters)
        //        {
        //            proceedMethod.GenericParameters.Add(new GenericParameter(genericParameter.Name, proceedMethod));
        //        }
        //    }

        //    var ilProcessor = proceedMethod.Body.GetILProcessor();
        //    var argumentTypeRef = ModuleDefinition.ImportReference(typeof(Argument));
        //    var argumentsTypeRef = ModuleDefinition.ImportReference(typeof(Arguments));
        //    var getArgumentsMethod = ModuleDefinition.ImportReference(typeof(Invocation).GetProperty(nameof(Invocation.Arguments)).GetMethod);
        //    var argumentsIndexerGetMethod = ModuleDefinition.ImportReference(typeof(Arguments).GetProperties().First(p => p.GetIndexParameters().Select(pi => pi.ParameterType).SequenceEqual(new[] { typeof(int) })).GetMethod);
        //    var getArgumentValueMethod = ModuleDefinition.ImportReference(typeof(Argument).GetProperty(nameof(Argument.Value)).GetMethod);

        //    var tempArgumentsVar = new VariableDefinition(argumentsTypeRef);
        //    proceedMethod.Body.Variables.Add(tempArgumentsVar);
        //    ilProcessor.Append(Instruction.Create(OpCodes.Ldarg, invocationParameter));
        //    ilProcessor.Append(Instruction.Create(OpCodes.Callvirt, getArgumentsMethod));
        //    ilProcessor.Append(Instruction.Create(OpCodes.Stloc, tempArgumentsVar));

        //    var localVars = new List<VariableDefinition>();
        //    foreach (var parameter in method.Parameters)
        //    {
        //        var realParameterType = parameter.ParameterType;
        //        if (realParameterType.IsByReference) realParameterType = (realParameterType as TypeSpecification).ElementType;

        //        var localVar = new VariableDefinition(realParameterType);
        //        localVars.Add(localVar);
        //        proceedMethod.Body.Variables.Add(localVar);

        //        ilProcessor.Append(Instruction.Create(OpCodes.Ldloc, tempArgumentsVar));
        //        ilProcessor.Append(Instruction.Create(OpCodes.Ldc_I4, parameter.Index));
        //        ilProcessor.Append(Instruction.Create(OpCodes.Callvirt, argumentsIndexerGetMethod));
        //        ilProcessor.Append(Instruction.Create(OpCodes.Callvirt, getArgumentValueMethod));

        //        if (realParameterType.IsValueType)
        //            ilProcessor.Append(Instruction.Create(OpCodes.Unbox_Any, realParameterType));
        //        else
        //            ilProcessor.Append(Instruction.Create(OpCodes.Castclass, realParameterType));

        //        ilProcessor.Append(Instruction.Create(OpCodes.Stloc, localVar));
        //    }

        //    // prepares to store the return value into "Invocation.ReturnValue"
        //    if (!originalMethodClone.IsVoidReturnType(TypeSystem)) ilProcessor.Append(Instruction.Create(OpCodes.Ldarg, invocationParameter));

        //    // call the original method
        //    ilProcessor.Append((proceedMethod.IsStatic ? Instruction.Create(OpCodes.Nop) : Instruction.Create(OpCodes.Ldarg_0)));
        //    foreach (var parameter in method.Parameters)
        //    {
        //        if (parameter.ParameterType.IsByReference)
        //            ilProcessor.Append(Instruction.Create(OpCodes.Ldloca, localVars[parameter.Index]));
        //        else
        //            ilProcessor.Append(Instruction.Create(OpCodes.Ldloc, localVars[parameter.Index]));
        //    }
        //    ilProcessor.Append((originalMethodClone.IsStatic ? Instruction.Create(OpCodes.Call, originalMethodClone) : Instruction.Create(OpCodes.Callvirt, originalMethodClone)));

        //    // stores the return value into "Invocation.ReturnValue"
        //    if (!originalMethodClone.IsVoidReturnType(TypeSystem))
        //    {
        //        var setReturnValue = ModuleDefinition.ImportReference(typeof(Invocation).GetProperty(nameof(Invocation.ReturnValue)).SetMethod);
        //        if (originalMethodClone.ReturnType.IsValueType) ilProcessor.Append(Instruction.Create(OpCodes.Box, originalMethodClone.ReturnType));
        //        ilProcessor.Append(Instruction.Create(OpCodes.Callvirt, setReturnValue));
        //    }

        //    var setArgumentValueMethod = ModuleDefinition.ImportReference(typeof(Argument).GetProperty(nameof(Argument.Value)).SetMethod);
        //    foreach (var parameter in method.Parameters)
        //    {
        //        ilProcessor.Append(Instruction.Create(OpCodes.Ldloc, tempArgumentsVar));
        //        ilProcessor.Append(Instruction.Create(OpCodes.Ldc_I4, parameter.Index));
        //        ilProcessor.Append(Instruction.Create(OpCodes.Callvirt, argumentsIndexerGetMethod));

        //        var localVar = localVars[parameter.Index];
        //        ilProcessor.Append(Instruction.Create(OpCodes.Ldloc, localVar));
        //        if (localVar.VariableType.IsValueType) ilProcessor.Append(Instruction.Create(OpCodes.Box, localVar.VariableType));

        //        ilProcessor.Append(Instruction.Create(OpCodes.Callvirt, setArgumentValueMethod));
        //    }

        //    ilProcessor.Append(Instruction.Create(OpCodes.Ret));

        //    return proceedMethod;
        //}

        //private void RewriteOriginalMethod(MethodDefinition method, MethodDefinition proceedMethod)
        //{
        //    List<Instruction> baseCallInstructions = null;
        //    if (method.IsConstructor && !method.IsStatic)
        //    {
        //        // if the method is a constructor stores all instruction up to the first "OpCodes.Call"
        //        // because these instructions represente the call to the base ctor and must stay inside the original ctor
        //        baseCallInstructions = method.Body.Instructions.TakeWhile(i => i.OpCode != OpCodes.Call).ToList();
        //        baseCallInstructions.Add(method.Body.Instructions.Skip(baseCallInstructions.Count).First());
        //    }

        //    method.Body.Variables.Clear();
        //    method.Body.ExceptionHandlers.Clear();
        //    method.Body.Instructions.Clear();
        //    method.DebugInformation.CustomDebugInformations.Clear();
        //    method.DebugInformation.SequencePoints.Clear();
        //    method.DebugInformation.Scope = null;
        //    method.Body.InitLocals = true;

        //    var ilProcessor = method.Body.GetILProcessor();

        //    if (baseCallInstructions != null)
        //    {
        //        // if baseCallInstructions is different from null, adds these instructions to the start of the method
        //        foreach (var instruction in baseCallInstructions) { ilProcessor.Append(instruction); }
        //    }

        //    var argumentTypeRef = ModuleDefinition.ImportReference(typeof(Argument));
        //    var argumentsArrayTypeRef = new ArrayType(argumentTypeRef);
        //    var argumentsArrayVar = new VariableDefinition(argumentsArrayTypeRef);
        //    var argumentCtor = ModuleDefinition.ImportReference(typeof(Argument).GetConstructors().First());
        //    method.Body.Variables.Add(argumentsArrayVar);
        //    ilProcessor.Append(Instruction.Create(OpCodes.Ldc_I4, method.Parameters.Count));
        //    ilProcessor.Append(Instruction.Create(OpCodes.Newarr, argumentTypeRef));
        //    ilProcessor.Append(Instruction.Create(OpCodes.Stloc, argumentsArrayVar));

        //    foreach (var parameter in method.Parameters)
        //    {
        //        ilProcessor.Append(Instruction.Create(OpCodes.Ldloc, argumentsArrayVar));
        //        ilProcessor.Append(Instruction.Create(OpCodes.Ldc_I4, parameter.Index));

        //        ilProcessor.Append(Instruction.Create(OpCodes.Ldc_I4, parameter.Index));
        //        ilProcessor.Append(Instruction.Create(OpCodes.Ldstr, parameter.Name));
        //        ilProcessor.Append(Instruction.Create(OpCodes.Ldarg, parameter));

        //        var realParameterType = parameter.ParameterType;
        //        if (realParameterType.IsByReference)
        //        {
        //            realParameterType = (realParameterType as TypeSpecification).ElementType;

        //            if (realParameterType.IsValueType)
        //                ilProcessor.Append(Instruction.Create(OpCodes.Ldobj, realParameterType));
        //            else
        //                ilProcessor.Append(Instruction.Create(OpCodes.Ldind_Ref));
        //        }
                
        //        if (realParameterType.IsValueType)
        //            ilProcessor.Append(Instruction.Create(OpCodes.Box, realParameterType));

        //        ilProcessor.Append(Instruction.Create(OpCodes.Newobj, argumentCtor));
        //        ilProcessor.Append(Instruction.Create(OpCodes.Stelem_Ref));
        //    }

        //    var invocationTypeRef = ModuleDefinition.ImportReference(typeof(Invocation));
        //    var invocationCtor = ModuleDefinition.ImportReference(typeof(Invocation).GetConstructors().First());
        //    var proceedExecutorCtor = ModuleDefinition.ImportReference(typeof(Action<Invocation>).GetConstructors().First());

        //    var invocationVar = new VariableDefinition(invocationTypeRef);
        //    method.Body.Variables.Add(invocationVar);

        //    var invocationSourceCtor = ModuleDefinition.ImportReference(typeof(InvocationSource).GetConstructors().First());
        //    var getCurrentMethod = ModuleDefinition.ImportReference(typeof(MethodBase).GetMethod(nameof(MethodBase.GetCurrentMethod)));
        //    ilProcessor.Append(Instruction.Create(OpCodes.Call, getCurrentMethod));
        //    ilProcessor.Append((method.IsStatic ? Instruction.Create(OpCodes.Ldnull) : Instruction.Create(OpCodes.Ldarg_0)));
        //    ilProcessor.Append(Instruction.Create(OpCodes.Newobj, invocationSourceCtor));

        //    ilProcessor.Append(Instruction.Create(OpCodes.Ldloc, argumentsArrayVar));

        //    ilProcessor.Append((method.IsStatic ? Instruction.Create(OpCodes.Ldnull) : Instruction.Create(OpCodes.Ldarg_0)));
        //    ilProcessor.Append(Instruction.Create(OpCodes.Ldftn, proceedMethod));
        //    ilProcessor.Append(Instruction.Create(OpCodes.Newobj, proceedExecutorCtor));

        //    ilProcessor.Append(Instruction.Create(OpCodes.Newobj, invocationCtor));
        //    ilProcessor.Append(Instruction.Create(OpCodes.Stloc, invocationVar));

        //    var invokeMethod = ModuleDefinition.ImportReference(typeof(CodeLoom).GetMethod(nameof(CodeLoom.Invoke)));
        //    ilProcessor.Append(Instruction.Create(OpCodes.Ldloc, invocationVar));
        //    ilProcessor.Append(Instruction.Create(OpCodes.Call, invokeMethod));

        //    var getArgumentValueMethod = ModuleDefinition.ImportReference(typeof(Argument).GetProperty(nameof(Argument.Value)).GetMethod);
        //    foreach (var parameter in method.Parameters)
        //    {
        //        if (!parameter.ParameterType.IsByReference) continue;

        //        var realParameterType = (parameter.ParameterType as TypeSpecification).ElementType;
        //        ilProcessor.Append(Instruction.Create(OpCodes.Ldarg, parameter));
        //        ilProcessor.Append(Instruction.Create(OpCodes.Ldloc, argumentsArrayVar));
        //        ilProcessor.Append(Instruction.Create(OpCodes.Ldc_I4, parameter.Index));
        //        ilProcessor.Append(Instruction.Create(OpCodes.Ldelem_Ref));
        //        ilProcessor.Append(Instruction.Create(OpCodes.Callvirt, getArgumentValueMethod));

        //        if (realParameterType.IsValueType)
        //        {
        //            ilProcessor.Append(Instruction.Create(OpCodes.Unbox_Any, realParameterType));
        //            ilProcessor.Append(Instruction.Create(OpCodes.Stobj, realParameterType));
        //        }
        //        else
        //        {
        //            ilProcessor.Append(Instruction.Create(OpCodes.Castclass, realParameterType));
        //            ilProcessor.Append(Instruction.Create(OpCodes.Stind_Ref));
        //        }
        //    }

        //    if(!method.IsVoidReturnType(TypeSystem))
        //    {
        //        var getReturnValueMethod = ModuleDefinition.ImportReference(typeof(Invocation).GetProperty(nameof(Invocation.ReturnValue), BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Instance).GetMethod);
        //        ilProcessor.Append(Instruction.Create(OpCodes.Ldloc, invocationVar));
        //        ilProcessor.Append(Instruction.Create(OpCodes.Callvirt, getReturnValueMethod));

        //        if (method.ReturnType.IsValueType)
        //            ilProcessor.Append(Instruction.Create(OpCodes.Unbox_Any, method.ReturnType));
        //        else
        //            ilProcessor.Append(Instruction.Create(OpCodes.Castclass, method.ReturnType));
        //    }

        //    ilProcessor.Append(Instruction.Create(OpCodes.Ret));
        //}

        private CodeLoomSetup GetSetup(ModuleDefinition moduleDefinition)
        {
            var setupAttr = moduleDefinition.Assembly.CustomAttributes.FirstOrDefault(attr => attr.AttributeType.FullName == typeof(CodeLoomSetupAttribute).FullName);
            var setupType = (setupAttr?.ConstructorArguments.First().Value as TypeReference).Resolve().GetSystemType();
            var setup = Activator.CreateInstance(setupType ?? typeof(CodeLoomSetup.Empty));

            return (CodeLoomSetup)setup;
        }

        public override IEnumerable<string> GetAssembliesForScanning()
        {
            return Enumerable.Empty<string>();
        }

        private void SetupMissingAssemblyResolutionStrategy()
        {
            // Fody already registers a handler for AppDomain.CurrentDomain.AssemblyResolve that uses Assembly.Load(byte[], byte[]) to load assemblies
            // This may cause the same assembly being loaded multiple time. Here we change the event handler using reflection to make our handler run
            // before the one registered by Fody. Our handler makes sure that an assembly is never loaded more than once.
            var field = typeof(AppDomain).GetRuntimeFields().FirstOrDefault(f => f.Name == "_AssemblyResolve");
            var eventDelegate = field.GetValue(AppDomain.CurrentDomain) as MulticastDelegate;
            var handlers = eventDelegate.GetInvocationList().ToList();

            ResolveEventHandler missingAssemblyResolutionCallback = OnResolveAssembly;
            handlers.Insert(0, missingAssemblyResolutionCallback);

            field.SetValue(AppDomain.CurrentDomain, Delegate.Combine(handlers.ToArray()));
        }

        private void TeardownMissingAssemblyResolutionStrategy()
        {
            // After we're done with our module weaver we remove our handler, leaving just the handler added by Fody.
            // This makes sure we won't impact any other weaver that might exist.
            var field = typeof(AppDomain).GetRuntimeFields().FirstOrDefault(f => f.Name == "_AssemblyResolve");
            var eventDelegate = field.GetValue(AppDomain.CurrentDomain) as MulticastDelegate;
            var handlers = eventDelegate.GetInvocationList().ToList();

            handlers.RemoveAt(0);

            field.SetValue(AppDomain.CurrentDomain, Delegate.Combine(handlers.ToArray()));
        }

        private Assembly OnResolveAssembly(object sender, ResolveEventArgs args)
        {
            // This is our custom missing assembly handler. First it checks to see if an assembly with the same name is already loaded.
            var alreadyLoadedAssembly = AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(a => a.FullName == args.Name);
            if (alreadyLoadedAssembly != null) return alreadyLoadedAssembly;

            // If it doesn't find a loaded assembly, it tries to find a .dll or .exe in one of the paths referenced by the assembly that is being weaved.
            var missingAssemblyDLLFileName = args.Name.Split(',').First() + ".dll";
            var missingAssemblyEXEFileName = args.Name.Split(',').First() + ".exe";
            var referencedAssemblies = new[] { AssemblyFilePath }.Union(References.Split(';'));

            foreach (var referencedAssembly in referencedAssemblies)
            {
                var referencedAssemblyFileName = Path.GetFileName(referencedAssembly);
                if (referencedAssemblyFileName == missingAssemblyDLLFileName || referencedAssemblyFileName == missingAssemblyEXEFileName)
                {
                    var assemblyBytes = File.ReadAllBytes(referencedAssembly);
                    var assembly = Assembly.Load(assemblyBytes);

                    return assembly;
                }
            }

            return null;
        }
    }
}
