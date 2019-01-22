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
using CodeLoom.Fody.Helpers;

namespace CodeLoom.Fody
{
    internal class InterceptMethodAspectsWeaver
    {
        private readonly ModuleWeaver ModuleWeaver;
        private readonly MethodWeaverHelper MethodWeaverHelper;
        private readonly Dictionary<System.Reflection.MethodBase, bool> WeavedMethods;

        internal InterceptMethodAspectsWeaver(ModuleWeaver moduleWeaver)
        {
            ModuleWeaver = moduleWeaver;
            MethodWeaverHelper = new MethodWeaverHelper(moduleWeaver);
            WeavedMethods = new Dictionary<System.Reflection.MethodBase, bool>();
        }

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
                    var clonedMethod = MethodWeaverHelper.CloneMethod(typeDefinition, originalMethod);
                    typeDefinition.Methods.Add(clonedMethod);

                    // creates a Binding class that inherits from AsyncMethodBinding
                    // this class is used to invoke the aspects in a way that works with generic methods
                    var methodBindingTypeDef = MethodWeaverHelper.CreateBindingTypeDef(typeDefinition, typeof(AsyncMethodBinding), originalMethod.Name, originalMethod.GenericParameters);

                    // creates the static INSTANCE field that will hold the Binding instance, and adds it to our class
                    var instanceField = MethodWeaverHelper.CreateBindingInstanceField(methodBindingTypeDef);
                    methodBindingTypeDef.Fields.Add(instanceField);

                    // creates the constructor of and adds it to our Binding class
                    var ctor = MethodWeaverHelper.CreateBindingCtor(methodBindingTypeDef, typeof(AsyncMethodBinding), typeof(IInterceptAsyncMethodAspect));
                    methodBindingTypeDef.Methods.Add(ctor);

                    // creates the static constructor and adds it to our Binding class 
                    var staticCtor = MethodWeaverHelper.CreateBindingStaticCtor(methodBindingTypeDef, instanceField, ctor, typeof(IInterceptAsyncMethodAspect), aspects);
                    methodBindingTypeDef.Methods.Add(staticCtor);

                    // creates the Proceed method that overrides the abstract method MethodBinding.Proceed/AsyncMethodBinding.Proceed
                    var bindingFlags = System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic;
                    var abstractProceedMethod = ModuleWeaver.ModuleDefinition.ImportReference(typeof(AsyncMethodBinding).GetMethod("Proceed", bindingFlags));
                    var proceedMethod = MethodWeaverHelper.CreateBindingProceedMethod(typeDefinition, methodBindingTypeDef, typeof(AsyncMethodContext), clonedMethod, abstractProceedMethod, true);
                    methodBindingTypeDef.Methods.Add(proceedMethod);

                    // adds our Binding class as a nested type of the class that we're weaving
                    typeDefinition.NestedTypes.Add(methodBindingTypeDef);

                    // rewrites the original method so that it calls AsyncMethodBinding.Run
                    MethodWeaverHelper.RewriteOriginalMethod(typeDefinition, methodBindingTypeDef, typeof(AsyncMethodBinding), typeof(AsyncMethodContext), originalMethod, true);
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
                    var clonedMethod = MethodWeaverHelper.CloneMethod(typeDefinition, originalMethod);
                    typeDefinition.Methods.Add(clonedMethod);

                    // creates a Binding class that inherits from MethodBinding
                    // this class is used to invoke the aspects in a way that works with generic methods
                    var methodBindingTypeDef = MethodWeaverHelper.CreateBindingTypeDef(typeDefinition, typeof(MethodBinding), originalMethod.Name, originalMethod.GenericParameters);

                    // creates the static INSTANCE field that will hold the Binding instance, and adds it to our class
                    var instanceField = MethodWeaverHelper.CreateBindingInstanceField(methodBindingTypeDef);
                    methodBindingTypeDef.Fields.Add(instanceField);

                    // creates the constructor of and adds it to our Binding class
                    var ctor = MethodWeaverHelper.CreateBindingCtor(methodBindingTypeDef, typeof(MethodBinding), typeof(IInterceptMethodAspect));
                    methodBindingTypeDef.Methods.Add(ctor);

                    // creates the static constructor and adds it to our Binding class 
                    var staticCtor = MethodWeaverHelper.CreateBindingStaticCtor(methodBindingTypeDef, instanceField, ctor, typeof(IInterceptMethodAspect), aspects);
                    methodBindingTypeDef.Methods.Add(staticCtor);

                    // creates the Proceed method that overrides the abstract method MethodBinding.Proceed/AsyncMethodBinding.Proceed
                    var bindingFlags = System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic;
                    var abstractProceedMethod = ModuleWeaver.ModuleDefinition.ImportReference(typeof(MethodBinding).GetMethod("Proceed", bindingFlags));
                    var proceedMethod = MethodWeaverHelper.CreateBindingProceedMethod(typeDefinition, methodBindingTypeDef, typeof(MethodContext), clonedMethod, abstractProceedMethod, false);
                    methodBindingTypeDef.Methods.Add(proceedMethod);

                    // adds our Binding class as a nested type of the class that we're weaving
                    typeDefinition.NestedTypes.Add(methodBindingTypeDef);

                    // rewrites the original method so that it calls MethodBinding.Run
                    MethodWeaverHelper.RewriteOriginalMethod(typeDefinition, methodBindingTypeDef, typeof(MethodBinding), typeof(MethodContext), originalMethod, false);
                }
            }
        }
    }
}
