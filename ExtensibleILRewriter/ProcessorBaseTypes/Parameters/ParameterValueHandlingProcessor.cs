using Mono.Cecil;
using System.Collections.Generic;
using ExtensibleILRewriter.Processors.Parameters;
using ExtensibleILRewriter.CodeInjection;

namespace ExtensibleILRewriter.ProcessorBaseTypes.Parameters
{
    public class ParameterValueHandlingProcessor<ConfigurationType> : ParameterProcessor<ConfigurationType>
        where ConfigurationType : ParameterValueHandlingProcessorConfiguration
    {
        private readonly Dictionary<ModuleDefinition, ModuleData> modulesData = new Dictionary<ModuleDefinition, ModuleData>();

        public ParameterValueHandlingProcessor([NotNull]ConfigurationType configuration, [NotNull]ILogger logger)
            : base(configuration, logger)
        {
        }

        public override void Process([NotNull]ParameterDefinition parameter, MethodDefinition declaringMethod)
        {
            ModuleData moduleData;
            if (!modulesData.TryGetValue(declaringMethod.Module, out moduleData))
            {
                moduleData = new ModuleData();
                var codeProvider = Configuration.CustomValueHandlingCodeProvider;
                moduleData.CodeInjector = new CodeInjector<ParameterValueHandlingCodeProviderArgument>(declaringMethod.Module, codeProvider);
                moduleData.StateHoldingField = PrepareStateHoldingField(codeProvider, declaringMethod.Module);

                modulesData.Add(declaringMethod.Module, moduleData);
            }

            moduleData.CodeInjector.InjectAtBegining(declaringMethod, new ParameterValueHandlingCodeProviderArgument(parameter, declaringMethod, moduleData.StateHoldingField), logger);
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

        struct ModuleData
        {
            public CodeInjector<ParameterValueHandlingCodeProviderArgument> CodeInjector;
            public FieldDefinition StateHoldingField;
        }
    }
}
