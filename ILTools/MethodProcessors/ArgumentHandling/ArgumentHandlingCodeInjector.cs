using System.Collections.Generic;
using System.Linq;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Collections.Generic;
using ILTools.Extensions;
using Mono.Cecil.Rocks;
using System;
using System.Diagnostics;

namespace ILTools.MethodProcessors.ArgumentHandling
{
    public class ArgumentHandligCodeInjector<ArgumentType> : IArgumentHandlingCodeInjector
    {
        private readonly IArgumentHandlingCodeProvider<ArgumentType> codeProvider;
        private readonly MethodDefinition handleParameterMethodDefinition;
        private readonly MethodReference handleParameterMethodImportedReference;
        private readonly Collection<Instruction> oldInstructions = new Collection<Instruction>();

        public ArgumentHandligCodeInjector(ModuleDefinition module, IArgumentHandlingCodeProvider<ArgumentType> codeProvider)
        {
            this.codeProvider = codeProvider;

            codeProvider.CheckPrerequisites();

            //if (codeProvider.MakeArgumentHandligMethodStatic)
            //{
            //    //var codeProviderModule = ModuleDefinition.ReadModule(codeProvider.GetType().Module.FullyQualifiedName);
                
            //    //PrepareStaticVersionOfHandling(module);

            //    //throw new NotImplementedException();
            //}
            //else
            {
                Action<ArgumentType, string> handleParameterMethodDelegate = codeProvider.HandleArgument;
                handleParameterMethodImportedReference = module.Import(handleParameterMethodDelegate.Method);
                handleParameterMethodDefinition = handleParameterMethodImportedReference.Resolve();
            }

            CheckHandleParameterMethod();
        }

        public void Inject(MethodDefinition method, ParameterDefinition parameter, ILogger logger)
        {
            if (!method.HasBody) throw new ArgumentException("Method does not contain body.");
            if (parameter.ParameterType.FullName != typeof(ArgumentType).FullName) throw new ArgumentException("Parameter type of \{nameof(ArgumentHandligCodeInjector)} does not match type of argument.", nameof(parameter));

            logger.Notice("Injecting parameter handlig to method '\{method.FullName}' of parameter '\{parameter.Name}'.");

            if (codeProvider.MakeArgumentHandligMethodStatic)
            {
                CallStaticHandlingFromProvider(method, parameter);
            }
            else CallNonStaticHandlingFromProvider(method, parameter);

            //InlineHandling(method, parameter);
        }

        private void CallNonStaticHandlingFromProvider(MethodDefinition method, ParameterDefinition parameter)
        {
            oldInstructions.Clear();
            method.Body.SimplifyMacros();
            oldInstructions.AddRange(method.Body.Instructions);

            method.Body.Instructions.Clear();
            //method.Body.Instructions.Add(Instruction.Create(OpCodes.Ldarg, codeProvider.HandlingObject)); //TODO: make correct 'this' object loading
            method.Body.Instructions.Add(Instruction.Create(OpCodes.Ldarg, parameter));
            method.Body.Instructions.Add(Instruction.Create(OpCodes.Ldstr, parameter.Name));
            method.Body.Instructions.Add(Instruction.Create(OpCodes.Call, handleParameterMethodImportedReference));
            method.Body.Instructions.AddRange(oldInstructions);

            method.Body.OptimizeMacros();
        }

        private void CallStaticHandlingFromProvider(MethodDefinition method, ParameterDefinition parameter)
        {
            oldInstructions.Clear();
            method.Body.SimplifyMacros();
            oldInstructions.AddRange(method.Body.Instructions);

            method.Body.Instructions.Clear();
            //TODO - load 'this' arg
            method.Body.Instructions.Add(Instruction.Create(OpCodes.Ldarg, parameter));
            method.Body.Instructions.Add(Instruction.Create(OpCodes.Ldstr, parameter.Name));
            method.Body.Instructions.Add(Instruction.Create(OpCodes.Call, handleParameterMethodImportedReference));
            method.Body.Instructions.AddRange(oldInstructions);

            method.Body.OptimizeMacros();
        }

        private void InlineHandling(MethodDefinition method, ParameterDefinition parameter)
        {
            //TODO - argument indexing must be repaired here
        }

        private void CheckHandleParameterMethod()
        {
            if (!handleParameterMethodDefinition.HasBody) throw new InvalidOperationException("Code provider's method '\{handleParameterMethodDefinition.FullName}' does not have body.");
        }

        //private void PrepareStaticVersionOfHandling(ModuleDefinition module)
        //{
        //    if (!handleParameterMethodDefinition.CouldBeStatic()) throw new InvalidOperationException("Code provider's method '\{handleParameterMethodDefinition.FullName}' must be able to be static.");

        //    var staticHandlingMethodDefinition = handleParameterMethodDefinition.CreateStaticVersion();
        //    staticHandlingMethodDefinition.Name = "\{handleParameterMethodDefinition.DeclaringType?.FullName}__static_\{handleParameterMethodDefinition.Name}";
        //    handleParameterMethodDefinition = staticHandlingMethodDefinition;
        //}
    }
}
