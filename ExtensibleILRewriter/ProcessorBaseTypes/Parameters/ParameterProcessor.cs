using ExtensibleILRewriter.Processors.Parameters;
using Mono.Cecil;

namespace ExtensibleILRewriter.ProcessorBaseTypes.Parameters
{
    public abstract class ParameterProcessor<ConfigurationType> : ComponentProcessor<ParameterDefinition, MethodDefinition, ConfigurationType>
        where ConfigurationType : ComponentProcessorConfiguration
    {
        public ParameterProcessor([NotNull]ConfigurationType configuration, [NotNull]ILogger logger) 
            : base(configuration, logger)
        {
        }
    }
}
