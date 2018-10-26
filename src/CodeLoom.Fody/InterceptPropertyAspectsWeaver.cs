using CodeLoom.Aspects;
using CodeLoom.Contexts;
using CodeLoom.Bindings;
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
    class InterceptPropertyAspectsWeaver
    {
        
        internal ModuleWeaver ModuleWeaver;

        public InterceptPropertyAspectsWeaver(ModuleWeaver moduleWeaver)
        {
            ModuleWeaver = moduleWeaver;
            WeavedProperties = new Dictionary<System.Reflection.PropertyInfo, bool>();
        }

        internal ModuleDefinition ModuleDefinition { get { return ModuleWeaver.ModuleDefinition; } }
        internal Dictionary<System.Reflection.PropertyInfo, bool> WeavedProperties { get; set; }

        public void Weave(TypeDefinition typeDefinition)
        {
            //var type = typeDefinition.TryGetSystemType();
            //if (type == null)
            //{
            //    ModuleWeaver.LogInfo($"Type {typeDefinition.FullName} will not be weaved because it was not possible to load its corresponding System.Type");
            //    return;
            //}

            //if (type.IsInterface)
            //{
            //    ModuleWeaver.LogInfo($"Type {typeDefinition.FullName} will not be weaved because it is an interface");
            //    return;
            //}

            //WeaveTypeProperties(typeDefinition);
        }

        /*
        private void WeaveTypeProperties(TypeDefinition typeDefinition)
        {
            var pipelineRunGetterMethodRef = ModuleDefinition.ImportReference(typeof(InterceptPropertyPipeline).GetMethod(nameof(InterceptPropertyPipeline.RunGetter)));
            var pipelineRunSetterMethodRef = ModuleDefinition.ImportReference(typeof(InterceptPropertyPipeline).GetMethod(nameof(InterceptPropertyPipeline.RunSetter)));
            var properties = typeDefinition.Properties.ToArray();

            foreach (var propertyDefinition in properties)
            {
                var property = propertyDefinition.GetPropertyInfo();
                if (property == null)
                {
                    ModuleWeaver.LogInfo($"Property {propertyDefinition.Name} from type {typeDefinition.FullName} will not be weaved because it was not possible to load its corresponding PropertyInfo");
                    continue;
                }

                if (WeavedProperties.ContainsKey(property))
                    continue;

                WeavedProperties.Add(property, true);

                var aspects = ModuleWeaver.Setup.GetAspects(property).ToArray();
                if (aspects.Length <= 0)
                {
                    ModuleWeaver.LogInfo($"Property {propertyDefinition.Name} from type {typeDefinition.FullName} will not be weaved because no aspect was applied to it");
                    continue;
                }

                if (!propertyDefinition.HasThis)
                    AddStaticCtorIfNeeded(typeDefinition);

                MethodDefinition propertyGetterProceed = null;
                MethodDefinition propertySetterProceed = null;
                FieldDefinition aspectField = AddAspectField(typeDefinition, propertyDefinition);

                if (property.GetMethod != null && !property.GetMethod.IsAbstract)
                {
                    propertyGetterProceed = CreatePropertyGetterProceedMethod(typeDefinition, propertyDefinition);
                    RewriteOriginalMethod(typeDefinition, propertyDefinition, propertyDefinition.GetMethod, aspectField, pipelineRunGetterMethodRef);
                }

                if (property.SetMethod != null && !property.SetMethod.IsAbstract)
                {
                    propertySetterProceed = CreatePropertySetterProceedMethod(typeDefinition, propertyDefinition);
                    RewriteOriginalMethod(typeDefinition, propertyDefinition, propertyDefinition.SetMethod, aspectField, pipelineRunSetterMethodRef);
                }

                AddAspectFieldInitialization(typeDefinition, aspectField, aspects, propertyGetterProceed, propertySetterProceed, !propertyDefinition.HasThis);
            }
        }

        private void AddStaticCtorIfNeeded(TypeDefinition typeDefinition)
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

        private FieldDefinition AddAspectField(TypeDefinition typeDefinition, PropertyDefinition propertyDefinition)
        {
            // Import the type reference for InterceptPropertyPipeline
            var pipelineTypeRef = ModuleDefinition.ImportReference(typeof(InterceptPropertyPipeline));

            // Creates a private field which will contain an instance of InterceptPropertyPipeline
            // This instance is responsible for coordinating the invocation of the multiple aspects applied to a property
            var attributes = FieldAttributes.Private;
            if (!propertyDefinition.HasThis) attributes = attributes | FieldAttributes.Static;
            var fieldName = Helpers.GetUniqueFieldName(typeDefinition, propertyDefinition.Name);
            var fieldDefinition = new FieldDefinition(fieldName, attributes, pipelineTypeRef);
            typeDefinition.Fields.Add(fieldDefinition);

            return fieldDefinition;
        }

        private void AddAspectFieldInitialization(TypeDefinition typeDefinition, FieldDefinition aspectField, InterceptPropertyAspect[] aspects, MethodDefinition propertyGetterProceed, MethodDefinition propertySetterProceed, bool isStatic)
        {
            // Import the ctor reference for InterceptPropertyPipeline
            var pipelineTypeRef = ModuleDefinition.ImportReference(typeof(InterceptPropertyPipeline));
            var pipelineTypeDef = pipelineTypeRef.Resolve();
            var pipelineCtor = ModuleDefinition.ImportReference(pipelineTypeDef.Methods.First(m => m.IsConstructor && !m.IsStatic));

            // Import types that will be used to initialize the aspect field
            var baseAspectTypeRef = ModuleDefinition.ImportReference(typeof(InterceptPropertyAspect));
            var contextTypeRef = ModuleDefinition.ImportReference(typeof(PropertyContext));
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

                // loads the aspectsArrayVar into the stack (this is used to create the InterceptPropertyPipeline instance)
                instructions.Add(Instruction.Create(OpCodes.Ldloc, aspectsArrayVar));

                // creates the Action<PropertyContext> delegate that points to the proceed getter method (this is used to create the InterceptPropertyPipeline instance)
                if (propertyGetterProceed != null)
                {
                    if (!isStatic)
                        instructions.Add(Instruction.Create(OpCodes.Ldarg_0));
                    else
                        instructions.Add(Instruction.Create(OpCodes.Ldnull));

                    instructions.Add(Instruction.Create(OpCodes.Ldftn, propertyGetterProceed));
                    instructions.Add(Instruction.Create(OpCodes.Newobj, actionCtor));
                }
                else
                {
                    instructions.Add(Instruction.Create(OpCodes.Ldnull));
                }

                // creates the Action<PropertyContext> delegate that points to the proceed setter method (this is used to create the InterceptPropertyPipeline instance)
                if (propertySetterProceed != null)
                {
                    if (!isStatic)
                        instructions.Add(Instruction.Create(OpCodes.Ldarg_0));
                    else
                        instructions.Add(Instruction.Create(OpCodes.Ldnull));

                    instructions.Add(Instruction.Create(OpCodes.Ldftn, propertySetterProceed));
                    instructions.Add(Instruction.Create(OpCodes.Newobj, actionCtor));
                }
                else
                {
                    instructions.Add(Instruction.Create(OpCodes.Ldnull));
                }

                // creates the InterceptPropertyPipeline instance
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

        private MethodDefinition CreatePropertyGetterProceedMethod(TypeDefinition typeDefinition, PropertyDefinition property)
        {
            var clonedMethod = CloneMethod(typeDefinition, property, property.GetMethod);
            typeDefinition.Methods.Add(clonedMethod);

            var proceedMethod = CreateProceedMethod(typeDefinition, property, clonedMethod);
            typeDefinition.Methods.Add(proceedMethod);

            return proceedMethod;
        }

        private MethodDefinition CreatePropertySetterProceedMethod(TypeDefinition typeDefinition, PropertyDefinition property)
        {
            var clonedMethod = CloneMethod(typeDefinition, property, property.GetMethod);
            typeDefinition.Methods.Add(clonedMethod);

            var proceedMethod = CreateProceedMethod(typeDefinition, property, clonedMethod);
            typeDefinition.Methods.Add(proceedMethod);

            return proceedMethod;
        }

        private MethodDefinition CloneMethod(TypeDefinition typeDefinition, PropertyDefinition propertyDefinition, MethodDefinition originalMethod)
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

        private MethodDefinition CreateProceedMethod(TypeDefinition typeDefinition, PropertyDefinition propertyDefinition, MethodDefinition originalMethodClone)
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

            var contextParameterTypeRef = ModuleDefinition.ImportReference(typeof(PropertyContext));
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

            var getArgumentsMethod = ModuleDefinition.ImportReference(typeof(PropertyContext).GetProperty(nameof(PropertyContext.Arguments)).GetMethod);
            var getArgumentValueMethod = ModuleDefinition.ImportReference(typeof(Arguments).GetMethod(nameof(Arguments.GetArgument), new Type[] { typeof(int) }));
            var setArgumentValueMethod = ModuleDefinition.ImportReference(typeof(Arguments).GetMethod(nameof(Arguments.SetArgument), new Type[] { typeof(int), typeof(object) }));

            var ilProcessor = proceedMethod.Body.GetILProcessor();

            // gets the Argument property from the PropertyContext parameter
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

            // set the PropertyContext.ReturnValue with the value returned from the originalMethod
            if (!originalMethodClone.ReturnType.SameTypeAs(ModuleWeaver.TypeSystem.VoidReference))
            {
                // stores the originalMethod return value into a local variable
                var returnValueVar = new VariableDefinition(originalMethodClone.ReturnType);
                proceedMethod.Body.Variables.Add(returnValueVar);
                ilProcessor.Append(Instruction.Create(OpCodes.Stloc, returnValueVar));

                // sets the value from the local variable into the PropertyContext.ReturnValue
                var setReturnValue = ModuleDefinition.ImportReference(typeof(PropertyContext).GetMethod(nameof(PropertyContext.SetReturnValue), new Type[] { typeof(object) }));
                ilProcessor.Append(Instruction.Create(OpCodes.Ldarg, contextParameter));
                ilProcessor.Append(Instruction.Create(OpCodes.Ldloc, returnValueVar));

                if (returnValueVar.VariableType.IsValueType)
                    ilProcessor.Append(Instruction.Create(OpCodes.Box, returnValueVar.VariableType));

                ilProcessor.Append(Instruction.Create(OpCodes.Callvirt, setReturnValue));
            }

            // sets the values of the arguments back into the PropertyContext, because they can be modified when they're "out" or "ref" parameters
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

        private void RewriteOriginalMethod(TypeDefinition typeDefinition, PropertyDefinition propertyDefinition, MethodDefinition originalMethod, FieldDefinition aspectField, MethodReference aspectRunMethod)
        {
            var ilProcessor = originalMethod.Body.GetILProcessor();
            originalMethod.Body.Instructions.Clear();
            originalMethod.Body.Variables.Clear();
            originalMethod.Body.InitLocals = true;

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
                    ilProcessor.Append(Instruction.Create(OpCodes.Dup));
                    ilProcessor.Append(Instruction.Create(OpCodes.Ldc_I4, parameter.Index));
                    ilProcessor.Append(Instruction.Create(OpCodes.Ldarg, parameter));

                    if (parameter.ParameterType.IsValueType)
                        ilProcessor.Append(Instruction.Create(OpCodes.Box, parameter.ParameterType));

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
            
            // creates a variable holding the reference to the PropertyInfo that represents the property being executed (this will be used to instantiate the PropertyContext)
            var propertyInfoTypeRef = ModuleDefinition.ImportReference(typeof(System.Reflection.PropertyInfo));
            var propertyInfoVar = new VariableDefinition(propertyInfoTypeRef);
            var propertyInfoCacheField = ModuleWeaver.CreatePropertyInfoCacheField(propertyDefinition);
            originalMethod.Body.Variables.Add(propertyInfoVar);
            ilProcessor.Append(Instruction.Create(OpCodes.Ldsfld, propertyInfoCacheField));
            ilProcessor.Append(Instruction.Create(OpCodes.Stloc, propertyInfoVar));

            // creates the PropertyContext instance
            var contextTypeRef = ModuleDefinition.ImportReference(typeof(PropertyContext));
            var contextCtor = ModuleDefinition.ImportReference(typeof(PropertyContext).GetConstructors().First());
            var contextVar = new VariableDefinition(contextTypeRef);
            originalMethod.Body.Variables.Add(contextVar);
            if (originalMethod.HasThis) ilProcessor.Append(Instruction.Create(OpCodes.Ldarg_0));
            if (!originalMethod.HasThis) ilProcessor.Append(Instruction.Create(OpCodes.Ldnull));
            ilProcessor.Append(Instruction.Create(OpCodes.Ldloc, propertyInfoVar));
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
                var getReturnValue = ModuleDefinition.ImportReference(typeof(PropertyContext).GetProperty(nameof(PropertyContext.ReturnValue)).GetMethod);
                ilProcessor.Append(Instruction.Create(OpCodes.Ldloc, contextVar));
                ilProcessor.Append(Instruction.Create(OpCodes.Callvirt, getReturnValue));

                if (originalMethod.ReturnType.IsValueType)
                    ilProcessor.Append(Instruction.Create(OpCodes.Unbox_Any, originalMethod.ReturnType));
                else
                    ilProcessor.Append(Instruction.Create(OpCodes.Castclass, originalMethod.ReturnType));
            }

            ilProcessor.Append(Instruction.Create(OpCodes.Ret));
        }
        */
    }
}
