using Mono.Cecil;
using Mono.Collections.Generic;
using ExtensibleILRewriter.Extensions;

namespace ExtensibleILRewriter.CodeInjection
{
    public class AttributeInjector<AttributeProviderArgumentType>
    {
        private readonly AttributeProvider<AttributeProviderArgumentType> attributeProvider;

        public AttributeInjector(AttributeProvider<AttributeProviderArgumentType> attributeProvider)
        {
            this.attributeProvider = attributeProvider;
        }

        public void AddAttributeToComponent<ComponentType>(ComponentType component, AttributeProviderArgumentType attributeProviderArgument, ILogger logger)
            where ComponentType : IProcessableComponent
        {
            AddAttributeToComponent(attributeProviderArgument, component.CustomAttributes, component.DeclaringModule, logger, component.FullName);
        }

        private void AddAttributeToComponent(AttributeProviderArgumentType attributeProviderArgument, Collection<CustomAttribute> componentAttributes, ModuleDefinition destinationModule, ILogger logger, string componentName)
        {
            var attributeInfo = attributeProvider.GetAttributeInfo(attributeProviderArgument, destinationModule);

            if (!attributeInfo.ShouldBeAttributeInjected) return;

            logger.Notice("Injecting attribute to \{componentName}.");

            componentAttributes.Add(attributeInfo.CustomAttribute);
        }
    }
}