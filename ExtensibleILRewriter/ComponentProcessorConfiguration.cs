using ExtensibleILRewriter.Processors.Parameters;
using System;
using System.Collections.Generic;

namespace ExtensibleILRewriter
{
    public abstract class ComponentProcessorConfiguration
    {
        private readonly List<string> supportedPropertyNames = new List<string>();

        public IEnumerable<string> SupportedPropertyNames { get { return supportedPropertyNames; } }

        public void LoadFromProperties([NotNull]ComponentProcessorProperties properties, TypeAliasResolver typeAliasResolver, string processorName)
        {
            CheckIfOnlySupportedPropertiesWereSpecified(properties, processorName);
            LoadFromPropertiesInternal(properties, typeAliasResolver, processorName);
        }

        protected abstract void LoadFromPropertiesInternal([NotNull]ComponentProcessorProperties properties, TypeAliasResolver typeAliasResolver, string processorName);

        protected void CheckIfContainsProperty([NotNull]ComponentProcessorProperties properties, string property)
        {
            if (!properties.ContainsProperty(property))
            {
                throw new InvalidOperationException($"{GetType().FullName} processor configuration needs '{property}' element in configuration specified.");
            }
        }

        protected void AddSupportedPropertyNames(params string[] names)
        {
            supportedPropertyNames.AddRange(names);
        }

        private void CheckIfOnlySupportedPropertiesWereSpecified([NotNull]ComponentProcessorProperties properties, string processorName)
        {
            var supportedPropertyName = new HashSet<string>(SupportedPropertyNames);
            foreach (var property in properties)
            {
                if (!supportedPropertyName.Contains(property.Key))
                {
                    throw new InvalidOperationException($"Not supported property '{property.Key}' of '{GetType().FullName}' configuration was specified for processor '{processorName}'.");
                }
            }
        }

        public class EmptyConfiguration : ComponentProcessorConfiguration
        {
            protected override void LoadFromPropertiesInternal(ComponentProcessorProperties properties, TypeAliasResolver typeAliasResolver, string processorName)
            {
            }
        }
    }
}
