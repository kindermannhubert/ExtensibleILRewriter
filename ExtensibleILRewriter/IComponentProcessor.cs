using ExtensibleILRewriter.Processors.Parameters;

namespace ExtensibleILRewriter
{
    public interface IComponentProcessor<ProcessableComponentType, out ConfigurationType>
        where ConfigurationType : ComponentProcessorConfiguration
        where ProcessableComponentType : IProcessableComponent
    {
        void Process([NotNull]ProcessableComponentType component);
    }
}
