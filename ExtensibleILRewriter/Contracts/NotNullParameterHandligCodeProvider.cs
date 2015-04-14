using ExtensibleILRewriter.ParameterProcessors;
using Mono.Cecil;
using System;

namespace ExtensibleILRewriter.Contracts
{
    public class NotNullArgumentHandligCodeProvider : ParameterValueHandlingCodeProvider<EmptyCodeProviderState>
    {
        public override bool ShouldHandleParameter(ParameterDefinition parameterDefinition, MethodDefinition declaringMethod)
        {
            return true;
        }

        public static void HandleParameter<ParameterType>(EmptyCodeProviderState state, ParameterType parameter, string parameterName)
        {
            if (parameter == null) throw new ArgumentNullException(parameterName);
        }

        protected override string GetHandleParameterMethodName(TypeReference parameterType)
        {
            return nameof(HandleParameter);
        }
    }
}
