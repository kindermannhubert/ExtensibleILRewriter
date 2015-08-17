using ExtensibleILRewriter.CodeInjection;
using System;

namespace ExtensibleILRewriter.Processors.Methods
{
    public class MethodCodeInjectingProcessorConfiguration : ComponentProcessorConfiguration
    {
        public MethodCodeInjectingProcessorConfiguration()
        {
            AddSupportedPropertyNames(nameof(CodeProvider));
        }

        public CodeProvider<MethodCodeInjectingCodeProviderArgument> CodeProvider { get; private set; }

        protected virtual CodeProvider<MethodCodeInjectingCodeProviderArgument> GetDefaultCodeProvider()
        {
            throw new InvalidOperationException($"General {typeof(MethodCodeInjectingProcessor<>).FullName} does not have any default code provider. You need to configure {nameof(CodeProvider)} at processor properties.");
        }

        protected override void LoadFromPropertiesInternal(ComponentProcessorProperties properties, TypeAliasResolver typeAliasResolver, string procesorName)
        {
            if (properties.ContainsProperty(nameof(CodeProvider)))
            {
                var odeProviderAlias = properties.GetProperty(nameof(CodeProvider));
                var codeProviderType = typeAliasResolver.ResolveType(odeProviderAlias);
                CodeProvider = (CodeProvider<MethodCodeInjectingCodeProviderArgument>)Activator.CreateInstance(codeProviderType);
            }
            else
            {
                CodeProvider = GetDefaultCodeProvider();
            }
        }
    }
}
