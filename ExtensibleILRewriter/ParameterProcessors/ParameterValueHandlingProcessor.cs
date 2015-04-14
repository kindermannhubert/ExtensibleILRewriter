using System;
using ExtensibleILRewriter.ParameterProcessors.Contracts;
using Mono.Cecil;
using ExtensibleILRewriter.Extensions;
using System.Collections.Generic;

namespace ExtensibleILRewriter.ParameterProcessors
{
    public class ParameterValueHandlingProcessor<ConfigurationType> : ParameterValueHandlingProcessorBase<ConfigurationType>
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
                info.CodeInjector = new ParameterValueHandlingCodeInjector(declaringMethod.Module, Configuration.CustomValueHandlingCodeProvider);
                info.StateHoldingField = PrepareStateHoldingField(declaringMethod.Module);

                injectionInfos.Add(declaringMethod.Module, info);
            }

            info.CodeInjector.Inject(declaringMethod, parameter, info.StateHoldingField, logger);
        }

        private FieldDefinition PrepareStateHoldingField(ModuleDefinition module)
        {
            var stateType = Configuration.CustomValueHandlingCodeProvider.GetStateType();
            return HandlingInstancesCodeGenerator.PrepareInstanceHoldingField(module, stateType, Configuration.StateInstanceName, Configuration.StateInstanceName);
        }

        class InjectionInfo
        {
            public ParameterValueHandlingCodeInjector CodeInjector;
            public FieldDefinition StateHoldingField;
        }
    }
}
