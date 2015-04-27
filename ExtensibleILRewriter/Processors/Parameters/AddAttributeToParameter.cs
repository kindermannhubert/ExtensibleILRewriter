using ExtensibleILRewriter.ProcessorBaseTypes;

namespace ExtensibleILRewriter.Processors.Parameters
{
    public class AddAttributeToParameter<ConfigurationType> : AddAttributeToComponent<MethodParameterProcessableComponent, ConfigurationType>
        where ConfigurationType : AddAttributeToComponentConfiguration
    {
        public AddAttributeToParameter(ConfigurationType configuration, ILogger logger)
        : base(configuration, logger)
        {
        }
    }
}
