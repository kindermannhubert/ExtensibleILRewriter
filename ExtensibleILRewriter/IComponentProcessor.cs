using ExtensibleILRewriter.Processors.Parameters;

namespace ExtensibleILRewriter
{
    public interface IComponentProcessor<ComponentType, DeclaringComponentType, out ConfigurationType>
        where ConfigurationType : ComponentProcessorConfiguration
    {
        void Process([NotNull]ComponentType component, DeclaringComponentType declaringComponent);
    }

    public class NoDeclaringComponent { }
}
