using Mono.Cecil;

namespace ExtensibleILRewriter.ProcessorBaseTypes.Parameters
{
    public struct ParameterValueHandlingCodeProviderArgument
    {
        public ParameterDefinition Parameter { get; }
        public MethodDefinition Method { get; }
        public FieldDefinition StateField { get; }

        public ParameterValueHandlingCodeProviderArgument(ParameterDefinition parameter, MethodDefinition method, FieldDefinition stateField)
        {
            Parameter = parameter;
            Method = method;
            StateField = stateField;
        }
    }
}
