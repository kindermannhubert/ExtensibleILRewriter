using ExtensibleILRewriter.Processors.Parameters;
using Mono.Cecil;

namespace ExtensibleILRewriter.ProcessorBaseTypes.Methods
{
    public abstract class MethodProcessor<ConfigurationType> : ComponentProcessor<MethodDefinition, TypeDefinition, ConfigurationType>
        where ConfigurationType : ComponentProcessorConfiguration
    {
        public MethodProcessor([NotNull]ConfigurationType configuration, [NotNull]ILogger logger)
            : base(configuration, logger)
        {
        }
    }
}
