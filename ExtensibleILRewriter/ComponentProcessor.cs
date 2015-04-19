using ExtensibleILRewriter.Processors.Parameters;

namespace ExtensibleILRewriter
{
    public abstract class ComponentProcessor<ComponentType, DeclaringComponentType, ConfigurationType> : IComponentProcessor<ComponentType, DeclaringComponentType, ConfigurationType>
        where ConfigurationType : ComponentProcessorConfiguration
    {
        protected readonly ILogger logger;

        public ConfigurationType Configuration { get; }

        public ComponentProcessor([NotNull]ConfigurationType configuration, [NotNull]ILogger logger)
        {
            this.Configuration = configuration;
            this.logger = logger;
        }

        public abstract void Process([NotNull]ComponentType component, DeclaringComponentType declaringComponent);
    }
}
