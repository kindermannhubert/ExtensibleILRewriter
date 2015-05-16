using Mono.Cecil;

namespace ExtensibleILRewriter.Processors.Methods
{
    public struct MethodCodeInjectingCodeProviderArgument
    {
        public MethodCodeInjectingCodeProviderArgument(MethodProcessableComponent method, FieldDefinition stateField)
        {
            Method = method;
            StateField = stateField;
        }

        public MethodProcessableComponent Method { get; }

        public FieldDefinition StateField { get; }
    }
}
