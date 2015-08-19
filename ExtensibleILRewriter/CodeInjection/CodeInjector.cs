using ExtensibleILRewriter.Extensions;
using ExtensibleILRewriter.Logging;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Collections.Generic;
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

        public delegate void CustomInstructionsInjection(MethodBody body, Collection<Instruction> newInstructions);

        public bool ShouldBeCallInjected(CodeProviderArgumentType codeProviderArgument)
        {
            return codeProvider.ShouldBeInjected(codeProviderArgument);
        }

        public void InjectAtBegining(MethodDefinition method, CodeProviderArgumentType codeProviderArgument, ILogger logger)
        {
            Inject(
                method,
                codeProviderArgument,
                logger,
                (body, newInstructions) => body.AddInstructionsToBegining(newInstructions));
        }

        public void InjectBeforeExit(MethodDefinition method, CodeProviderArgumentType codeProviderArgument, ILogger logger)
        {
            Inject(
                method,
                codeProviderArgument,
                logger,
                (body, newInstructions) => body.AddInstructionsBeforeExit(newInstructions));
        }

        public void Inject(MethodDefinition method, CodeProviderArgumentType codeProviderArgument, ILogger logger, CustomInstructionsInjection injectNewInstructions)
        {
            var callInfo = codeProvider.GetCallInfo(codeProviderArgument, method.Module);

            if (!method.HasBody)
            {
                throw new ArgumentException("Method does not contain body.");
            }

            logger.Notice($"Injecting method call before exit of method '{method.FullName}'.");

            newInstructions.Clear();

            GenerateInstructionsForInjectedCall(newInstructions, callInfo.MethodReferenceToBeCalled, callInfo.CallArguments);

            injectNewInstructions(method.Body, newInstructions);
        }

        private static void GenerateInstructionsForInjectedCall(Collection<Instruction> instructions, MethodReference methodCall, CodeProviderCallArgument[] arguments)
        {
            foreach (var arg in arguments)
            {
                instructions.Add(arg.GenerateLoadInstruction());
            }

            instructions.Add(Instruction.Create(OpCodes.Call, methodCall));
        }
    }
}