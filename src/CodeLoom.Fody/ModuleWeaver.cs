using CodeLoom.Aspects;
using CodeLoom.Setup;
using Fody;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Cecil.Rocks;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

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

        public override IEnumerable<string> GetAssembliesForScanning()
        {
            return Enumerable.Empty<string>();
        }

        private void WeaveType(TypeDefinition type)
        {
            if (type.IsInterface) return;

            foreach (var nestedType in type.NestedTypes)
            {
                WeaveType(nestedType);
            }

            LogInfo($"Weaving type {type.FullName}");

            LogInfo($"Weaving {nameof(InterceptMethodAspect)}");
            InterceptMethodAspectsWeaver.Weave(type);

            LogInfo($"Weaving {nameof(InterceptPropertyAspect)}");
            InterceptPropertyAspectsWeaver.Weave(type);

            LogInfo($"Weaving {nameof(ImplementInterfaceAspect)}");
            ImplementInterfaceAspectsWeaver.Weave(type);
        }

        private CodeLoomSetup GetSetup(ModuleDefinition moduleDefinition)
        {
            var setupAttr = moduleDefinition.Assembly.CustomAttributes.FirstOrDefault(attr => attr.AttributeType.FullName == typeof(CodeLoomSetupAttribute).FullName);
            var setupType = (setupAttr?.ConstructorArguments.First().Value as TypeReference).Resolve().GetSystemType();
            var setup = Activator.CreateInstance(setupType ?? typeof(CodeLoomSetup.Empty));

            return (CodeLoomSetup)setup;
        }

        private void SetupMissingAssemblyResolutionStrategy()
        {
            // Fody already registers a handler for AppDomain.CurrentDomain.AssemblyResolve that uses Assembly.Load(byte[], byte[]) to load assemblies
            // This may cause the same assembly being loaded multiple time. Here we change the event handler using reflection to make our handler run
            // before the one registered by Fody. Our handler makes sure that an assembly is never loaded more than once.
            var field = System.Reflection.RuntimeReflectionExtensions.GetRuntimeFields(typeof(AppDomain)).FirstOrDefault(f => f.Name == "_AssemblyResolve");
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
            var field = System.Reflection.RuntimeReflectionExtensions.GetRuntimeFields(typeof(AppDomain)).FirstOrDefault(f => f.Name == "_AssemblyResolve");
            var eventDelegate = field.GetValue(AppDomain.CurrentDomain) as MulticastDelegate;
            var handlers = eventDelegate.GetInvocationList().ToList();

            handlers.RemoveAt(0);

            field.SetValue(AppDomain.CurrentDomain, Delegate.Combine(handlers.ToArray()));
        }

        private System.Reflection.Assembly OnResolveAssembly(object sender, ResolveEventArgs args)
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
                    var assembly = System.Reflection.Assembly.Load(assemblyBytes);

                    return assembly;
                }
            }

            return null;
        }
    }
}
