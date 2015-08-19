using ExtensibleILRewriter.CodeInjection;
using ExtensibleILRewriter.Logging;
using Mono.Cecil;
using System;
using System.Collections.Generic;

namespace ExtensibleILRewriter.Processors.Parameters
{
    public class ParameterValueHandlingProcessor<ConfigurationType> : ComponentProcessor<ConfigurationType>
        where ConfigurationType : ParameterValueHandlingProcessorConfiguration
    {
        private readonly Dictionary<ModuleDefinition, ModuleData> modulesData = new Dictionary<ModuleDefinition, ModuleData>();

        public ParameterValueHandlingProcessor([NotNull]ConfigurationType configuration, [NotNull]ILogger logger)
            : base(configuration, logger)
        {
            AddSupportedComponent(ProcessableComponentType.MethodParameter);
        }

        public override void Process([NotNull]IProcessableComponent component)
        {
            if (component.Type != ProcessableComponentType.MethodParameter)
            {
                throw new InvalidOperationException("Component is expected to be method parameter.");
            }

            var parameter = (MethodParameterProcessableComponent)component;

            ModuleData moduleData;
            if (!modulesData.TryGetValue(parameter.DeclaringModule, out moduleData))
            {
                moduleData = new ModuleData();
                var codeProvider = Configuration.CustomValueHandlingCodeProvider;
                moduleData.CodeInjector = new CodeInjector<ParameterValueHandlingCodeProviderArgument>(parameter.DeclaringModule, codeProvider);
                moduleData.StateHoldingField = PrepareStateHoldingField(codeProvider, parameter.DeclaringModule);

                modulesData.Add(parameter.DeclaringModule, moduleData);
            }

            var codeProviderArgument = new ParameterValueHandlingCodeProviderArgument(parameter, moduleData.StateHoldingField);
            if (moduleData.CodeInjector.ShouldBeCallInjected(codeProviderArgument))
            {
                moduleData.CodeInjector.InjectAtBegining(parameter.DeclaringComponent.UnderlyingComponent, codeProviderArgument, Logger);
            }
        }

        private FieldDefinition PrepareStateHoldingField(CodeProvider<ParameterValueHandlingCodeProviderArgument> codeProvider, ModuleDefinition module)
        {
            if (codeProvider.HasState)
            {
                var stateInstanceId = new StateInstanceId(Configuration.StateInstanceName);
                return StateInstancesCodeGenerator.PrepareInstanceHoldingField(module, codeProvider.GetStateType(), Configuration.StateInstanceName, stateInstanceId);
            }
            else
            {
                return null;
            }
        }

        private struct ModuleData
        {
            public CodeInjector<ParameterValueHandlingCodeProviderArgument> CodeInjector;
            public FieldDefinition StateHoldingField;
        }
    }
}
