using ExtensibleILRewriter.Processors.Parameters;
using System.Collections.Generic;

namespace ExtensibleILRewriter
{
    public interface IComponentProcessor<out ConfigurationType>
        where ConfigurationType : ComponentProcessorConfiguration
    {
        IReadOnlyCollection<ProcessableComponentType> SupportedComponents { get; }

        void Process([NotNull]IProcessableComponent component);
    }
}
