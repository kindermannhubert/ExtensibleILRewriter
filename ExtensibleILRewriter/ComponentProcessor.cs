using ExtensibleILRewriter.Logging;
using ExtensibleILRewriter.Processors.Parameters;

namespace ExtensibleILRewriter
{
    public abstract class ComponentProcessor<ProcessableComponentType, ConfigurationType> : IComponentProcessor<ProcessableComponentType, ConfigurationType>
        where ConfigurationType : ComponentProcessorConfiguration
        where ProcessableComponentType : IProcessableComponent
    {
        public ComponentProcessor([NotNull]ConfigurationType configuration, [NotNull]ILogger logger)
        {
            Configuration = configuration;
            Logger = logger;
        }

        public ILogger Logger { get; }

        public ConfigurationType Configuration { get; }

        public abstract void Process([NotNull]ProcessableComponentType component);
    }
}
