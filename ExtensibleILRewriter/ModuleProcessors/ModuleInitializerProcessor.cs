using Mono.Cecil;
using System;
using System.Linq;
using ExtensibleILRewriter.MethodProcessors.Contracts;
using Mono.Cecil.Cil;
using Mono.Cecil.Rocks;

namespace ExtensibleILRewriter.ModuleProcessors
{
    public class ModuleInitializerProcessor : ComponentProcessor<ModuleDefinition, ComponentProcessorConfiguration.EmptyConfiguration>
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

            var moduleType = module.GetType("<Module>");
            if (moduleType == null) throw new InvalidOperationException("ModuleInitializerProcessor cannot find type '<Module>' in '\{module.FullyQualifiedName}' module.");

            var cctor = FindOrCreateInitializer(moduleType);
            cctor.Body.SimplifyMacros();

            var instructions = cctor.Body.Instructions;
            var oldInstructions = instructions.ToArray();
            instructions.Clear();
            foreach (var ins in oldInstructions)
            {
                if (ins.OpCode.Code == Code.Ret)
                {
                    instructions.Add(Instruction.Create(OpCodes.Call, method));
                }
                instructions.Add(ins);
            }

            cctor.Body.OptimizeMacros();
        }

        private MethodDefinition FindOrCreateInitializer(TypeDefinition moduleType)
        {
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
