using ExtensibleILRewriter.Processors.Parameters;
using ExtensibleILRewriter.CodeInjection;
using ExtensibleILRewriter.Logging;

namespace ExtensibleILRewriter.Processors
{
    public class AddAttributeToComponent<ConfigurationType> : ComponentProcessor<ConfigurationType>
        where ConfigurationType : AddAttributeToComponentConfiguration
    {
        private AttributeInjector attributeInjector;

        public AddAttributeToComponent(ConfigurationType configuration, ILogger logger)
        : base(configuration, logger)
        {
            attributeInjector = new AttributeInjector(configuration.CustomAttributeProvider);

            AddSupportedComponent(ProcessableComponentType.Assembly);
            AddSupportedComponent(ProcessableComponentType.Module);
            AddSupportedComponent(ProcessableComponentType.Type);
            AddSupportedComponent(ProcessableComponentType.Method);
            AddSupportedComponent(ProcessableComponentType.MethodParameter);
        }

        public override void Process([NotNull]IProcessableComponent component)
        {
            attributeInjector.AddAttributeToComponent(component, Logger);
        }
    }
}
