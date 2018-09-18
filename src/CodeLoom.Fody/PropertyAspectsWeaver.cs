using Mono.Cecil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeLoom.Fody
{
    class PropertyAspectsWeaver
    {
        internal ModuleWeaver ModuleWeaver;

        public PropertyAspectsWeaver(ModuleWeaver moduleWeaver)
        {
            ModuleWeaver = moduleWeaver;
        }

        public void Weave(TypeDefinition typeDefinition)
        {

        }
    }
}
