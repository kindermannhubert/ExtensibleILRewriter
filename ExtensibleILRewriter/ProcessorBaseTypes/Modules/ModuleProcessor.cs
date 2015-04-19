using ExtensibleILRewriter.Processors.Parameters;
using Mono.Cecil;

namespace ExtensibleILRewriter.ProcessorBaseTypes.Modules
{
    public abstract class ModuleProcessor<ConfigurationType> : ComponentProcessor<ModuleDefinition, AssemblyDefinition, ConfigurationType>
        where ConfigurationType : ComponentProcessorConfiguration
    {
        public ModuleProcessor([NotNull]ConfigurationType configuration, [NotNull]ILogger logger) 
            : base(configuration, logger)
        {
        }
    }
}
