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
using ILTools.MethodProcessors.ArgumentHandling;

namespace ILTools.MethodProcessors.Contracts
{
    public class NotNullAttributeProcessor2 : ComponentProcessor<MethodDefinition>
    {
        private readonly static string notNullAttributeFullName = typeof(NotNullAttribute).FullName;
        private readonly Dictionary<TypeReference, IArgumentHandlingCodeInjector> codeInjectorsCache = new Dictionary<TypeReference, IArgumentHandlingCodeInjector>();
        private readonly ArgumentHandlingType handlingType;

        public NotNullAttributeProcessor2([NotNull]ComponentProcessorProperties properties, [NotNull]ILogger logger)
            : base(properties, logger)
        {
            const string HandligTypeName = "HandlingType";

            if (!properties.ContainsProperty(HandligTypeName))
            {
                var message = "\{nameof(NotNullAttributeProcessor2)} processor needs '\{HandligTypeName}' element in configuration specified.";
                logger.Error(message);
                throw new InvalidOperationException(message);
            }
            if (!Enum.TryParse<ArgumentHandlingType>(properties.GetProperty(HandligTypeName), out handlingType))
            {
                var message = "Unable to parse handling type property of \{nameof(NotNullAttributeProcessor2)} processor. Value: '\{properties.GetProperty(HandligTypeName)}'.";
                logger.Error(message);
                throw new InvalidOperationException(message);
            }
        }

        public override void Process(MethodDefinition method)
        {
            foreach (var parameter in method.Parameters)
            {
                if (parameter.CustomAttributes.Any(a => a.AttributeType.FullName == notNullAttributeFullName))
                {
                    if (parameter.ParameterType.IsValueType)
                    {
                        logger.LogErrorWithSource(method, "Parameter '\{parameter.Name}' of method '\{method.FullName}' cannot be non-nullable because it is a value type.");
                        continue;
                    }

                    if (!method.HasBody)
                    {
                        logger.LogErrorWithSource(method, "Method '\{method.FullName}' does not have body and cannot be rewritten.");
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
                var codeProvider = Activator.CreateInstance(codeProviderType, handlingType);

                codeInjector = (IArgumentHandlingCodeInjector)Activator.CreateInstance(codeInjectorType, new object[] { module, codeProvider });

                codeInjectorsCache.Add(ArgumentType, codeInjector);
            }

            return codeInjector;
        }
    }
}