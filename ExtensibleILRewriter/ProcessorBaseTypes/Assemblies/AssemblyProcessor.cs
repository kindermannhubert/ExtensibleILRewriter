using ExtensibleILRewriter.Processors.Parameters;
using Mono.Cecil;

namespace ExtensibleILRewriter.ProcessorBaseTypes.Assemblies
{
    public abstract class AssemblyProcessor<ConfigurationType> : ComponentProcessor<AssemblyDefinition, NoDeclaringComponent, ConfigurationType>
        where ConfigurationType : ComponentProcessorConfiguration
    {
        public AssemblyProcessor([NotNull]ConfigurationType configuration, [NotNull]ILogger logger)
            : base(configuration, logger)
        {
        }
    }
}
