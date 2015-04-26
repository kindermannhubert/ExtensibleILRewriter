using ExtensibleILRewriter.ProcessorBaseTypes;
using Mono.Cecil;

namespace ExtensibleILRewriter.Processors.Assemblies
{
    public class AddAssemblyAttribute<ConfigurationType> : AddComponentAttribute<AssemblyDefinition, NoDeclaringComponent, ConfigurationType>
        where ConfigurationType : AddAssemblyAttributeConfiguration
    {
        public AddAssemblyAttribute(ConfigurationType configuration, ILogger logger)
        : base(configuration, logger)
        {
        }
    }
}
