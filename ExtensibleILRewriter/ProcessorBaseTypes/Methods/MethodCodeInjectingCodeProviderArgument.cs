using Mono.Cecil;

namespace ExtensibleILRewriter.ProcessorBaseTypes.Methods
{
    public struct MethodCodeInjectingCodeProviderArgument
    {
        public MethodCodeInjectingCodeProviderArgument(MethodDefinition method, FieldDefinition stateField)
        {
            Method = method;
            StateField = stateField;
        }

        public MethodDefinition Method { get; }

        public FieldDefinition StateField { get; }
    }
}
