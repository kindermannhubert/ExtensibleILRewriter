using ExtensibleILRewriter.ParameterProcessors.Contracts;
using Mono.Cecil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExtensibleILRewriter
{
    public interface IComponentProcessor<ComponentType, out ConfigurationType>
        where ConfigurationType : ComponentProcessorConfiguration
    {
        void Process([NotNull]ComponentType component);
    }
}
