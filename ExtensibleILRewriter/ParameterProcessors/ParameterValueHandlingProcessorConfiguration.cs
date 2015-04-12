using System;
using System.Collections.Generic;

namespace ExtensibleILRewriter.ParameterProcessors
{
    public class ParameterValueHandlingProcessorConfiguration : ComponentProcessorConfiguration
    {
        protected virtual IParameterValueHandlingCodeProvider GetDefaultCodeProvider()
        {
            throw new InvalidOperationException("General \{nameof(ParameterValueHandlingProcessor)} does not have any default code provider. You need to configure \{nameof(CustomValueHandlingCodeProvider)} at processor properties.");
        }

        public IParameterValueHandlingCodeProvider CustomValueHandlingCodeProvider { get; private set; }

        public string StateInstanceName { get; private set; }

        public override IEnumerable<string> SupportedPropertyNames { get; } = new string[] { nameof(CustomValueHandlingCodeProvider), nameof(StateInstanceName) };

        protected override void LoadFromPropertiesInternal(ComponentProcessorProperties properties, TypeAliasResolver typeAliasResolver, string procesorName)
        {
            if (properties.ContainsProperty(nameof(CustomValueHandlingCodeProvider)))
            {
                var customValueHandlingCodeProviderAlias = properties.GetProperty(nameof(CustomValueHandlingCodeProvider));
                var customValueHandlingCodeProviderType = typeAliasResolver.ResolveType(customValueHandlingCodeProviderAlias);
                CustomValueHandlingCodeProvider = (IParameterValueHandlingCodeProvider)Activator.CreateInstance(customValueHandlingCodeProviderType);

                base.CheckIfContainsProperty(properties, nameof(StateInstanceName));
                StateInstanceName = properties.GetProperty(nameof(StateInstanceName));
            }
            else
            {
                CustomValueHandlingCodeProvider = GetDefaultCodeProvider();
            }
        }
    }
}
