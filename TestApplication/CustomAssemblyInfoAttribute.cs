using Mono.Cecil;
using System;
using ExtensibleILRewriter.CodeInjection;
using ExtensibleILRewriter;

namespace TestApplication
{
    public class CustomAssemblyInfoAttributeProvider : AttributeProvider<AssemblyProcessableComponent>
    {
        protected override AttributeProviderAttributeArgument[] GetAttributeArguments(AssemblyProcessableComponent assembly)
        {
            return new AttributeProviderAttributeArgument[]
            {
                AttributeProviderAttributeArgument.CreateParameterArgument("text", "hello")
            };
        }

        protected override Type GetAttributeType(AssemblyProcessableComponent assembly)
        {
            return typeof(CustomAssemblyInfoAttribute);
        }

        protected override bool ShouldBeInjected(AssemblyProcessableComponent assembly)
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
