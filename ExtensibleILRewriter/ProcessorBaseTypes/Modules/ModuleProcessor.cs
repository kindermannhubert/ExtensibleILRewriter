using ExtensibleILRewriter.Processors.Parameters;

namespace ExtensibleILRewriter.ProcessorBaseTypes.Modules
{
    public abstract class ModuleProcessor<ConfigurationType> : ComponentProcessor<ModuleProcessableComponent, ConfigurationType>
        where ConfigurationType : ComponentProcessorConfiguration
    {
        public ModuleProcessor([NotNull]ConfigurationType configuration, [NotNull]ILogger logger) 
            : base(configuration, logger)
        {
        }
    }
}
