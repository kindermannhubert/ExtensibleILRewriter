using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Cecil.Rocks;
using Mono.Collections.Generic;
using System;
using System.Linq;

namespace ExtensibleILRewriter.Extensions
{
    public static class MethodDefinitionExtensions
    {
        public static bool CouldBeStatic(this MethodDefinition method)
        {
            if (!method.HasBody)
            {
                return false;
            }

            if (method.IsStatic)
            {
                return true;
            }

            return !AccessesThis(method.Body);
        }

        private static bool AccessesThis(MethodBody methodBody)
        {
            foreach (var ins in methodBody.Instructions)
            {
                if (ins.OpCode.Code == Code.Ldarg_0)
                {
                    return true;
                }

                if (ins.Operand as ParameterDefinition == methodBody.ThisParameter)
                {
                    return true;
                }
            }

            return false;
        }

        public static void AddInstructionToBegining(this MethodBody body, Instruction newInstruction)
        {
            body.SimplifyMacros();

            body.Instructions.Insert(0, newInstruction);

            body.OptimizeMacros();
        }

        public static void AddInstructionsToBegining(this MethodBody body, Collection<Instruction> newInstructions, bool repairSequencePoints = true)
        {
            body.SimplifyMacros();

            var instructions = body.Instructions;
            var oldInstructions = instructions.ToArray();

            instructions.Clear();
            instructions.AddRange(newInstructions);
            instructions.AddRange(oldInstructions);

            if (repairSequencePoints)
            {
                var sequencePoint = instructions.FirstOrDefault(i => i.SequencePoint != null && i.SequencePoint.Document != null)?.SequencePoint;
                if (sequencePoint != null)
                {
                    var firstInstruction = instructions[0];
                    firstInstruction.SequencePoint = new SequencePoint(sequencePoint.Document);
                    firstInstruction.SequencePoint.StartLine = sequencePoint.StartLine;
                    firstInstruction.SequencePoint.EndLine = sequencePoint.EndLine;
                    firstInstruction.SequencePoint.StartColumn = sequencePoint.StartColumn;
                    firstInstruction.SequencePoint.EndColumn = sequencePoint.EndColumn;
                }
            }

            body.OptimizeMacros();
        }

        public static void AddInstructionsBeforeExit(this MethodBody body, Collection<Instruction> newInstructions)
        {
            body.SimplifyMacros();

            var instructions = body.Instructions;
            var oldInstructions = instructions.ToArray();

            instructions.Clear();

            foreach (var ins in oldInstructions)
            {
                if (ins.OpCode.Code == Code.Ret)
                {
                    instructions.AddRange(newInstructions);
                }

                instructions.Add(ins);
            }

            body.OptimizeMacros();
        }
    }
}