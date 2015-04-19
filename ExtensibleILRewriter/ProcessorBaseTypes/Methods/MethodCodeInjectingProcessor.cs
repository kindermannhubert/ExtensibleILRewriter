using Mono.Cecil;
using System.Collections.Generic;
using ExtensibleILRewriter.Processors.Parameters;
using ExtensibleILRewriter.CodeInjection;

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

        public override void Process([NotNull]MethodDefinition method, TypeDefinition declaringType)
        {
            ModuleData moduleData;
            if (!modulesData.TryGetValue(method.Module, out moduleData))
            {
                moduleData = new ModuleData();
                var codeProvider = Configuration.CustomValueHandlingCodeProvider;
                moduleData.CodeInjector = new CodeInjector<MethodCodeInjectingCodeProviderArgument>(method.Module, codeProvider);
                moduleData.StateHoldingField = PrepareStateHoldingField(codeProvider, method.Module);

                modulesData.Add(method.Module, moduleData);
            }

            moduleData.CodeInjector.InjectAtBegining(method, new MethodCodeInjectingCodeProviderArgument(method, moduleData.StateHoldingField), logger);
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

        struct ModuleData
        {
            public CodeInjector<MethodCodeInjectingCodeProviderArgument> CodeInjector;
            public FieldDefinition StateHoldingField;
        }
    }
}
