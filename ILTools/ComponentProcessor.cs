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

        public ComponentProcessor([NotNull]ComponentProcessorProperties properties)
        {
            this.properties = properties;
        }

        public abstract void Process([NotNull]ComponentType component, [NotNull]ILogger logger);
    }
}
