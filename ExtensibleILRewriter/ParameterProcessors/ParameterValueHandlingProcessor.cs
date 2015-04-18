using Mono.Cecil;
using System.Collections.Generic;
using ExtensibleILRewriter.Contracts;
using ExtensibleILRewriter.CodeInjection;

namespace ExtensibleILRewriter.ParameterProcessors
{
    public class ParameterValueHandlingProcessor<ConfigurationType> : ParameterProcessor<ConfigurationType>
        where ConfigurationType : ParameterValueHandlingProcessorConfiguration
    {
        private readonly Dictionary<ModuleDefinition, InjectionInfo> injectionInfos = new Dictionary<ModuleDefinition, InjectionInfo>();

        public ParameterValueHandlingProcessor([NotNull]ConfigurationType configuration, [NotNull]ILogger logger)
            : base(configuration, logger)
        {
        }

        public override void Process([NotNull]ParameterDefinition parameter, MethodDefinition declaringMethod)
        {
            InjectionInfo info;
            if (!injectionInfos.TryGetValue(declaringMethod.Module, out info))
            {
                info = new InjectionInfo();
                var codeProvider = Configuration.CustomValueHandlingCodeProvider;
                info.CodeInjector = new CodeInjector<ParameterValueHandlingCodeProviderArgument>(declaringMethod.Module, codeProvider);
                info.StateHoldingField = PrepareStateHoldingField(codeProvider, declaringMethod.Module);

                injectionInfos.Add(declaringMethod.Module, info);
            }

            info.CodeInjector.Inject(declaringMethod, new ParameterValueHandlingCodeProviderArgument(parameter, declaringMethod, info.StateHoldingField), logger);
        }

        private FieldDefinition PrepareStateHoldingField(CodeProvider<ParameterValueHandlingCodeProviderArgument> codeProvider, ModuleDefinition module)
        {
            if (codeProvider.HasState)
            {
                return HandlingInstancesCodeGenerator.PrepareInstanceHoldingField(module, codeProvider.GetStateType(), Configuration.StateInstanceName, Configuration.StateInstanceName);
            }
            else
            {
                return null;
            }
        }

        class InjectionInfo
        {
            public CodeInjector<ParameterValueHandlingCodeProviderArgument> CodeInjector;
            public FieldDefinition StateHoldingField;
        }
    }
}
