using ExtensibleILRewriter.Logging;
using ExtensibleILRewriter.ProcessorBaseTypes;

namespace ExtensibleILRewriter.Processors.Types
{
    public class AddAttributeToType<ConfigurationType> : AddAttributeToComponent<TypeProcessableComponent, ConfigurationType>
        where ConfigurationType : AddAttributeToComponentConfiguration
    {
        public AddAttributeToType(ConfigurationType configuration, ILogger logger)
        : base(configuration, logger)
        {
        }
    }
}
