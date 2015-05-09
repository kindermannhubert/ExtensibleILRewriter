using ExtensibleILRewriter.CodeInjection;
using ExtensibleILRewriter.Processors.Parameters;
using System;

namespace ExtensibleILRewriter.Processors
{
    public class AddAttributeProcessorConfiguration : ComponentProcessorConfiguration
    {
        public AddAttributeProcessorConfiguration()
        {
            AddSupportedPropertyNames(nameof(CustomAttributeProvider));
        }

        public AttributeProvider CustomAttributeProvider { get; private set; }

        protected virtual AttributeProvider GetDefaultCodeProvider()
        {
            throw new InvalidOperationException($"General {nameof(AddAttributeProcessorConfiguration)} does not have any default attribute provider. You need to configure {nameof(CustomAttributeProvider)} at processor properties.");
        }

        protected override void LoadFromPropertiesInternal([NotNull]ComponentProcessorProperties properties, TypeAliasResolver typeAliasResolver, string processorName)
        {
            if (properties.ContainsProperty(nameof(CustomAttributeProvider)))
            {
                var customAttributeProviderAlias = properties.GetProperty(nameof(CustomAttributeProvider));
                var customAttributeProviderType = typeAliasResolver.ResolveType(customAttributeProviderAlias);
                CustomAttributeProvider = (AttributeProvider)Activator.CreateInstance(customAttributeProviderType);
            }
            else
            {
                CustomAttributeProvider = GetDefaultCodeProvider();
            }
        }
    }
}
