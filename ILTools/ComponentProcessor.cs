using ILTools.MethodProcessors.Contracts;
using Mono.Cecil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ILTools
{
    public abstract class ComponentProcessor<ComponentType>
    {
        protected readonly ComponentProcessorProperties properties;
        protected readonly ILogger logger;

        public ComponentProcessor([NotNull]ComponentProcessorProperties properties, [NotNull]ILogger logger)
        {
            this.properties = properties;
            this.logger = logger;
        }

        public abstract void Process([NotNull]ComponentType component);
    }
}
