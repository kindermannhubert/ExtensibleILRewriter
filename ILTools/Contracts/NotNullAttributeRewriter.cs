using System.Collections.Generic;
using System.Linq;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Collections.Generic;
using ILTools.Extensions;
using Mono.Cecil.Rocks;
using System;
using System.Diagnostics;

namespace ILTools.Contracts
{
    public class NotNullAttributeRewriter : IMethodProcessor
    {
        private readonly static string notNullAttributeFullName = typeof(NotNullAttribute).FullName;
        private readonly static Instruction nopInstruction = Instruction.Create(OpCodes.Nop);

        public void Rewrite(MethodDefinition method, List<string> errors)
        {
            var body = method.Body;
            Collection<Instruction> originalMethodInstructions = null, bodyInstructions = null;
            List<Instruction> newBranchInstructions = null;
            MethodReference argumentNullExceptionCtor = null; //TODO PERF:  smarter cacheing per module

            foreach (var parameter in method.Parameters)
            {
                if (parameter.CustomAttributes.Any(a => a.AttributeType.FullName == notNullAttributeFullName))
                {
                    if (parameter.ParameterType.IsValueType)
                    {
                        errors.Add("Parameter '\{parameter.Name}' of method '\{method.FullName}' cannot be non-nullable because it is value type.");
                        continue;
                    }

                    if (!method.HasBody)
                    {
                        errors.Add("Method '\{method.FullName}' does not have body and cannot be rewritten.");
                        continue;
                    }

                    if (originalMethodInstructions == null)
                    {
                        //lazy initialization
                        body.SimplifyMacros();
                        bodyInstructions = body.Instructions;
                        originalMethodInstructions = new Collection<Instruction>(bodyInstructions);
                        bodyInstructions.Clear();

                        newBranchInstructions = new List<Instruction>();
                        argumentNullExceptionCtor = method.Module.Import(typeof(ArgumentNullException).GetConstructor(new[] { typeof(string) }));
                    }

                    //IL_0000: ldarg.0
                    //IL_0001: brtrue.s IL_000e
                    //IL_0003: ldstr "xxx"
                    //IL_0008: newobj System.Void System.ArgumentNullException::.ctor(System.String)
                    //IL_000d: throw
                    //IL_000e: ret

                    var ldargInstruction = Instruction.Create(OpCodes.Ldarg, parameter);
                    bodyInstructions.Add(ldargInstruction);
                    if (newBranchInstructions.Count > 0)
                    {
                        var lastBranchInstruction = newBranchInstructions[newBranchInstructions.Count - 1];
                        Debug.Assert(lastBranchInstruction.Operand == nopInstruction);
                        lastBranchInstruction.Operand = ldargInstruction;
                    }

                    var branchInstruction = Instruction.Create(OpCodes.Brtrue, nopInstruction);
                    bodyInstructions.Add(branchInstruction);
                    bodyInstructions.Add(Instruction.Create(OpCodes.Ldstr, parameter.Name));
                    bodyInstructions.Add(Instruction.Create(OpCodes.Newobj, argumentNullExceptionCtor));
                    bodyInstructions.Add(Instruction.Create(OpCodes.Throw));

                    newBranchInstructions.Add(branchInstruction);
                }
            }

            if (originalMethodInstructions != null)
            {
                int numberOfNewInstructions = bodyInstructions.Count;
                bodyInstructions.AddRange(originalMethodInstructions);

                if (newBranchInstructions.Count > 0)
                {
                    var lastBranchInstruction = newBranchInstructions[newBranchInstructions.Count - 1];
                    Debug.Assert(lastBranchInstruction.Operand == nopInstruction);
                    lastBranchInstruction.Operand = bodyInstructions[numberOfNewInstructions];
                }

                body.OptimizeMacros();
            }
        }
    }
}
