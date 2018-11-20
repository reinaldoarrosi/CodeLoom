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
                var property = propertyDefinition.GetPropertyInfo();
                if (property == null)
                {
                    ModuleWeaver.LogInfo($"Property {propertyDefinition.Name} from type {typeDefinition.FullName} will not be weaved because it was not possible to load its corresponding PropertyInfo");
                    continue;
                }

                if (WeavedProperties.ContainsKey(property))
                    continue;

                WeavedProperties.Add(property, true);

                var aspects = ModuleWeaver.Setup.GetPropertyAspects(property).ToArray();
                if (aspects.Length <= 0)
                {
                    ModuleWeaver.LogInfo($"Property {propertyDefinition.Name} from type {typeDefinition.FullName} will not be weaved because no aspect was applied to it");
                    continue;
                }
            }
        }
    }
}
