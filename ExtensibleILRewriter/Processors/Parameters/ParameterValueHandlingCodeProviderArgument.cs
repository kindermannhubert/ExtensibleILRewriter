using Mono.Cecil;

namespace ExtensibleILRewriter.Processors.Parameters
{
    public struct ParameterValueHandlingCodeProviderArgument
    {
        public ParameterValueHandlingCodeProviderArgument(MethodParameterProcessableComponent parameter, FieldDefinition stateField)
        {
            Parameter = parameter;
            StateField = stateField;
        }

        public MethodParameterProcessableComponent Parameter { get; }

        public FieldDefinition StateField { get; }
    }
}
