using ExtensibleILRewriter.CodeInjection;
using System;

namespace ExtensibleILRewriter.Processors.Parameters
{
    public class ParameterValueHandlingProcessorConfiguration : ComponentProcessorConfiguration
    {
        private string stateInstanceName;

        public ParameterValueHandlingProcessorConfiguration()
        {
            AddSupportedPropertyNames(nameof(CustomValueHandlingCodeProvider), nameof(StateInstanceName));
        }

        public CodeProvider<ParameterValueHandlingCodeProviderArgument> CustomValueHandlingCodeProvider { get; private set; }

        public string StateInstanceName { get { return stateInstanceName; } }

        protected virtual CodeProvider<ParameterValueHandlingCodeProviderArgument> GetDefaultCodeProvider()
        {
            throw new InvalidOperationException($"General {typeof(ParameterValueHandlingProcessor<>).FullName} does not have any default code provider. You need to configure {nameof(CustomValueHandlingCodeProvider)} at processor properties.");
        }

        protected override void LoadFromPropertiesInternal(ComponentProcessorProperties properties, TypeAliasResolver typeAliasResolver, string procesorName)
        {
            if (properties.ContainsProperty(nameof(CustomValueHandlingCodeProvider)))
            {
                var customValueHandlingCodeProviderAlias = properties.GetProperty(nameof(CustomValueHandlingCodeProvider));
                var customValueHandlingCodeProviderType = typeAliasResolver.ResolveType(customValueHandlingCodeProviderAlias);
                CustomValueHandlingCodeProvider = (CodeProvider<ParameterValueHandlingCodeProviderArgument>)Activator.CreateInstance(customValueHandlingCodeProviderType);
            }
            else
            {
                CustomValueHandlingCodeProvider = GetDefaultCodeProvider();
            }

            properties.TryGetProperty(nameof(StateInstanceName), out stateInstanceName);
        }
    }
}
