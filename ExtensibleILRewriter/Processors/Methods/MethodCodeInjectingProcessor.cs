using ExtensibleILRewriter.CodeInjection;
using ExtensibleILRewriter.Logging;
using ExtensibleILRewriter.Processors.Parameters;
using Mono.Cecil;
using System;
using System.Collections.Generic;

namespace ExtensibleILRewriter.Processors.Methods
{
    public abstract class MethodCodeInjectingProcessor<ConfigurationType> : ComponentProcessor<ConfigurationType>
        where ConfigurationType : MethodCodeInjectingProcessorConfiguration
    {
        private readonly Dictionary<ModuleDefinition, ModuleData> modulesData = new Dictionary<ModuleDefinition, ModuleData>();

        public MethodCodeInjectingProcessor([NotNull]ConfigurationType configuration, [NotNull]ILogger logger)
            : base(configuration, logger)
        {
            AddSupportedComponent(ProcessableComponentType.Method);
        }

        protected CodeInjector<MethodCodeInjectingCodeProviderArgument>.CustomInstructionsInjection CustomInstructionsInjection { get; set; }

        public override void Process([NotNull]IProcessableComponent component)
        {
            if (component.Type != ProcessableComponentType.Method)
            {
                throw new InvalidOperationException("Component is expected to be method.");
            }

            var method = (MethodProcessableComponent)component;
            var declaringModule = method.DeclaringModule;

            ModuleData moduleData;
            if (!modulesData.TryGetValue(declaringModule, out moduleData))
            {
                moduleData = new ModuleData();
                var codeProvider = Configuration.CodeProvider;
                var stateInstanceName = GetStateInstanceName(method);
                moduleData.CodeInjector = new CodeInjector<MethodCodeInjectingCodeProviderArgument>(declaringModule, codeProvider);
                moduleData.StateHoldingField = PrepareStateHoldingField(stateInstanceName, codeProvider, declaringModule);

                modulesData.Add(declaringModule, moduleData);
            }

            var codeProviderArgument = new MethodCodeInjectingCodeProviderArgument(method, moduleData.StateHoldingField);
            if (moduleData.CodeInjector.ShouldBeCallInjected(codeProviderArgument))
            {
                var injectionPlace = GetInjectionPlace(method);
                switch (injectionPlace)
                {
                    case MethodInjectionPlace.Begining:
                        moduleData.CodeInjector.InjectAtBegining(method.UnderlyingComponent, codeProviderArgument, Logger);
                        break;
                    case MethodInjectionPlace.Exit:
                        moduleData.CodeInjector.InjectBeforeExit(method.UnderlyingComponent, codeProviderArgument, Logger);
                        break;
                    case MethodInjectionPlace.Custom:
                        if (CustomInstructionsInjection == null)
                        {
                            throw new InvalidOperationException($"If you want to use {nameof(MethodInjectionPlace)}.{nameof(MethodInjectionPlace.Custom)} you need to set value to {nameof(CustomInstructionsInjection)} property.");
                        }

                        moduleData.CodeInjector.Inject(method.UnderlyingComponent, codeProviderArgument, Logger, CustomInstructionsInjection);
                        break;
                    default:
                        throw new InvalidOperationException($"Unknown injection place '{injectionPlace}' specified.");
                }
            }
        }

        protected abstract MethodInjectionPlace GetInjectionPlace(MethodProcessableComponent method);

        protected abstract string GetStateInstanceName(MethodProcessableComponent method);

        private FieldDefinition PrepareStateHoldingField(string stateInstanceName, CodeProvider<MethodCodeInjectingCodeProviderArgument> codeProvider, ModuleDefinition module)
        {
            if (codeProvider.HasState)
            {
                if (string.IsNullOrWhiteSpace(stateInstanceName))
                {
                    throw new InvalidOperationException($"Code provider '{codeProvider.GetType().FullName}' has state but '{nameof(stateInstanceName)}' is not defined in configuration of processor.");
                }

                var stateInstanceId = new StateInstanceId(stateInstanceName);
                return StateInstancesCodeGenerator.PrepareInstanceHoldingField(module, codeProvider.GetStateType(), stateInstanceName, stateInstanceId);
            }
            else
            {
                return null;
            }
        }

        private struct ModuleData
        {
            public CodeInjector<MethodCodeInjectingCodeProviderArgument> CodeInjector;
            public FieldDefinition StateHoldingField;
        }
    }
}
