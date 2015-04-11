using ExtensibleILRewriter.ParameterProcessors.Contracts;
using Mono.Cecil;

namespace ExtensibleILRewriter.MethodArgumentProcessors
{
    public abstract class GeneralParameterProcessor<ConfigurationType> : ComponentProcessor<ParameterDefinition, ConfigurationType>
        where ConfigurationType : ComponentProcessorConfiguration
    {
        public GeneralParameterProcessor([NotNull]ConfigurationType configuration, [NotNull]ILogger logger) 
            : base(configuration, logger)
        {
        }
    }
}
