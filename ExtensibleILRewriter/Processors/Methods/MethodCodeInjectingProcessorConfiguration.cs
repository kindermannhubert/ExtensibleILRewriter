using ExtensibleILRewriter.CodeInjection;
using System;

namespace ExtensibleILRewriter.Processors.Methods
{
    public class MethodCodeInjectingProcessorConfiguration : ComponentProcessorConfiguration
    {
        private string stateInstanceName;
        private MethodInjectionPlace injectionPlace = MethodInjectionPlace.Begining;

        public MethodCodeInjectingProcessorConfiguration()
        {
            AddSupportedPropertyNames(
                nameof(CustomValueHandlingCodeProvider),
                nameof(StateInstanceName),
                nameof(InjectionPlace));
        }

        public CodeProvider<MethodCodeInjectingCodeProviderArgument> CustomValueHandlingCodeProvider { get; private set; }

        public string StateInstanceName { get { return stateInstanceName; } }

        public MethodInjectionPlace InjectionPlace { get { return injectionPlace; } }

        protected virtual CodeProvider<MethodCodeInjectingCodeProviderArgument> GetDefaultCodeProvider()
        {
            throw new InvalidOperationException($"General {typeof(MethodCodeInjectingProcessor<>).FullName} does not have any default code provider. You need to configure {nameof(CustomValueHandlingCodeProvider)} at processor properties.");
        }

        protected override void LoadFromPropertiesInternal(ComponentProcessorProperties properties, TypeAliasResolver typeAliasResolver, string procesorName)
        {
            if (properties.ContainsProperty(nameof(CustomValueHandlingCodeProvider)))
            {
                var customValueHandlingCodeProviderAlias = properties.GetProperty(nameof(CustomValueHandlingCodeProvider));
                var customValueHandlingCodeProviderType = typeAliasResolver.ResolveType(customValueHandlingCodeProviderAlias);
                CustomValueHandlingCodeProvider = (CodeProvider<MethodCodeInjectingCodeProviderArgument>)Activator.CreateInstance(customValueHandlingCodeProviderType);
            }
            else
            {
                CustomValueHandlingCodeProvider = GetDefaultCodeProvider();
            }

            properties.TryGetProperty(nameof(StateInstanceName), out stateInstanceName);

            string injectionPlaceText;
            if (properties.TryGetProperty(nameof(InjectionPlace), out injectionPlaceText))
            {
                if (!Enum.TryParse(injectionPlaceText, out injectionPlace))
                {
                    throw new InvalidOperationException($"Unable to parse value '{injectionPlaceText}' from configuration to {typeof(MethodInjectionPlace).FullName} enum.");
                }
            }
        }
    }
}
