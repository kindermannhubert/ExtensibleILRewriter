using ExtensibleILRewriter.CodeInjection;
using ExtensibleILRewriter.ProcessorBaseTypes.Parameters;
using Mono.Cecil;
using System;

namespace ExtensibleILRewriter.Processors.Parameters
{
    public class NotNullArgumentHandligCodeProvider : CodeProvider<ParameterValueHandlingCodeProviderArgument>
    {
        public override bool HasState { get { return false; } }

        protected override bool ShouldBeInjected(ParameterValueHandlingCodeProviderArgument codeProviderArgument)
        {
            return true;
        }

        protected override string GetCodeProvidingMethodName(ParameterValueHandlingCodeProviderArgument codeProviderArgument)
        {
            return nameof(HandleParameter);
        }

        protected override CodeProviderCallArgument[] GetCodeProvidingMethodArguments(ParameterValueHandlingCodeProviderArgument codeProviderArgument)
        {
            return new CodeProviderCallArgument[]
            {
                CodeProviderCallArgument.CreateGenericParameterArgument("parameter", codeProviderArgument.Parameter),
                CodeProviderCallArgument.CreateTextArgument("parameterName", codeProviderArgument.Parameter.Name)
            };
        }

        public static void HandleParameter<ParameterType>(ParameterType parameter, string parameterName)
        {
            if (parameter == null) throw new ArgumentNullException(parameterName);
        }

        protected override TypeReference[] GetCodeProvidingMethodGenericArgumentTypes(ParameterValueHandlingCodeProviderArgument codeProviderArgument)
        {
            return new TypeReference[] { codeProviderArgument.Parameter.ParameterType };
        }
    }
}
