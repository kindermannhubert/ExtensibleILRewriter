using System.Linq;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Collections.Generic;
using ExtensibleILRewriter.Extensions;
using Mono.Cecil.Rocks;
using System;
using System.Collections.Generic;

namespace ExtensibleILRewriter.ParameterProcessors
{
    public class ParameterValueHandlingCodeInjector
    {
        public const string StaticHandlingMethodPrefix = "__static_";

        private readonly ModuleDefinition module;
        private readonly IParameterValueHandlingCodeProvider codeProvider;
        private readonly Dictionary<TypeReference, GenericInstanceMethod> handleParameterMethodImportedReferences = new Dictionary<TypeReference, GenericInstanceMethod>();
        private readonly Collection<Instruction> newInstructions = new Collection<Instruction>();

        public ParameterValueHandlingCodeInjector(ModuleDefinition module, IParameterValueHandlingCodeProvider codeProvider)
        {
            this.module = module;
            this.codeProvider = codeProvider;
        }

        public void Inject(MethodDefinition method, ParameterDefinition parameter, FieldDefinition stateHoldingField, ILogger logger)
        {
            if (!codeProvider.ShouldHandleParameter(parameter, method)) return;
            if (!method.HasBody) throw new ArgumentException("Method does not contain body.");

            logger.Notice("Injecting parameter handlig to method '\{method.FullName}' of parameter '\{parameter.Name}'.");

            GenericInstanceMethod handleParameterMethodImportedReference;
            if (!handleParameterMethodImportedReferences.TryGetValue(parameter.ParameterType, out handleParameterMethodImportedReference))
            {
                var handligMethodReference = module.Import(codeProvider.GetHandleParameterMethodInfo(parameter.ParameterType));
                handleParameterMethodImportedReference = new GenericInstanceMethod(handligMethodReference);
                handleParameterMethodImportedReference.GenericArguments.Add(parameter.ParameterType);
            }

            newInstructions.Clear();
            if (stateHoldingField == null) newInstructions.Add(Instruction.Create(OpCodes.Ldnull));
            else newInstructions.Add(Instruction.Create(OpCodes.Ldsfld, stateHoldingField));
            newInstructions.Add(Instruction.Create(OpCodes.Ldarg, parameter));
            newInstructions.Add(Instruction.Create(OpCodes.Ldstr, parameter.Name));
            newInstructions.Add(Instruction.Create(OpCodes.Call, handleParameterMethodImportedReference));

            method.Body.AddInstructionsToBegining(newInstructions);
        }
    }
}