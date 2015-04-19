using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Collections.Generic;
using ExtensibleILRewriter.Extensions;
using System;

namespace ExtensibleILRewriter.CodeInjection
{
    public class CodeInjector<CodeProviderArgumentType>
    {
        private readonly ModuleDefinition module;
        private readonly CodeProvider<CodeProviderArgumentType> codeProvider;
        private readonly Collection<Instruction> newInstructions = new Collection<Instruction>();

        public CodeInjector(ModuleDefinition module, CodeProvider<CodeProviderArgumentType> codeProvider)
        {
            this.module = module;
            this.codeProvider = codeProvider;
        }

        public void InjectAtBegining(MethodDefinition method, CodeProviderArgumentType codeProviderArgument, ILogger logger)
        {
            var callInfo = codeProvider.GetCallInfo(codeProviderArgument, method.Module);

            if (!callInfo.ShouldBeCallInjected) return;
            if (!method.HasBody) throw new ArgumentException("Method does not contain body.");

            logger.Notice("Injecting method call into method '\{method.FullName}'.");

            newInstructions.Clear();
            foreach (var arg in callInfo.CallArguments) newInstructions.Add(arg.GenerateLoadInstruction());
            newInstructions.Add(Instruction.Create(OpCodes.Call, callInfo.MethodReferenceToBeCalled));

            method.Body.AddInstructionsToBegining(newInstructions);
        }
    }
}