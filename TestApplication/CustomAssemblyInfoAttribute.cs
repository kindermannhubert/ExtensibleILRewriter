using Mono.Cecil;
using System;
using ExtensibleILRewriter.CodeInjection;

namespace TestApplication
{
    public class CustomAssemblyInfoAttributeProvider : AttributeProvider<AssemblyDefinition>
    {
        protected override AttributeProviderAttributeArgument[] GetAttributeArguments(AssemblyDefinition attributeProviderArgument)
        {
            return new AttributeProviderAttributeArgument[]
            {
                AttributeProviderAttributeArgument.CreateParameterArgument("text", "hello")
            };
        }

        protected override Type GetAttributeType(AssemblyDefinition attributeProviderArgument)
        {
            return typeof(CustomAssemblyInfoAttribute);
        }

        protected override bool ShouldBeInjected(AssemblyDefinition attributeProviderArgument)
        {
            return true;
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
