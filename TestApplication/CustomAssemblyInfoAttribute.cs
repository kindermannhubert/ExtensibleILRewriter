using Mono.Cecil;
using System;
using ExtensibleILRewriter.CodeInjection;
using ExtensibleILRewriter;

namespace TestApplication
{
    public class CustomAssemblyInfoAttributeProvider : AttributeProvider
    {
        protected override AttributeProviderAttributeArgument[] GetAttributeArguments(IProcessableComponent component)
        {
            return new AttributeProviderAttributeArgument[]
            {
                AttributeProviderAttributeArgument.CreateParameterArgument("text", "hello from '\{component.Name}'!")
            };
        }

        protected override Type GetAttributeType(IProcessableComponent component)
        {
            return typeof(CustomAssemblyInfoAttribute);
        }

        protected override bool ShouldBeInjected(IProcessableComponent component)
        {
            return component.Type == ProcessableComponentType.Assembly;
        }
    }

    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = false, Inherited = false)]
    public class CustomAssemblyInfoAttribute : Attribute
    {
        public CustomAssemblyInfoAttribute(string text)
        {
        }
    }
}
