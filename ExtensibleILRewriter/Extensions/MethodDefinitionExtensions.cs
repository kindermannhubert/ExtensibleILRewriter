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
            if (!method.HasBody) return false;
            if (method.IsStatic) return true;
            return !AccessesThis(method.Body);
        }

        private static bool AccessesThis(MethodBody methodBody)
        {
            foreach (var ins in methodBody.Instructions)
            {
                if (ins.OpCode.Code == Code.Ldarg_0) return true;
                if (ins.Operand as ParameterDefinition == methodBody.ThisParameter) return true;
            }
            return false;
        }

        public static MethodDefinition CreateStaticVersion(this MethodDefinition method)
        {
            if (!method.CouldBeStatic()) throw new InvalidOperationException("Method '\{method.FullName}' cannot be made static.");

            var attributes = MethodAttributes.Static | MethodAttributes.HideBySig;
            attributes |= method.Attributes & MethodAttributes.Private;
            attributes |= method.Attributes & MethodAttributes.FamANDAssem;
            attributes |= method.Attributes & MethodAttributes.Assembly;
            attributes |= method.Attributes & MethodAttributes.Family;
            attributes |= method.Attributes & MethodAttributes.FamORAssem;
            attributes |= method.Attributes & MethodAttributes.Public;

            var staticMethod = new MethodDefinition(method.Name, attributes, method.ReturnType);
            staticMethod.Parameters.AddRange(method.Parameters);
            staticMethod.Body.Variables.AddRange(method.Body.Variables);
            staticMethod.Body.InitLocals = method.Body.InitLocals;

            method.Body.SimplifyMacros();
            foreach (var ins in method.Body.Instructions)
            {
                Instruction newIns = ins;
                if (ins.OpCode.OperandType == OperandType.InlineArg)
                {
                    var parameter = (ParameterDefinition)ins.Operand;
                    newIns = Instruction.Create(ins.OpCode, parameter);
                }

                newIns.Operand = ins.Operand;
                staticMethod.Body.Instructions.Add(newIns);
            }
            staticMethod.Body.OptimizeMacros();
            method.Body.OptimizeMacros();

            return staticMethod;
        }

        public static void AddInstructionToBegining(this MethodBody body, Instruction newInstruction)
        {
            body.SimplifyMacros();

            body.Instructions.Insert(0, newInstruction);

            body.OptimizeMacros();
        }

        public static void AddInstructionsToBegining(this MethodBody body, Collection<Instruction> newInstructions)
        {
            body.SimplifyMacros();

            var instructions = body.Instructions;
            var oldInstructions = instructions.ToArray();

            instructions.Clear();
            instructions.AddRange(newInstructions);
            instructions.AddRange(oldInstructions);

            body.OptimizeMacros();
        }
    }
}