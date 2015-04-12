using ExtensibleILRewriter.Extensions;
using Mono.Cecil;
using System;
using System.Reflection;

namespace ExtensibleILRewriter.ParameterProcessors.Contracts
{
    public class NotNullArgumentHandligCodeProvider : ParameterValueHandlingCodeProvider<EmptyCodeProviderState>
    {
        public override bool ShouldHandleParameter(ParameterDefinition parameterDefinition, MethodDefinition declaringMethod)
        {
            //return true;

            //TODO - temporary for testing
            return parameterDefinition.ParameterType.IsValueType.Implies(parameterDefinition.ParameterType.IsNullableValueType());
        }

        public static void HandleParameter<ParameterType>(EmptyCodeProviderState state, ParameterType parameter, string parameterName)
        {
            if (parameter == null) throw new ArgumentNullException(parameterName);
        }

        public override MethodInfo GetHandleParameterMethodInfo(TypeReference parameterType)
        {
            return base.GetHandleParameterMethodInfo(nameof(HandleParameter));
        }
    }
}
