using ExtensibleILRewriter.Logging;
using ExtensibleILRewriter.Processors.Parameters;

namespace ExtensibleILRewriter.ProcessorBaseTypes.Methods
{
    public abstract class MethodProcessor<ConfigurationType> : ComponentProcessor<MethodProcessableComponent, ConfigurationType>
        where ConfigurationType : ComponentProcessorConfiguration
    {
        public MethodProcessor([NotNull]ConfigurationType configuration, [NotNull]ILogger logger)
            : base(configuration, logger)
        {
        }
    }
}
