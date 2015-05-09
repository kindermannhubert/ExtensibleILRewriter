using ExtensibleILRewriter.CodeInjection;
using Mono.Cecil;
using System;

namespace ExtensibleILRewriter.Tests.AddAttributeProcessor
{
    public class InjectedAttributeProvider : AttributeProvider
    {
        protected override AttributeProviderAttributeArgument[] GetAttributeArguments(IProcessableComponent component)
        {
            return Xxx(component);
        }

        private static AttributeProviderAttributeArgument[] Xxx(IProcessableComponent component)
        {
            var typeDefinition = component.Type == ProcessableComponentType.Type ? ((TypeDefinition)component.UnderlyingComponent) : null;

            return new AttributeProviderAttributeArgument[]
            {
                AttributeProviderAttributeArgument.CreateParameterArgument("component", component.Type),
                AttributeProviderAttributeArgument.CreateParameterArgument("type", typeDefinition),
                AttributeProviderAttributeArgument.CreateParameterArgument("nameHash", component.Name.GetHashCode()),
                AttributeProviderAttributeArgument.CreateParameterArgument("name", component.Name)
            };
        }

        protected override Type GetAttributeType(IProcessableComponent component)
        {
            return typeof(InjectedAttribute);
        }

        protected override bool ShouldBeInjected(IProcessableComponent component)
        {
            return component.Name.StartsWith("Decorate_");
        }
    }
}
