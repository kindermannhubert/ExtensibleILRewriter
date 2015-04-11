using ExtensibleILRewriter.ParameterProcessors.Contracts;
using Mono.Cecil;

namespace ExtensibleILRewriter.MethodProcessors
{
    public abstract class MethodProcessor<ConfigurationType> : ComponentProcessor<MethodDefinition, ConfigurationType>
        where ConfigurationType : ComponentProcessorConfiguration
    {
        public MethodProcessor([NotNull]ConfigurationType configuration, [NotNull]ILogger logger)
            : base(configuration, logger)
        {
        }
    }
}
