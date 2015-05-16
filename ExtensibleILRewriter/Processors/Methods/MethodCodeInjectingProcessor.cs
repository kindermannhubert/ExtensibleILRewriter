using Mono.Cecil;
using System.Collections.Generic;
using ExtensibleILRewriter.Processors.Parameters;
using ExtensibleILRewriter.CodeInjection;
using ExtensibleILRewriter.Logging;
using System;

namespace ExtensibleILRewriter.Processors.Methods
{
    public class MethodCodeInjectingProcessor<ConfigurationType> : ComponentProcessor<ConfigurationType>
        where ConfigurationType : MethodCodeInjectingProcessorConfiguration
    {
        private readonly Dictionary<ModuleDefinition, ModuleData> modulesData = new Dictionary<ModuleDefinition, ModuleData>();

        public MethodCodeInjectingProcessor([NotNull]ConfigurationType configuration, [NotNull]ILogger logger)
            : base(configuration, logger)
        {
            AddSupportedComponent(ProcessableComponentType.Method);
        }

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
                var codeProvider = Configuration.CustomValueHandlingCodeProvider;
                moduleData.CodeInjector = new CodeInjector<MethodCodeInjectingCodeProviderArgument>(declaringModule, codeProvider);
                moduleData.StateHoldingField = PrepareStateHoldingField(codeProvider, declaringModule);

                modulesData.Add(declaringModule, moduleData);
            }

            switch (Configuration.InjectionPlace)
            {
                case MethodInjectionPlace.Begining:
                    moduleData.CodeInjector.InjectAtBegining(method.UnderlyingComponent, new MethodCodeInjectingCodeProviderArgument(method, moduleData.StateHoldingField), Logger);
                    break;
                case MethodInjectionPlace.Exit:
                    moduleData.CodeInjector.InjectBeforeExit(method.UnderlyingComponent, new MethodCodeInjectingCodeProviderArgument(method, moduleData.StateHoldingField), Logger);
                    break;
                default:
                    throw new InvalidOperationException($"Unknown injection place '{Configuration.InjectionPlace}' specified.");
            }
        }

        private FieldDefinition PrepareStateHoldingField(CodeProvider<MethodCodeInjectingCodeProviderArgument> codeProvider, ModuleDefinition module)
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

        private struct ModuleData
        {
            public CodeInjector<MethodCodeInjectingCodeProviderArgument> CodeInjector;
            public FieldDefinition StateHoldingField;
        }
    }
}
