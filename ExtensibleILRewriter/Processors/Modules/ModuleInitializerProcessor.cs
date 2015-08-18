using ExtensibleILRewriter.Extensions;
using ExtensibleILRewriter.Logging;
using ExtensibleILRewriter.Processors.Parameters;
using Mono.Cecil;
using Mono.Cecil.Cil;
using System;
using System.Linq;

namespace ExtensibleILRewriter.Processors.Modules
{
    public class ModuleInitializerProcessor : ComponentProcessor<ComponentProcessorConfiguration.EmptyConfiguration>
    {
        private static readonly string ModuleInitializerAttributeFullName = typeof(ModuleInitializerAttribute).FullName;

        public ModuleInitializerProcessor([NotNull]ComponentProcessorConfiguration.EmptyConfiguration configuration, [NotNull]ILogger logger)
            : base(configuration, logger)
        {
            AddSupportedComponent(ProcessableComponentType.Module);
        }

        public override void Process([NotNull]IProcessableComponent component)
        {
            if (component.Type != ProcessableComponentType.Module)
            {
                throw new InvalidOperationException("Component is expected to be module.");
            }

            var module = (ModuleProcessableComponent)component;
            var attributes = module.CustomAttributes.Where(a => a.AttributeType.FullName == ModuleInitializerAttributeFullName);
            foreach (var attribute in attributes)
            {
                ProcessModuleInitializerAttribute(attribute, module);
            }
        }

        public static MethodDefinition FindOrCreateInitializer([NotNull]ModuleDefinition module)
        {
            var moduleType = module.GetType("<Module>");
            if (moduleType == null)
            {
                throw new InvalidOperationException($"ModuleInitializerProcessor cannot find type '<Module>' in '{module.FullyQualifiedName}' module.");
            }

            var cctor = moduleType.Methods.FirstOrDefault(x => x.Name == ".cctor");
            if (cctor == null)
            {
                var attributes = MethodAttributes.Private | MethodAttributes.HideBySig | MethodAttributes.Static | MethodAttributes.SpecialName | MethodAttributes.RTSpecialName;
                cctor = new MethodDefinition(".cctor", attributes, moduleType.Module.TypeSystem.Void);
                moduleType.Methods.Add(cctor);
                cctor.Body.Instructions.Add(Instruction.Create(OpCodes.Ret));
            }

            return cctor;
        }

        private void ProcessModuleInitializerAttribute([NotNull]CustomAttribute attribute, [NotNull]ModuleProcessableComponent module)
        {
            var typeName = (string)attribute.ConstructorArguments[0].Value;
            var methodName = (string)attribute.ConstructorArguments[1].Value;

            var moduleDefinition = module.UnderlyingComponent;

            var type = moduleDefinition.GetType(typeName);
            if (type == null)
            {
                throw new InvalidOperationException($"ModuleInitializerProcessor cannot find type '{typeName}'.");
            }

            var method = type.Methods.FirstOrDefault(m => m.Name == methodName);
            if (method == null)
            {
                throw new InvalidOperationException($"ModuleInitializerProcessor cannot find method '{methodName}' on type '{typeName}'.");
            }

            if (!method.IsStatic)
            {
                throw new InvalidOperationException($"Method '{method.FullName}' is marked to be called from module initializer so it must be static.");
            }

            if (!method.IsPublic)
            {
                throw new InvalidOperationException($"Method '{method.FullName}' is marked to be called from module initializer so it must be public.");
            }

            if (method.ReturnType != moduleDefinition.TypeSystem.Void)
            {
                throw new InvalidOperationException($"Method '{method.FullName}' is marked to be called from module initializer so it must be void.");
            }

            if (method.Parameters.Count != 0)
            {
                throw new InvalidOperationException($"Method '{method.FullName}' is marked to be called from module initializer so it must have 0 arguments.");
            }

            var cctor = FindOrCreateInitializer(moduleDefinition);

            cctor.Body.AddInstructionToBegining(Instruction.Create(OpCodes.Call, method));
        }
    }
}
