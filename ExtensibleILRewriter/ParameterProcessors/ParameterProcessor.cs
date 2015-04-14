using ExtensibleILRewriter.Contracts;
using Mono.Cecil;

namespace ExtensibleILRewriter.ParameterProcessors
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
