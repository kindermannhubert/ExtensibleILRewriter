using ExtensibleILRewriter.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ExtensibleILRewriter
{
    public abstract class ComponentProcessorConfiguration
    {
        public abstract IEnumerable<string> SupportedPropertyNames { get; }

        public void LoadFromProperties([NotNull]ComponentProcessorProperties properties, TypeAliasResolver typeAliasResolver, string processorName)
        {
            CheckIfOnlySupportedPropertiesWereSpecified(properties, processorName);
            LoadFromPropertiesInternal(properties, typeAliasResolver, processorName);
        }

        protected abstract void LoadFromPropertiesInternal([NotNull]ComponentProcessorProperties properties, TypeAliasResolver typeAliasResolver, string processorName);

        private void CheckIfOnlySupportedPropertiesWereSpecified([NotNull]ComponentProcessorProperties properties, string processorName)
        {
            var supportedPropertyName = new HashSet<string>(SupportedPropertyNames);
            foreach (var property in properties)
            {
                if (!supportedPropertyName.Contains(property.Key)) throw new InvalidOperationException("Not supported property '\{property.Key}' of '\{GetType().FullName}' configuration was specified for processor '\{processorName}'.");
            }
        }

        protected void CheckIfContainsProperty([NotNull]ComponentProcessorProperties properties, string property)
        {
            if (!properties.ContainsProperty(property))
            {
                throw new InvalidOperationException("\{GetType().FullName} processor configuration needs '\{property}' element in configuration specified.");
            }
        }

        public class EmptyConfiguration : ComponentProcessorConfiguration
        {
            public override IEnumerable<string> SupportedPropertyNames { get { return Enumerable.Empty<string>(); } }

            protected override void LoadFromPropertiesInternal(ComponentProcessorProperties properties, TypeAliasResolver typeAliasResolver, string processorName)
            {
            }
        }
    }
}
