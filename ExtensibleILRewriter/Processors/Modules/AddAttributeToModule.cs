using ExtensibleILRewriter.ProcessorBaseTypes;

namespace ExtensibleILRewriter.Processors.Modules
{
    public class AddAttributeToModule<ConfigurationType> : AddAttributeToComponent<ModuleProcessableComponent, ConfigurationType>
        where ConfigurationType : AddAttributeToComponentConfiguration
    {
        public AddAttributeToModule(ConfigurationType configuration, ILogger logger)
        : base(configuration, logger)
        {
        }
    }
}
