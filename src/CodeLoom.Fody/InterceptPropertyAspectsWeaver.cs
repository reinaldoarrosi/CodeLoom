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
using System.Reflection;

namespace CodeLoom.Fody
{
    internal class InterceptPropertyAspectsWeaver : InterceptMethodAspectsWeaver
    {  
        internal ModuleWeaver ModuleWeaver;

        internal InterceptPropertyAspectsWeaver(ModuleWeaver moduleWeaver)
            : base(moduleWeaver)
        {
            ModuleWeaver = moduleWeaver;
            WeavedProperties = new Dictionary<System.Reflection.PropertyInfo, bool>();
        }

        internal Dictionary<System.Reflection.PropertyInfo, bool> WeavedProperties { get; set; }

        internal override void Weave(TypeDefinition typeDefinition)
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

            WeaveTypeProperties(typeDefinition);
        }

        private void WeaveTypeProperties(TypeDefinition typeDefinition)
        {
            var properties = typeDefinition.Properties.ToArray();

            foreach (var propertyDefinition in properties)
            {
                var property = propertyDefinition.TryGetPropertyInfo();
                if (property == null)
                {
                    ModuleWeaver.LogInfo($"Property {propertyDefinition.Name} from type {typeDefinition.FullName} will not be weaved because it was not possible to load its corresponding PropertyInfo");
                    continue;
                }

                if (WeavedProperties.ContainsKey(property))
                {
                    // if the property we're trying to weave was already weaved we'll skip weaving it
                    // all aspects are applied the first time the property is weaved, there's no need to weave it more than once
                    continue;
                }
                else
                {
                    // mark this property so that it won't be weaved again
                    WeavedProperties.Add(property, true);
                }

                var aspects = ModuleWeaver.Setup.GetPropertyAspects(property).ToArray();
                if (aspects.Length <= 0)
                {
                    ModuleWeaver.LogInfo($"Property {propertyDefinition.Name} from type {typeDefinition.FullName} will not be weaved because no aspect was applied to it");
                    continue;
                }

                // clone the original getter because we're going to rewrite it
                var clonedGetMethod = propertyDefinition.GetMethod != null ? CloneMethod(typeDefinition, propertyDefinition.GetMethod, true) : null;
                if (clonedGetMethod != null) typeDefinition.Methods.Add(clonedGetMethod);

                // clone the original setter because we're going to rewrite it
                var clonedSetMethod = propertyDefinition.SetMethod != null ? CloneMethod(typeDefinition, propertyDefinition.SetMethod, true) : null;
                if (clonedSetMethod != null) typeDefinition.Methods.Add(clonedSetMethod);

                // creates a Binding class that inherits from PropertyBinding
                // this class is used to invoke the aspects in a way that works with generic methods
                var propertyGenericParameters = (propertyDefinition.GetMethod ?? propertyDefinition.SetMethod).GenericParameters;
                var bindingTypeDef = CreateBindingTypeDef(typeDefinition, typeof(PropertyBinding), propertyDefinition.Name, propertyGenericParameters);

                // creates the static INSTANCE field that will hold the Binding instance, and adds it to our class
                var instanceField = CreateBindingInstanceField(bindingTypeDef);
                bindingTypeDef.Fields.Add(instanceField);

                // creates the constructor of and adds it to our Binding class
                var ctor = CreateBindingCtor(bindingTypeDef, typeof(IInterceptPropertyAspect));
                bindingTypeDef.Methods.Add(ctor);

                // creates the static constructor and adds it to our Binding class 
                var staticCtor = CreateBindingStaticCtor(bindingTypeDef, instanceField, ctor, typeof(IInterceptPropertyAspect), aspects);
                bindingTypeDef.Methods.Add(staticCtor);

                
                if (clonedGetMethod != null)
                {
                    // creates the ProceedGet method that overrides the abstract method MethodBinding.Proceed/AsyncMethodBinding.Proceed
                    var bindingFlags = BindingFlags.Instance | BindingFlags.NonPublic;
                    var abstractProceedGetMethod = ModuleDefinition.ImportReference(typeof(PropertyBinding).GetMethod("ProceedGet", bindingFlags));
                    var proceedGetMethod = CreateBindingProceedMethod(typeDefinition, bindingTypeDef, typeof(PropertyContext), clonedGetMethod, abstractProceedGetMethod, false);
                    bindingTypeDef.Methods.Add(proceedGetMethod);

                    // rewrites the original method so that it calls AsyncMethodBinding.Run
                    RewriteOriginalMethod(typeDefinition, bindingTypeDef, typeof(PropertyContext), propertyDefinition.GetMethod, false);
                }

                if (clonedSetMethod != null)
                {
                    // creates the ProceedGet method that overrides the abstract method MethodBinding.Proceed/AsyncMethodBinding.Proceed
                    var bindingFlags = BindingFlags.Instance | BindingFlags.NonPublic;
                    var abstractProceedSetMethod = ModuleDefinition.ImportReference(typeof(PropertyBinding).GetMethod("ProceedSet", bindingFlags));
                    var proceedSetMethod = CreateBindingProceedMethod(typeDefinition, bindingTypeDef, typeof(PropertyContext), clonedSetMethod, abstractProceedSetMethod, false);
                    bindingTypeDef.Methods.Add(proceedSetMethod);

                    // rewrites the original method so that it calls AsyncMethodBinding.Run
                    RewriteOriginalMethod(typeDefinition, bindingTypeDef, typeof(PropertyContext), propertyDefinition.SetMethod, false);
                }

                // adds our Binding class as a nested type of the class that we're weaving
                typeDefinition.NestedTypes.Add(bindingTypeDef);
            }
        }
    }
}
