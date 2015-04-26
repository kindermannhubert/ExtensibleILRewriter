using ExtensibleILRewriter.Processors.Parameters;

namespace ExtensibleILRewriter.ProcessorBaseTypes.Parameters
{
    public abstract class ParameterProcessor<ConfigurationType> : ComponentProcessor<MethodParameterProcessableComponent, ConfigurationType>
        where ConfigurationType : ComponentProcessorConfiguration
    {
        public ParameterProcessor([NotNull]ConfigurationType configuration, [NotNull]ILogger logger) 
            : base(configuration, logger)
        {
        }
    }
}
