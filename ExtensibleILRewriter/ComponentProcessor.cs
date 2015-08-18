using ExtensibleILRewriter.Logging;
using ExtensibleILRewriter.Processors.Parameters;
using System;
using System.Collections.Generic;

namespace ExtensibleILRewriter
{
    public abstract class ComponentProcessor<ConfigurationType> : IComponentProcessor<ConfigurationType>
        where ConfigurationType : ComponentProcessorConfiguration
    {
        private readonly List<ProcessableComponentType> supportedComponents = new List<ProcessableComponentType>();

        public ComponentProcessor([NotNull]ConfigurationType configuration, [NotNull]ILogger logger)
        {
            Configuration = configuration;
            Logger = logger;
        }

        public ILogger Logger { get; }

        public ConfigurationType Configuration { get; }

        public IReadOnlyCollection<ProcessableComponentType> SupportedComponents { get { return supportedComponents; } }

        public virtual void Process([NotNull]IProcessableComponent component)
        {
            throw new NotImplementedException();
        }

        protected void AddSupportedComponent(ProcessableComponentType componentType)
        {
            supportedComponents.Add(componentType);
        }
    }
}
