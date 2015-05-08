using ExtensibleILRewriter.Logging;
using ExtensibleILRewriter.ProcessorBaseTypes;

namespace ExtensibleILRewriter.Processors.Methods
{
    public class AddAttributeToMethod<ConfigurationType> : AddAttributeToComponent<MethodProcessableComponent, ConfigurationType>
        where ConfigurationType : AddAttributeToComponentConfiguration
    {
        public AddAttributeToMethod(ConfigurationType configuration, ILogger logger)
        : base(configuration, logger)
        {
        }
    }
}
