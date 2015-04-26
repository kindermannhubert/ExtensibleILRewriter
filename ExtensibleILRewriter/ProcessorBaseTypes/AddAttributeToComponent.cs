using ExtensibleILRewriter.Processors.Parameters;
using ExtensibleILRewriter.CodeInjection;

namespace ExtensibleILRewriter.ProcessorBaseTypes
{
    public class AddAttributeToComponent<ProcessableComponentType, ConfigurationType> : ComponentProcessor<ProcessableComponentType, ConfigurationType>
        where ProcessableComponentType : IProcessableComponent
        where ConfigurationType : AddAttributeToComponentConfiguration<ProcessableComponentType>
    {
        private AttributeInjector<ProcessableComponentType> attributeInjector;

        public AddAttributeToComponent(ConfigurationType configuration, ILogger logger)
        : base(configuration, logger)
        {
            attributeInjector = new AttributeInjector<ProcessableComponentType>(configuration.CustomAttributeProvider);
        }

        public override void Process([NotNull]ProcessableComponentType component)
        {
            attributeInjector.AddAttributeToComponent(component, component, logger);
        }
    }
}
