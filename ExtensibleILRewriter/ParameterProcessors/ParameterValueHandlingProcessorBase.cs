using ExtensibleILRewriter.Contracts;

namespace ExtensibleILRewriter.ParameterProcessors
{
    public abstract class ParameterValueHandlingProcessorBase<ConfigurationType> : ParameterProcessor<ConfigurationType>
        where ConfigurationType : ParameterValueHandlingProcessorConfiguration
    {
        public ParameterValueHandlingProcessorBase([NotNull]ConfigurationType configuration, [NotNull]ILogger logger)
            : base(configuration, logger)
        {
        }
    }
}
