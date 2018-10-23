using CodeLoom.Aspects;
using CodeLoom.Contexts;
using CodeLoom.Pipelines;
using Mono.Cecil;
using Mono.Cecil.Cil;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

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

        public virtual void Weave(TypeDefinition typeDefinition)
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

        protected virtual void WeaveTypeMethods(TypeDefinition typeDefinition)
        {
            var pipelineRunMethodRef = ModuleDefinition.ImportReference(typeof(InterceptMethodPipeline).GetMethod(nameof(InterceptMethodPipeline.Run)));
            var currentTypeDefinition = typeDefinition;

            while (currentTypeDefinition != null)
            {
                var currentTypeDefinitionMethods = currentTypeDefinition.Methods.ToArray();

                foreach (var methodDefinition in currentTypeDefinitionMethods)
                {
                    if (methodDefinition.IsGetter || methodDefinition.IsSetter || methodDefinition.IsConstructor)
                        continue;

                    var method = methodDefinition.TryGetMethodBase();
                    if (method == null)
                    {
                        ModuleWeaver.LogInfo($"Method {methodDefinition.Name} from type {typeDefinition.FullName} will not be weaved because it was not possible to load its corresponding MethodBase");
                        continue;
                    }

                    if (method.IsAbstract)
                    {
                        ModuleWeaver.LogInfo($"Method {methodDefinition.Name} from type {typeDefinition.FullName} will not be weaved because it is an abstract method");
                        continue;
                    }

                    if (WeavedMethods.ContainsKey(method))
                        continue;

                    WeavedMethods.Add(method, true);

                    var aspects = ModuleWeaver.Setup.GetAspects(method).ToArray();
                    if (aspects.Length <= 0)
                    {
                        ModuleWeaver.LogInfo($"Method {methodDefinition.Name} from type {typeDefinition.FullName} will not be weaved because no aspect was applied to it");
                        continue;
                    }

                    if (!methodDefinition.HasThis)
                        AddStaticCtorIfNeeded(typeDefinition);

                    var aspectField = AddAspectField(typeDefinition, methodDefinition);

                    var clonedMethod = CloneMethod(typeDefinition, methodDefinition);
                    typeDefinition.Methods.Add(clonedMethod);

                    var proceedMethod = CreateProceedMethod(typeDefinition, clonedMethod);
                    typeDefinition.Methods.Add(proceedMethod);

                    RewriteOriginalMethod(typeDefinition, methodDefinition, aspectField, pipelineRunMethodRef);
                    AddAspectFieldInitialization(typeDefinition, aspectField, aspects, proceedMethod, method.IsStatic);
                }

                currentTypeDefinition = currentTypeDefinition?.BaseType?.Resolve();
            }
        }

        protected virtual void AddStaticCtorIfNeeded(TypeDefinition typeDefinition)
        {
            var existingStaticCtor = typeDefinition.Methods.FirstOrDefault(c => c.IsConstructor && c.IsStatic);
            if (existingStaticCtor != null) return;

            MethodAttributes attributes = MethodAttributes.Private | MethodAttributes.HideBySig | MethodAttributes.SpecialName | MethodAttributes.RTSpecialName | MethodAttributes.Static;
            MethodDefinition staticCtor = new MethodDefinition(".cctor", attributes, ModuleWeaver.TypeSystem.VoidReference);
            staticCtor.Body.InitLocals = true;
            typeDefinition.Methods.Add(staticCtor);

            var ilProcessor = staticCtor.Body.GetILProcessor();
            ilProcessor.Append(Instruction.Create(OpCodes.Ret));
        }

        protected virtual FieldDefinition AddAspectField(TypeDefinition typeDefinition, MethodDefinition methodDefinition)
        {
            // Import the type reference for InterceptMethodPipeline
            var pipelineTypeRef = ModuleDefinition.ImportReference(typeof(InterceptMethodPipeline));

            // Creates a private field which will contain an instance of InterceptMethodPipeline
            // This instance is responsible for coordinating the invocation of the multiple aspects applied to a method
            var attributes = FieldAttributes.Private;
            if (methodDefinition.IsStatic) attributes = attributes | FieldAttributes.Static;
            var fieldName = Helpers.GetUniqueFieldName(typeDefinition, methodDefinition.Name);
            var fieldDefinition = new FieldDefinition(fieldName, attributes, pipelineTypeRef);
            typeDefinition.Fields.Add(fieldDefinition);

            return fieldDefinition;
        }

        protected virtual void AddAspectFieldInitialization(TypeDefinition typeDefinition, FieldDefinition aspectField, InterceptMethodAspect[] aspects, MethodDefinition proceed, bool isStatic)
        {
            // Import the ctor reference for InterceptMethodPipeline
            var pipelineTypeRef = ModuleDefinition.ImportReference(typeof(InterceptMethodPipeline));
            var pipelineTypeDef = pipelineTypeRef.Resolve();
            var pipelineCtor = ModuleDefinition.ImportReference(pipelineTypeDef.Methods.First(m => m.IsConstructor && !m.IsStatic));

            // Import types that will be used to initialize the aspect field
            var baseAspectTypeRef = ModuleDefinition.ImportReference(typeof(InterceptMethodAspect));
            var contextTypeRef = ModuleDefinition.ImportReference(typeof(MethodContext));
            var actionTypeRef = ModuleDefinition.ImportReference(typeof(Action<>));
            var originalMethodTypeRef = new GenericInstanceType(actionTypeRef);
            originalMethodTypeRef.GenericArguments.Add(contextTypeRef);
            var actionCtor = ModuleDefinition.ImportReference(typeof(Action<>).GetConstructors().First());
            actionCtor.DeclaringType = originalMethodTypeRef;

            // Adds the field initialization code to every ctor present in the type that is being weaved
            // This is equivalent to initializing a field directly in its declaration e.g. "private MyAspect _aspect = new MyAspect();"
            foreach (var ctor in typeDefinition.Methods.Where(m => m.IsConstructor && ((!isStatic && !m.IsStatic) || (isStatic && m.IsStatic))))
            {
                List<Instruction> instructions = new List<Instruction>();

                var aspectsArrayVar = new VariableDefinition(new ArrayType(baseAspectTypeRef));
                ctor.Body.Variables.Add(aspectsArrayVar);
                ctor.Body.InitLocals = true;
                instructions.Add(Instruction.Create(OpCodes.Ldc_I4, aspects.Length));
                instructions.Add(Instruction.Create(OpCodes.Newarr, baseAspectTypeRef));
                instructions.Add(Instruction.Create(OpCodes.Stloc, aspectsArrayVar));
                for (int i = 0; i < aspects.Length; i++)
                {
                    var aspect = aspects[i];
                    var aspectTypeRef = ModuleDefinition.ImportReference(aspect.GetType());
                    var aspectTypeDef = aspectTypeRef.Resolve();
                    var aspectCtor = ModuleDefinition.ImportReference(aspectTypeDef.Methods.FirstOrDefault(m => m.IsConstructor && !m.IsStatic && !m.HasParameters));
                    aspectCtor.DeclaringType = aspectTypeRef;

                    instructions.Add(Instruction.Create(OpCodes.Ldloc, aspectsArrayVar));
                    instructions.Add(Instruction.Create(OpCodes.Ldc_I4, i));
                    instructions.Add(Instruction.Create(OpCodes.Newobj, aspectCtor));
                    instructions.Add(Instruction.Create(OpCodes.Stelem_Ref));
                }

                // loads "this" into the stack (this is necessary so that we can call Stfld to set the aspect field value)
                if (!isStatic) instructions.Add(Instruction.Create(OpCodes.Ldarg_0));

                // loads the aspectsArrayVar into the stack (this is used to create the InterceptMethodPipeline instance)
                instructions.Add(Instruction.Create(OpCodes.Ldloc, aspectsArrayVar));

                // creates the Action<MethodContext> delegate that points to the proceed getter method (this is used to create the InterceptMethodPipeline instance)
                if (proceed != null)
                {
                    if (!isStatic)
                        instructions.Add(Instruction.Create(OpCodes.Ldarg_0));
                    else
                        instructions.Add(Instruction.Create(OpCodes.Ldnull));

                    instructions.Add(Instruction.Create(OpCodes.Ldftn, proceed));
                    instructions.Add(Instruction.Create(OpCodes.Newobj, actionCtor));
                }
                else
                {
                    instructions.Add(Instruction.Create(OpCodes.Ldnull));
                }

                // creates the InterceptMethodPipeline instance
                instructions.Add(Instruction.Create(OpCodes.Newobj, pipelineCtor));

                // sets the aspect field
                if (!isStatic)
                    instructions.Add(Instruction.Create(OpCodes.Stfld, aspectField));
                else
                    instructions.Add(Instruction.Create(OpCodes.Stsfld, aspectField));

                // finds the insertion point and inserts the field initialization code at this point
                var ilProcessor = ctor.Body.GetILProcessor();
                if (!ctor.IsStatic)
                {
                    // if it's not a static ctor, skips until the first OpCodes.Call
                    // this is because this OpCodes.Call represents the call to the base classe ctor, and must stay where it is
                    var insertionPoint = ctor.Body.Instructions.SkipWhile(i => i.OpCode != OpCodes.Call).First();
                    ilProcessor.InsertAfter(insertionPoint, instructions);
                }
                else
                {
                    // if it's a static ctor, inserts right at the start of the method
                    var insertionPoint = ctor.Body.Instructions.First();
                    ilProcessor.InsertBefore(insertionPoint, instructions);
                }
            }
        }

        protected virtual MethodDefinition CloneMethod(TypeDefinition typeDefinition, MethodDefinition originalMethod)
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

            foreach (var parameter in originalMethod.Parameters)
                clone.Parameters.Add(parameter);

            foreach (var variable in originalMethod.Body.Variables)
                clone.Body.Variables.Add(variable);

            foreach (var exceptionHandler in originalMethod.Body.ExceptionHandlers)
                clone.Body.ExceptionHandlers.Add(exceptionHandler);

            if (originalMethod.HasGenericParameters)
            {
                foreach (var genericParameter in originalMethod.GenericParameters)
                    clone.GenericParameters.Add(new GenericParameter(genericParameter.Name, clone));
            }

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

        protected virtual MethodDefinition CreateProceedMethod(TypeDefinition typeDefinition, MethodDefinition originalMethodClone)
        {
            var attributes = originalMethodClone.Attributes;
            attributes &= ~MethodAttributes.Public;
            attributes &= ~MethodAttributes.SpecialName;
            attributes &= ~MethodAttributes.RTSpecialName;
            attributes |= MethodAttributes.Private;

            var methodName = Helpers.GetUniqueMethodName(typeDefinition, $"proceed_{originalMethodClone.Name}");
            var proceedMethod = new MethodDefinition(methodName, attributes, ModuleWeaver.TypeSystem.VoidReference);
            proceedMethod.Body.InitLocals = true;

            var debuggerStepThroughAttrCtor = ModuleDefinition.ImportReference(typeof(DebuggerStepThroughAttribute).GetConstructors().First());
            proceedMethod.CustomAttributes.Add(new CustomAttribute(debuggerStepThroughAttrCtor));

            var compilerGeneratedAttrCtor = ModuleDefinition.ImportReference(typeof(CompilerGeneratedAttribute).GetConstructors().First());
            proceedMethod.CustomAttributes.Add(new CustomAttribute(compilerGeneratedAttrCtor));

            var contextParameterTypeRef = ModuleDefinition.ImportReference(typeof(MethodContext));
            var contextParameter = new ParameterDefinition("context", ParameterAttributes.None, contextParameterTypeRef);
            proceedMethod.Parameters.Add(contextParameter);

            if (originalMethodClone.HasGenericParameters)
            {
                foreach (var genericParameter in originalMethodClone.GenericParameters)
                {
                    proceedMethod.GenericParameters.Add(new GenericParameter(genericParameter.Name, proceedMethod));
                }
            }

            var argumentsTypeRef = ModuleDefinition.ImportReference(typeof(Arguments));
            var argumentsVar = new VariableDefinition(argumentsTypeRef);
            proceedMethod.Body.Variables.Add(argumentsVar);

            var getArgumentsMethod = ModuleDefinition.ImportReference(typeof(MethodContext).GetProperty(nameof(MethodContext.Arguments)).GetMethod);
            var getArgumentValueMethod = ModuleDefinition.ImportReference(typeof(Arguments).GetMethod(nameof(Arguments.GetArgumentValue), new Type[] { typeof(int) }));
            var setArgumentValueMethod = ModuleDefinition.ImportReference(typeof(Arguments).GetMethod(nameof(Arguments.SetArgumentValue), new Type[] { typeof(int), typeof(object) }));

            var ilProcessor = proceedMethod.Body.GetILProcessor();

            // gets the Argument property from the MethodContext parameter
            ilProcessor.Append(Instruction.Create(OpCodes.Ldarg, contextParameter));
            ilProcessor.Append(Instruction.Create(OpCodes.Callvirt, getArgumentsMethod));
            ilProcessor.Append(Instruction.Create(OpCodes.Stloc, argumentsVar));

            // loads the arguments into local variables
            var localParametersVariables = new List<VariableDefinition>();
            foreach (var parameter in originalMethodClone.Parameters)
            {
                var realParameterType = parameter.ParameterType;
                if (realParameterType.IsByReference) realParameterType = (realParameterType as TypeSpecification).ElementType;

                var parameterVar = new VariableDefinition(realParameterType);
                localParametersVariables.Add(parameterVar);
                proceedMethod.Body.Variables.Add(parameterVar);

                ilProcessor.Append(Instruction.Create(OpCodes.Ldloc, argumentsVar));
                ilProcessor.Append(Instruction.Create(OpCodes.Ldc_I4, parameter.Index));
                ilProcessor.Append(Instruction.Create(OpCodes.Callvirt, getArgumentValueMethod));

                if (realParameterType.IsValueType)
                    ilProcessor.Append(Instruction.Create(OpCodes.Unbox_Any, realParameterType));
                else
                    ilProcessor.Append(Instruction.Create(OpCodes.Castclass, realParameterType));

                ilProcessor.Append(Instruction.Create(OpCodes.Stloc, parameterVar));
            }

            // prepares to call the original method, loading "this" and the local variables into the stack
            if (originalMethodClone.HasThis)
                ilProcessor.Append(Instruction.Create(OpCodes.Ldarg_0));

            foreach (var parameter in originalMethodClone.Parameters)
            {
                if (parameter.ParameterType.IsByReference)
                    ilProcessor.Append(Instruction.Create(OpCodes.Ldloca, localParametersVariables[parameter.Index]));
                else
                    ilProcessor.Append(Instruction.Create(OpCodes.Ldloc, localParametersVariables[parameter.Index]));
            }

            // calls the original method
            if (originalMethodClone.HasThis)
                ilProcessor.Append(Instruction.Create(OpCodes.Callvirt, originalMethodClone));
            else
                ilProcessor.Append(Instruction.Create(OpCodes.Call, originalMethodClone));

            // set the MethodContext.ReturnValue with the value returned from the originalMethod
            if (!originalMethodClone.ReturnType.SameTypeAs(ModuleWeaver.TypeSystem.VoidReference))
            {
                // stores the originalMethod return value into a local variable
                var returnValueVar = new VariableDefinition(originalMethodClone.ReturnType);
                proceedMethod.Body.Variables.Add(returnValueVar);
                ilProcessor.Append(Instruction.Create(OpCodes.Stloc, returnValueVar));

                // sets the value from the local variable into the MethodContext.ReturnValue
                var setReturnValue = ModuleDefinition.ImportReference(typeof(MethodContext).GetMethod(nameof(MethodContext.SetReturnValue), new Type[] { typeof(object) }));
                ilProcessor.Append(Instruction.Create(OpCodes.Ldarg, contextParameter));
                ilProcessor.Append(Instruction.Create(OpCodes.Ldloc, returnValueVar));

                if (returnValueVar.VariableType.IsValueType)
                    ilProcessor.Append(Instruction.Create(OpCodes.Box, returnValueVar.VariableType));

                ilProcessor.Append(Instruction.Create(OpCodes.Callvirt, setReturnValue));
            }

            // sets the values of the arguments back into the MethodContext, because they can be modified when they're "out" or "ref" parameters
            foreach (var parameter in originalMethodClone.Parameters)
            {
                if (!parameter.ParameterType.IsByReference) continue;

                var localParameterVar = localParametersVariables[parameter.Index];
                ilProcessor.Append(Instruction.Create(OpCodes.Ldloc, argumentsVar));
                ilProcessor.Append(Instruction.Create(OpCodes.Ldc_I4, parameter.Index));
                ilProcessor.Append(Instruction.Create(OpCodes.Ldloc, localParameterVar));

                if (localParameterVar.VariableType.IsValueType)
                    ilProcessor.Append(Instruction.Create(OpCodes.Box, localParameterVar.VariableType));

                ilProcessor.Append(Instruction.Create(OpCodes.Callvirt, setArgumentValueMethod));
            }

            ilProcessor.Append(Instruction.Create(OpCodes.Ret));

            return proceedMethod;
        }

        protected virtual void RewriteOriginalMethod(TypeDefinition typeDefinition, MethodDefinition originalMethod, FieldDefinition aspectField, MethodReference aspectRunMethod)
        {
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

            // if baseCallInstructions is different from null, adds these instructions to the start of the method
            if (baseCallInstructions != null)
            {
                foreach (var instruction in baseCallInstructions)
                {
                    ilProcessor.Append(instruction); 
                }
            }

            // creates a string[] containing the name of the parameters (this will be used to instantiate Arguments)
            var parameterNamesArrayVar = new VariableDefinition(new ArrayType(ModuleWeaver.TypeSystem.StringReference));
            originalMethod.Body.Variables.Add(parameterNamesArrayVar);
            ilProcessor.Append(Instruction.Create(OpCodes.Ldc_I4, originalMethod.Parameters.Count));
            ilProcessor.Append(Instruction.Create(OpCodes.Newarr, ModuleWeaver.TypeSystem.StringReference));
            foreach (var parameter in originalMethod.Parameters)
            {
                ilProcessor.Append(Instruction.Create(OpCodes.Dup));
                ilProcessor.Append(Instruction.Create(OpCodes.Ldc_I4, parameter.Index));
                ilProcessor.Append(Instruction.Create(OpCodes.Ldstr, parameter.Name));
                ilProcessor.Append(Instruction.Create(OpCodes.Stelem_Ref));
            }
            ilProcessor.Append(Instruction.Create(OpCodes.Stloc, parameterNamesArrayVar));

            // creates a Type[] containing the Type of the parameters (this will be used to instantiate Arguments)
            var typeTypeRef = ModuleDefinition.ImportReference(typeof(Type));
            var parameterTypesArrayVar = new VariableDefinition(new ArrayType(typeTypeRef));
            var getTypeFromHandleMethodRef = ModuleDefinition.ImportReference(typeof(Type).GetMethod("GetTypeFromHandle"));
            originalMethod.Body.Variables.Add(parameterTypesArrayVar);
            ilProcessor.Append(Instruction.Create(OpCodes.Ldc_I4, originalMethod.Parameters.Count));
            ilProcessor.Append(Instruction.Create(OpCodes.Newarr, typeTypeRef));
            foreach (var parameter in originalMethod.Parameters)
            {
                ilProcessor.Append(Instruction.Create(OpCodes.Dup));
                ilProcessor.Append(Instruction.Create(OpCodes.Ldc_I4, parameter.Index));
                ilProcessor.Append(Instruction.Create(OpCodes.Ldtoken, parameter.ParameterType));
                ilProcessor.Append(Instruction.Create(OpCodes.Call, getTypeFromHandleMethodRef));
                ilProcessor.Append(Instruction.Create(OpCodes.Stelem_Ref));
            }
            ilProcessor.Append(Instruction.Create(OpCodes.Stloc, parameterTypesArrayVar));

            // creates an object[] containing the value of the parameters (this will be used to instantiate Arguments)
            var parameterValuesArrayVar = new VariableDefinition(new ArrayType(ModuleWeaver.TypeSystem.ObjectReference));
            originalMethod.Body.Variables.Add(parameterValuesArrayVar);
            ilProcessor.Append(Instruction.Create(OpCodes.Ldc_I4, originalMethod.Parameters.Count));
            ilProcessor.Append(Instruction.Create(OpCodes.Newarr, ModuleWeaver.TypeSystem.ObjectReference));
            foreach (var parameter in originalMethod.Parameters)
            {
                ilProcessor.Append(Instruction.Create(OpCodes.Dup));
                ilProcessor.Append(Instruction.Create(OpCodes.Ldc_I4, parameter.Index));
                ilProcessor.Append(Instruction.Create(OpCodes.Ldarg, parameter));

                if (parameter.ParameterType.IsValueType)
                    ilProcessor.Append(Instruction.Create(OpCodes.Box, parameter.ParameterType));

                ilProcessor.Append(Instruction.Create(OpCodes.Stelem_Ref));
            }
            ilProcessor.Append(Instruction.Create(OpCodes.Stloc, parameterValuesArrayVar));

            // creates the Arguments instance that will be used to create the MethodContext
            var argumentsTypeRef = ModuleDefinition.ImportReference(typeof(Arguments));
            var argumentsCtor = ModuleDefinition.ImportReference(typeof(Arguments).GetConstructors().First());
            var argumentsVar = new VariableDefinition(argumentsTypeRef);
            originalMethod.Body.Variables.Add(argumentsVar);
            ilProcessor.Append(Instruction.Create(OpCodes.Ldloc, parameterNamesArrayVar));
            ilProcessor.Append(Instruction.Create(OpCodes.Ldloc, parameterTypesArrayVar));
            ilProcessor.Append(Instruction.Create(OpCodes.Ldloc, parameterValuesArrayVar));
            ilProcessor.Append(Instruction.Create(OpCodes.Newobj, argumentsCtor));
            ilProcessor.Append(Instruction.Create(OpCodes.Stloc, argumentsVar));

            // creates the MethodContext instance
            var contextTypeRef = ModuleDefinition.ImportReference(typeof(MethodContext));
            var contextCtor = ModuleDefinition.ImportReference(typeof(MethodContext).GetConstructors().First());
            var contextVar = new VariableDefinition(contextTypeRef);
            originalMethod.Body.Variables.Add(contextVar);
            if (originalMethod.HasThis) ilProcessor.Append(Instruction.Create(OpCodes.Ldarg_0));
            if (!originalMethod.HasThis) ilProcessor.Append(Instruction.Create(OpCodes.Ldnull));
            ilProcessor.Append(Instruction.Create(OpCodes.Ldloc, argumentsVar));
            ilProcessor.Append(Instruction.Create(OpCodes.Newobj, contextCtor));
            ilProcessor.Append(Instruction.Create(OpCodes.Stloc, contextVar));

            // invoke the aspects
            if (!aspectField.IsStatic)
            {
                ilProcessor.Append(Instruction.Create(OpCodes.Ldarg_0));
                ilProcessor.Append(Instruction.Create(OpCodes.Ldfld, aspectField));
            }
            else
            {
                ilProcessor.Append(Instruction.Create(OpCodes.Ldsfld, aspectField));
            }

            ilProcessor.Append(Instruction.Create(OpCodes.Ldloc, contextVar));
            ilProcessor.Append(Instruction.Create(OpCodes.Callvirt, aspectRunMethod));

            // prepares the return value
            if (!originalMethod.ReturnType.SameTypeAs(ModuleWeaver.TypeSystem.VoidReference))
            {
                var getReturnValue = ModuleDefinition.ImportReference(typeof(MethodContext).GetProperty(nameof(MethodContext.ReturnValue)).GetMethod);
                ilProcessor.Append(Instruction.Create(OpCodes.Ldloc, contextVar));
                ilProcessor.Append(Instruction.Create(OpCodes.Callvirt, getReturnValue));

                if (originalMethod.ReturnType.IsValueType)
                    ilProcessor.Append(Instruction.Create(OpCodes.Unbox_Any, originalMethod.ReturnType));
                else
                    ilProcessor.Append(Instruction.Create(OpCodes.Castclass, originalMethod.ReturnType));
            }

            ilProcessor.Append(Instruction.Create(OpCodes.Ret));
        }
    }
}
