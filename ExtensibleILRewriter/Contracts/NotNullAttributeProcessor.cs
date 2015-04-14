using System.Linq;
using Mono.Cecil;
using ExtensibleILRewriter.Extensions;
using ExtensibleILRewriter.ParameterProcessors;

namespace ExtensibleILRewriter.Contracts
{
    public class NotNullAttributeProcessor : ParameterValueHandlingProcessor<NotNullAttributeProcessor.ProcessorConfiguration>
    {
        private readonly static string notNullAttributeFullName = typeof(NotNullAttribute).FullName;

        public NotNullAttributeProcessor([NotNull]ProcessorConfiguration configuration, [NotNull]ILogger logger)
            : base(configuration, logger)
        {
        }

        public override void Process(ParameterDefinition parameter, MethodDefinition declaringMethod)
        {
            if (parameter.CustomAttributes.Any(a => a.AttributeType.FullName == notNullAttributeFullName))
            {
                var method = parameter.Method as MethodDefinition;
                if (method == null)
                {
                    logger.Error("Unable to get MethodDefinition from parameter '\{parameter.Name}' of method '\{method.FullName}'.");
                    return;
                }

                if (parameter.ParameterType.IsValueType && !parameter.ParameterType.IsNullableValueType())
                {
                    logger.LogErrorWithSource(method, "Parameter '\{parameter.Name}' of method '\{method.FullName}' cannot be non-nullable because it is a value type.");
                    return;
                }

                base.Process(parameter, declaringMethod);
            }
        }

        public class ProcessorConfiguration : ParameterValueHandlingProcessorConfiguration
        {
            protected override IParameterValueHandlingCodeProvider GetDefaultCodeProvider()
            {
                return new NotNullArgumentHandligCodeProvider();
            }
        }
    }
}