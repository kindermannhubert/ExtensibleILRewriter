using ExtensibleILRewriter.Processors.Parameters;
using Mono.Cecil;

namespace ExtensibleILRewriter.ProcessorBaseTypes.Types
{
    public abstract class TypeProcessor<ConfigurationType> : ComponentProcessor<TypeDefinition, ModuleDefinition, ConfigurationType>
        where ConfigurationType : ComponentProcessorConfiguration
    {
        public TypeProcessor([NotNull]ConfigurationType configuration, [NotNull]ILogger logger) 
            : base(configuration, logger)
        {
        }
    }
}
