using ExtensibleILRewriter.ProcessorBaseTypes;

namespace ExtensibleILRewriter.Processors.Assemblies
{
    public class AddAttributeToAssembly<ConfigurationType> : AddAttributeToComponent<AssemblyProcessableComponent, ConfigurationType>
        where ConfigurationType : AddAttributeToComponentConfiguration
    {
        public AddAttributeToAssembly(ConfigurationType configuration, ILogger logger)
        : base(configuration, logger)
        {
        }
    }
}
