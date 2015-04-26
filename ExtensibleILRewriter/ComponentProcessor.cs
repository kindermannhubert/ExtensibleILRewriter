using ExtensibleILRewriter.Processors.Parameters;

namespace ExtensibleILRewriter
{
    public abstract class ComponentProcessor<ProcessableComponentType, ConfigurationType> : IComponentProcessor<ProcessableComponentType, ConfigurationType>
        where ConfigurationType : ComponentProcessorConfiguration
        where ProcessableComponentType : IProcessableComponent
    {
        protected readonly ILogger logger;

        public ConfigurationType Configuration { get; }

        public ComponentProcessor([NotNull]ConfigurationType configuration, [NotNull]ILogger logger)
        {
            this.Configuration = configuration;
            this.logger = logger;
        }

        public abstract void Process([NotNull]ProcessableComponentType component);
    }
}
