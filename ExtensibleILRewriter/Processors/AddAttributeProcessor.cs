using ExtensibleILRewriter.Processors.Parameters;
using ExtensibleILRewriter.CodeInjection;
using ExtensibleILRewriter.Logging;

namespace ExtensibleILRewriter.Processors
{
    public class AddAttributeProcessor<ConfigurationType> : ComponentProcessor<ConfigurationType>
        where ConfigurationType : AddAttributeProcessorConfiguration
    {
        private AttributeInjector attributeInjector;

        public AddAttributeProcessor(ConfigurationType configuration, ILogger logger)
        : base(configuration, logger)
        {
            attributeInjector = new AttributeInjector(configuration.CustomAttributeProvider);

            AddSupportedComponent(ProcessableComponentType.Assembly);
            AddSupportedComponent(ProcessableComponentType.Module);
            AddSupportedComponent(ProcessableComponentType.Type);
            AddSupportedComponent(ProcessableComponentType.Field);
            AddSupportedComponent(ProcessableComponentType.Property);
            AddSupportedComponent(ProcessableComponentType.Method);
            AddSupportedComponent(ProcessableComponentType.MethodParameter);
        }

        public override void Process([NotNull]IProcessableComponent component)
        {
            attributeInjector.AddAttributeProcessor(component, Logger);
        }
    }
}
