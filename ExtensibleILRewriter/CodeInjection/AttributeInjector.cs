using Mono.Cecil;
using Mono.Collections.Generic;
using ExtensibleILRewriter.Extensions;

namespace ExtensibleILRewriter.CodeInjection
{
    public class AttributeInjector
    {
        private readonly AttributeProvider attributeProvider;

        public AttributeInjector(AttributeProvider attributeProvider)
        {
            this.attributeProvider = attributeProvider;
        }

        public void AddAttributeToComponent(IProcessableComponent component, ILogger logger)
        {
            var attributeInfo = attributeProvider.GetAttributeInfo(component);

            if (!attributeInfo.ShouldBeAttributeInjected) return;

            logger.Notice("Injecting attribute to \{component.FullName}.");

            component.CustomAttributes.Add(attributeInfo.CustomAttribute);
        }
    }
}