using ExtensibleILRewriter.Processors.Parameters;
using ExtensibleILRewriter.CodeInjection;

namespace ExtensibleILRewriter.ProcessorBaseTypes
{
    public class AddAttributeToComponent<ProcessableComponentType, ConfigurationType> : ComponentProcessor<ProcessableComponentType, ConfigurationType>
        where ProcessableComponentType : IProcessableComponent
        where ConfigurationType : AddAttributeToComponentConfiguration
    {
        private AttributeInjector attributeInjector;

        public AddAttributeToComponent(ConfigurationType configuration, ILogger logger)
        : base(configuration, logger)
        {
            attributeInjector = new AttributeInjector(configuration.CustomAttributeProvider);
        }

        public override void Process([NotNull]ProcessableComponentType component)
        {
            attributeInjector.AddAttributeToComponent(component, logger);
        }
    }
}
