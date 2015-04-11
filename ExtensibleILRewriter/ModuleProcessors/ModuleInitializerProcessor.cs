using Mono.Cecil;
using System;
using System.Linq;
using ExtensibleILRewriter.ParameterProcessors.Contracts;
using Mono.Cecil.Cil;
using ExtensibleILRewriter.Extensions;

namespace ExtensibleILRewriter.ModuleProcessors
{
    public class ModuleInitializerProcessor : ModuleProcessor<ComponentProcessorConfiguration.EmptyConfiguration>
    {
        private readonly static string moduleInitializerAttributeFullName = typeof(ModuleInitializerAttribute).FullName;

        public ModuleInitializerProcessor([NotNull]ComponentProcessorConfiguration.EmptyConfiguration configuration, [NotNull]ILogger logger)
            : base(configuration, logger)
        {
        }

        public override void Process([NotNull]ModuleDefinition module)
        {
            var attribute = module.CustomAttributes.FirstOrDefault(a => a.AttributeType.FullName == moduleInitializerAttributeFullName);
            if (attribute == null) return;

            var typeName = (string)attribute.ConstructorArguments[0].Value;
            var methodName = (string)attribute.ConstructorArguments[1].Value;

            var type = module.GetType(typeName);
            if (type == null) throw new InvalidOperationException("ModuleInitializerProcessor cannot find type '\{typeName}'.");

            var method = type.Methods.FirstOrDefault(m => m.Name == methodName);
            if (method == null) throw new InvalidOperationException("ModuleInitializerProcessor cannot find method '\{methodName}' on type '\{typeName}'.");

            if (!method.IsStatic) throw new InvalidOperationException("Method '\{method.FullName}' is marked to be called from module initializer so it must be static.");
            if (!method.IsPublic) throw new InvalidOperationException("Method '\{method.FullName}' is marked to be called from module initializer so it must be public.");
            if (method.ReturnType != module.TypeSystem.Void) throw new InvalidOperationException("Method '\{method.FullName}' is marked to be called from module initializer so it must be void.");
            if (method.Parameters.Count != 0) throw new InvalidOperationException("Method '\{method.FullName}' is marked to be called from module initializer so it must have 0 arguments.");

            var cctor = FindOrCreateInitializer(module);

            cctor.Body.AddInstructionToBegining(Instruction.Create(OpCodes.Call, method));
        }

        public static MethodDefinition FindOrCreateInitializer([NotNull]ModuleDefinition module)
        {
            var moduleType = module.GetType("<Module>");
            if (moduleType == null) throw new InvalidOperationException("ModuleInitializerProcessor cannot find type '<Module>' in '\{module.FullyQualifiedName}' module.");

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
    }
}
