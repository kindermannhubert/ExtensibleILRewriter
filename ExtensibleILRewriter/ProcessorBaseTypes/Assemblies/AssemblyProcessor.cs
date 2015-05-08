using ExtensibleILRewriter.Logging;
using ExtensibleILRewriter.Processors.Parameters;

namespace ExtensibleILRewriter.ProcessorBaseTypes.Assemblies
{
    public abstract class AssemblyProcessor<ConfigurationType> : ComponentProcessor<AssemblyProcessableComponent, ConfigurationType>
        where ConfigurationType : ComponentProcessorConfiguration
    {
        public AssemblyProcessor([NotNull]ConfigurationType configuration, [NotNull]ILogger logger)
            : base(configuration, logger)
        {
        }
    }
}
