using ExtensibleILRewriter.ParameterProcessors.Contracts;
using Mono.Cecil;

namespace ExtensibleILRewriter.AssemblyProcessors
{
    public abstract class AssemblyProcessor<ConfigurationType> : ComponentProcessor<AssemblyDefinition, ConfigurationType>
        where ConfigurationType : ComponentProcessorConfiguration
    {
        public AssemblyProcessor([NotNull]ConfigurationType configuration, [NotNull]ILogger logger)
            : base(configuration, logger)
        {
        }
    }
}
