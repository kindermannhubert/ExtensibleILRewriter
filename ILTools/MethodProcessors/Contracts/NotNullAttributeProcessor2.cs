using System.Collections.Generic;
using System.Linq;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Collections.Generic;
using ILTools.Extensions;
using Mono.Cecil.Rocks;
using System;
using System.Diagnostics;
using ILTools.MethodProcessors.Helpers;

namespace ILTools.MethodProcessors.Contracts
{
    public class NotNullAttributeProcessor2 : IComponentProcessor<MethodDefinition>
    {
        private readonly static string notNullAttributeFullName = typeof(NotNullAttribute).FullName;
        private readonly Dictionary<TypeReference, IArgumentHandlingCodeInjector> codeInjectorsCache = new Dictionary<TypeReference, IArgumentHandlingCodeInjector>();

        public void Process(MethodDefinition method, ILogger logger)
        {
            foreach (var parameter in method.Parameters)
            {
                if (parameter.CustomAttributes.Any(a => a.AttributeType.FullName == notNullAttributeFullName))
                {
                    if (parameter.ParameterType.IsValueType)
                    {
                        LogError(method, logger, "Parameter '\{parameter.Name}' of method '\{method.FullName}' cannot be non-nullable because it is a value type.");
                        continue;
                    }

                    if (!method.HasBody)
                    {
                        LogError(method, logger, "Method '\{method.FullName}' does not have body and cannot be rewritten.");
                        continue;
                    }

                    var codeInjector = GetCodeInjector(method.Module, parameter.ParameterType);
                    codeInjector.Inject(method, parameter, logger);
                }
            }
        }

        private IArgumentHandlingCodeInjector GetCodeInjector(ModuleDefinition module, TypeReference ArgumentType)
        {
            IArgumentHandlingCodeInjector codeInjector;

            if (!codeInjectorsCache.TryGetValue(ArgumentType, out codeInjector))
            {
                var parameterClrType = Type.GetType(ArgumentType.FullName);
                var codeProviderType = typeof(NotNullArgumentHandligCodeProvider<>).MakeGenericType(parameterClrType);
                var codeInjectorType = typeof(ArgumentHandligCodeInjector<>).MakeGenericType(parameterClrType);
                var codeProvider = Activator.CreateInstance(codeProviderType);

                codeInjector = (IArgumentHandlingCodeInjector)Activator.CreateInstance(codeInjectorType, new object[] { module, codeProvider });

                codeInjectorsCache.Add(ArgumentType, codeInjector);
            }

            return codeInjector;
        }

        private void LogError(MethodDefinition method, ILogger logger, string message)
        {
            var firstSequencePoint = method.HasBody ? method.Body.Instructions.FirstOrDefault(i => i.SequencePoint != null)?.SequencePoint : null;
            logger.MessageDetailed(
                LogLevel.Error,
                firstSequencePoint?.Document.Url ?? "Unknown",
                firstSequencePoint?.StartLine ?? 0, firstSequencePoint?.StartColumn ?? 0,
                firstSequencePoint?.EndLine ?? 0, firstSequencePoint?.EndColumn ?? 0,
                message);
        }
    }
}