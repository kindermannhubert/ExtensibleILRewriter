﻿using System.Linq;
using Mono.Cecil;
using ExtensibleILRewriter.Extensions;
using ExtensibleILRewriter.ProcessorBaseTypes.Parameters;
using ExtensibleILRewriter.CodeInjection;
using System;

namespace ExtensibleILRewriter.Processors.Parameters
{
    public class NotNullAttributeProcessor : ParameterValueHandlingProcessor<NotNullAttributeProcessor.ProcessorConfiguration>
    {
        private readonly static string notNullAttributeFullName = typeof(NotNullAttribute).FullName;

        public NotNullAttributeProcessor([NotNull]ProcessorConfiguration configuration, [NotNull]ILogger logger)
            : base(configuration, logger)
        {
        }

        public override void Process(MethodParameterProcessableComponent parameter)
        {
            var parameterDefinition = parameter.UnderlyingComponent;
            if (parameter.CustomAttributes.Any(a => a.AttributeType.FullName == notNullAttributeFullName))
            {
                var methodDefinition = parameter.DeclaringComponent.UnderlyingComponent;

                if (parameterDefinition.ParameterType.IsValueType && !parameterDefinition.ParameterType.IsNullableValueType())
                {
                    logger.LogErrorWithSource(methodDefinition, "Parameter '\{parameter.Name}' of method '\{methodDefinition.FullName}' cannot be non-nullable because it is a value type.");
                    return;
                }

                base.Process(parameter);
            }
        }

        public class ProcessorConfiguration : ParameterValueHandlingProcessorConfiguration
        {
            protected override CodeProvider<ParameterValueHandlingCodeProviderArgument> GetDefaultCodeProvider()
            {
                return new DefaultNotNullArgumentHandligCodeProvider();
            }
        }

        public class DefaultNotNullArgumentHandligCodeProvider : CodeProvider<ParameterValueHandlingCodeProviderArgument>
        {
            public override bool HasState { get { return false; } }

            protected override bool ShouldBeInjected(ParameterValueHandlingCodeProviderArgument codeProviderArgument)
            {
                return true;
            }

            protected override string GetCodeProvidingMethodName(ParameterValueHandlingCodeProviderArgument codeProviderArgument)
            {
                return nameof(HandleParameter);
            }

            protected override CodeProviderCallArgument[] GetCodeProvidingMethodArguments(ParameterValueHandlingCodeProviderArgument codeProviderArgument)
            {
                return new CodeProviderCallArgument[]
                {
                CodeProviderCallArgument.CreateGenericParameterArgument("parameter", codeProviderArgument.Parameter),
                CodeProviderCallArgument.CreateTextArgument("parameterName", codeProviderArgument.Parameter.Name)
                };
            }

            public static void HandleParameter<ParameterType>(ParameterType parameter, string parameterName)
            {
                if (parameter == null) throw new ArgumentNullException(parameterName);
            }

            protected override TypeReference[] GetCodeProvidingMethodGenericArgumentTypes(ParameterValueHandlingCodeProviderArgument codeProviderArgument)
            {
                return new TypeReference[] { codeProviderArgument.Parameter.ParameterType };
            }
        }
    }
}