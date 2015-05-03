using ExtensibleILRewriter.CodeInjection;
using ExtensibleILRewriter.Processors.Parameters;
using System;

namespace ExtensibleILRewriter.ProcessorBaseTypes
{
    public class AddAttributeToComponentConfiguration : ComponentProcessorConfiguration
    {
        public AddAttributeToComponentConfiguration()
        {
            AddSupportedPropertyNames(nameof(CustomAttributeProvider));
        }

        protected virtual AttributeProvider GetDefaultCodeProvider()
        {
            throw new InvalidOperationException($"General {nameof(AddAttributeToComponentConfiguration)} does not have any default attribute provider. You need to configure {nameof(CustomAttributeProvider)} at processor properties.");
        }

        public AttributeProvider CustomAttributeProvider { get; private set; }

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
