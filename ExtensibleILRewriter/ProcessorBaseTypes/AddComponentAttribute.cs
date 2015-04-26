using ExtensibleILRewriter.Processors.Parameters;
using Mono.Cecil;
using ExtensibleILRewriter.CodeInjection;

namespace ExtensibleILRewriter.ProcessorBaseTypes
{
    public class AddComponentAttribute<ComponentType, DeclaringComponentType, ConfigurationType> : ComponentProcessor<ComponentType, DeclaringComponentType, ConfigurationType>
        where ConfigurationType : AddComponentAttributeConfiguration<ComponentType>
    {
        private AttributeInjector<ComponentType> attributeInjector;

        public AddComponentAttribute(ConfigurationType configuration, ILogger logger)
        : base(configuration, logger)
        {
            attributeInjector = new AttributeInjector<ComponentType>(configuration.CustomAttributeProvider);
        }

        public override void Process([NotNull]ComponentType component, DeclaringComponentType declaringComponent)
        {
            attributeInjector.AddAttributeToComponent(component, component, logger);
        }
    }
}
