using ExtensibleILRewriter.ParameterProcessors.Contracts;
using Mono.Cecil;

namespace ExtensibleILRewriter.MethodArgumentProcessors
{
    public abstract class GenericParameterProcessor<ConfigurationType, ParameterType> : ComponentProcessor<ParameterDefinition, ConfigurationType>
        where ConfigurationType : ComponentProcessorConfiguration
    {
        public GenericParameterProcessor([NotNull]ConfigurationType configuration, [NotNull]ILogger logger)
            : base(configuration, logger)
        {
        }
    }
}
