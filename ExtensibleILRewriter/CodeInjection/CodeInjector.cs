using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Collections.Generic;
using ExtensibleILRewriter.Extensions;
using System;
using ExtensibleILRewriter.Logging;

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

        public bool ShouldBeCallInjected(CodeProviderArgumentType codeProviderArgument)
        {
            return codeProvider.ShouldBeInjected(codeProviderArgument);
        }

        public void InjectAtBegining(MethodDefinition method, CodeProviderArgumentType codeProviderArgument, ILogger logger)
        {
            var callInfo = codeProvider.GetCallInfo(codeProviderArgument, method.Module);

            if (!method.HasBody)
            {
                throw new ArgumentException("Method does not contain body.");
            }

            logger.Notice($"Injecting method call at begining of method '{method.FullName}'.");

            newInstructions.Clear();

            InjectCall(newInstructions, callInfo.MethodReferenceToBeCalled, callInfo.CallArguments);

            method.Body.AddInstructionsToBegining(newInstructions);
        }

        public void InjectBeforeExit(MethodDefinition method, CodeProviderArgumentType codeProviderArgument, ILogger logger)
        {
            var callInfo = codeProvider.GetCallInfo(codeProviderArgument, method.Module);

            if (!method.HasBody)
            {
                throw new ArgumentException("Method does not contain body.");
            }

            logger.Notice($"Injecting method call before exit of method '{method.FullName}'.");

            newInstructions.Clear();

            InjectCall(newInstructions, callInfo.MethodReferenceToBeCalled, callInfo.CallArguments);

            method.Body.AddInstructionsBeforeExit(newInstructions);
        }

        private static void InjectCall(Collection<Instruction> instructions, MethodReference methodCall, CodeProviderCallArgument[] arguments)
        {
            foreach (var arg in arguments)
            {
                instructions.Add(arg.GenerateLoadInstruction());
            }

            instructions.Add(Instruction.Create(OpCodes.Call, methodCall));
        }
    }
}