using ExtensibleILRewriter.Processors.Parameters;

namespace ExtensibleILRewriter.ProcessorBaseTypes.Types
{
    public abstract class TypeProcessor<ConfigurationType> : ComponentProcessor<TypeProcessableComponent, ConfigurationType>
        where ConfigurationType : ComponentProcessorConfiguration
    {
        public TypeProcessor([NotNull]ConfigurationType configuration, [NotNull]ILogger logger)
            : base(configuration, logger)
        {
        }
    }
}
