using System.Collections.Generic;
using System.Linq;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Collections.Generic;
using ExtensibleILRewriter.Extensions;
using Mono.Cecil.Rocks;
using System;
using System.Diagnostics;

namespace ExtensibleILRewriter.MethodProcessors.ArgumentHandling
{
    public class ArgumentHandligCodeInjector<ArgumentType> : IArgumentHandlingCodeInjector
    {
        public const string StaticHandlingMethodPrefix = "__static_";

        private readonly ArgumentHandlingCodeProvider<ArgumentType> codeProvider;
        private readonly MethodDefinition handleParameterMethodDefinition;
        private readonly MethodReference handleParameterMethodImportedReference;
        private readonly Collection<Instruction> oldInstructions = new Collection<Instruction>();

        public ArgumentHandligCodeInjector(ModuleDefinition module, ArgumentHandlingCodeProvider<ArgumentType> codeProvider)
        {
            this.codeProvider = codeProvider;

            codeProvider.CheckPrerequisites();

            switch (codeProvider.HandlingType)
            {
                case ArgumentHandlingType.CallStaticHandling:
                    var methodInfo = codeProvider.GetType().GetMethod(StaticHandlingMethodPrefix + nameof(ArgumentHandlingCodeProvider<>.HandleArgument));
                    handleParameterMethodImportedReference = module.Import(methodInfo);
                    handleParameterMethodDefinition = handleParameterMethodImportedReference.Resolve();
                    break;
                case ArgumentHandlingType.CallInstanceHandling:
                    Action<ArgumentType, string> handleParameterMethodDelegate = codeProvider.HandleArgument;
                    handleParameterMethodImportedReference = module.Import(handleParameterMethodDelegate.Method);
                    handleParameterMethodDefinition = handleParameterMethodImportedReference.Resolve();
                    break;
                default:
                    throw new NotImplementedException("Unknown argument handling type: '\{codeProvider.HandlingType}'.");
            }

            CheckHandleParameterMethod();
        }

        private void CheckHandleParameterMethod()
        {
            if (!handleParameterMethodDefinition.HasBody) throw new InvalidOperationException("Code provider's method '\{handleParameterMethodDefinition.FullName}' does not have body.");
        }

        public void Inject(MethodDefinition method, ParameterDefinition parameter, ILogger logger)
        {
            if (!method.HasBody) throw new ArgumentException("Method does not contain body.");
            if (parameter.ParameterType.FullName != typeof(ArgumentType).FullName) throw new ArgumentException("Parameter type of \{nameof(ArgumentHandligCodeInjector)} does not match type of argument.", nameof(parameter));

            logger.Notice("Injecting parameter handlig to method '\{method.FullName}' of parameter '\{parameter.Name}'.");

            EmitCallHandling(method, parameter);
        }

        private void EmitCallHandling(MethodDefinition method, ParameterDefinition parameter)
        {
            oldInstructions.Clear();
            method.Body.SimplifyMacros();
            oldInstructions.AddRange(method.Body.Instructions);

            switch (codeProvider.HandlingType)
            {
                case ArgumentHandlingType.CallStaticHandling:
                    EmitStaticCallHandling(method, parameter, oldInstructions);
                    break;
                case ArgumentHandlingType.CallInstanceHandling:
                    EmitInstanceCallHandling(method, parameter, oldInstructions);
                    break;
                default:
                    throw new NotImplementedException("Unknown argument handling type: '\{codeProvider.HandlingType}'.");
            }


            var sequencePoint = method.Body.Instructions.FirstOrDefault(i => i.SequencePoint != null && i.SequencePoint.Document != null)?.SequencePoint;
            if (sequencePoint != null)
            {
                var firstInstruction = method.Body.Instructions[0];
                firstInstruction.SequencePoint = new SequencePoint(sequencePoint.Document);
                firstInstruction.SequencePoint.StartLine = sequencePoint.StartLine;
                firstInstruction.SequencePoint.EndLine = sequencePoint.EndLine;
                firstInstruction.SequencePoint.StartColumn = sequencePoint.StartColumn;
                firstInstruction.SequencePoint.EndColumn = sequencePoint.EndColumn;
            }

            method.Body.OptimizeMacros();
        }

        private void EmitStaticCallHandling(MethodDefinition method, ParameterDefinition parameter, Collection<Instruction> oldInstructions)
        {
            method.Body.Instructions.Clear();
            method.Body.Instructions.Add(Instruction.Create(OpCodes.Ldarg, parameter));
            method.Body.Instructions.Add(Instruction.Create(OpCodes.Ldstr, parameter.Name));
            method.Body.Instructions.Add(Instruction.Create(OpCodes.Call, handleParameterMethodImportedReference));
            method.Body.Instructions.AddRange(oldInstructions);
        }

        private void EmitInstanceCallHandling(MethodDefinition method, ParameterDefinition parameter, Collection<Instruction> oldInstructions)
        {
            method.Body.Instructions.Clear();

            method.Body.Instructions.Add(Instruction.Create(OpCodes.Ldarg, parameter));
            method.Body.Instructions.Add(Instruction.Create(OpCodes.Ldarg, parameter));
            method.Body.Instructions.Add(Instruction.Create(OpCodes.Ldarg, parameter));

            method.Body.Instructions.Add(Instruction.Create(OpCodes.Ldarg, parameter));
            method.Body.Instructions.Add(Instruction.Create(OpCodes.Ldstr, parameter.Name));
            method.Body.Instructions.Add(Instruction.Create(OpCodes.Call, handleParameterMethodImportedReference));
            method.Body.Instructions.AddRange(oldInstructions);
        }

        //private static void InstanceHandle(ArgumentHandlingCodeProvider<ArgumentType> instance, ArgumentType argument, string argumentName)
        //{
        //    if (instance != null)
        //    {
        //        instance.HandleArgument(argument, argumentName);
        //    }
        //}
    }
}