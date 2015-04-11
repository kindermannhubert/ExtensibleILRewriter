using ExtensibleILRewriter.ParameterProcessors.Contracts;
using Mono.Cecil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExtensibleILRewriter
{
    public abstract class ComponentProcessor<ComponentType, ConfigurationType> : IComponentProcessor<ComponentType, ConfigurationType>
        where ConfigurationType : ComponentProcessorConfiguration
    {
        protected readonly ConfigurationType configuration;
        protected readonly ILogger logger;

        public ComponentProcessor([NotNull]ConfigurationType configuration, [NotNull]ILogger logger)
        {
            this.configuration = configuration;
            this.logger = logger;
        }

        public abstract void Process([NotNull]ComponentType component);
    }
}
