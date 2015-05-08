using Mono.Cecil;
using System.Collections.Generic;
using ExtensibleILRewriter.Processors.Parameters;
using ExtensibleILRewriter.CodeInjection;
using ExtensibleILRewriter.Logging;

namespace ExtensibleILRewriter.ProcessorBaseTypes.Methods
{
    public class MethodCodeInjectingProcessor<ConfigurationType> : MethodProcessor<ConfigurationType>
        where ConfigurationType : MethodCodeInjectingProcessorConfiguration
    {
        private readonly Dictionary<ModuleDefinition, ModuleData> modulesData = new Dictionary<ModuleDefinition, ModuleData>();

        public MethodCodeInjectingProcessor([NotNull]ConfigurationType configuration, [NotNull]ILogger logger)
            : base(configuration, logger)
        {
        }

        public override void Process([NotNull]MethodProcessableComponent method)
        {
            ModuleData moduleData;
            if (!modulesData.TryGetValue(method.DeclaringModule, out moduleData))
            {
                moduleData = new ModuleData();
                var codeProvider = Configuration.CustomValueHandlingCodeProvider;
                moduleData.CodeInjector = new CodeInjector<MethodCodeInjectingCodeProviderArgument>(method.DeclaringModule, codeProvider);
                moduleData.StateHoldingField = PrepareStateHoldingField(codeProvider, method.DeclaringModule);

                modulesData.Add(method.DeclaringModule, moduleData);
            }

            moduleData.CodeInjector.InjectAtBegining(method.UnderlyingComponent, new MethodCodeInjectingCodeProviderArgument(method.UnderlyingComponent, moduleData.StateHoldingField), Logger);
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
