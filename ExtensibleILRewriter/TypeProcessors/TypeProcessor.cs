using ExtensibleILRewriter.ParameterProcessors.Contracts;
using Mono.Cecil;

namespace ExtensibleILRewriter.TypeProcessors
{
    public abstract class TypeProcessor<ConfigurationType> : ComponentProcessor<TypeDefinition, ConfigurationType>
        where ConfigurationType : ComponentProcessorConfiguration
    {
        public TypeProcessor([NotNull]ConfigurationType configuration, [NotNull]ILogger logger) 
            : base(configuration, logger)
        {
        }
    }
}
