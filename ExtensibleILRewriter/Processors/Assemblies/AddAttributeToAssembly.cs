using ExtensibleILRewriter.ProcessorBaseTypes;

namespace ExtensibleILRewriter.Processors.Assemblies
{
    public class AddAttributeToAssembly<ConfigurationType> : AddAttributeToComponent<AssemblyProcessableComponent, ConfigurationType>
        where ConfigurationType : AddAttributeToAssemblyConfiguration
    {
        public AddAttributeToAssembly(ConfigurationType configuration, ILogger logger)
        : base(configuration, logger)
        {
        }
    }
}
