using ExtensibleILRewriter.Extensions;
using Mono.Cecil;
using System;
using System.Linq;
using System.Reflection;

namespace ExtensibleILRewriter.CodeInjection
{
    public abstract class AttributeProvider<AttributeProviderArgumentType>
    {
        protected abstract bool ShouldBeInjected(AttributeProviderArgumentType attributeProviderArgument);
        protected abstract Type GetAttributeType(AttributeProviderArgumentType attributeProviderArgument);
        protected abstract AttributeProviderAttributeArgument[] GetAttributeArguments(AttributeProviderArgumentType attributeProviderArgument);

        protected CustomAttribute GetAndCheckCustomAttribute(Type attributeClrType, AttributeProviderAttributeArgument[] attributeArguments, ModuleDefinition destinationModule)
        {
            if (!attributeClrType.IsDerivedFrom(typeof(Attribute)))
                throw new InvalidOperationException("Type '\{attributeClrType.FullName}' is not derived from '\{typeof(Attribute).FullName}' class.");

            var attributeCtors = attributeClrType.GetConstructors(BindingFlags.Instance | BindingFlags.Public).ToArray();
            if (attributeCtors.Length != 1) throw new NotSupportedException("Injection of attributes with just one ctor is currently supported. Attribute '\{attributeClrType.FullName}'.");

            var attributeClrCtor = attributeCtors[0];
            var attributeCtor = destinationModule.Import(attributeClrCtor);

            var attributeParams = attributeClrCtor.GetParameters(); //using CLR params because Cecil params does not contain name parameter

            if (attributeParams.Length != attributeArguments.Length)
                throw new InvalidOperationException("Ctor of attribute '\{attributeClrType.FullName}' contains \{attributeParams.Length} parameters but attribute provider returns only \{attributeArguments.Length} parameters.");

            for (int i = 0; i < attributeParams.Length; i++)
            {
                if (attributeParams[i].Name != attributeArguments[i].Name)
                    throw new InvalidOperationException("Parameter \{i} of ctor of attribute '\{attributeClrType.FullName}' is named '\{attributeParams[i].Name}' but attribute provider returns parameter with name '\{attributeArguments[i].Name}'.");

                //TODO
                //if (attributeParams[i].ParameterType.FullName != attributeArguments[i].ClrType.FullName)
                //{
                //    if (attributeArguments[i].ClrType != null || !attributeParams[i].ParameterType.IsGenericParameter)
                //        throw new InvalidOperationException("Parameter '\{attributeArguments[i].Name}' of of attribute '\{attributeName}' in assembly '\{attributeAssembly.FullName}' should generic.");
                //}
            }

            var customAttribute = new CustomAttribute(attributeCtor);

            foreach (var arg in attributeArguments)
            {
                var customArgument = arg.GenerateCustomAttributeArgument(destinationModule);
                customAttribute.ConstructorArguments.Add(customArgument);
            }

            return customAttribute;
        }

        public AttributeProviderInjectionInfo GetAttributeInfo(AttributeProviderArgumentType attributeProviderArgument, ModuleDefinition destinationModule)
        {
            if (ShouldBeInjected(attributeProviderArgument))
            {
                var attributeArguments = GetAttributeArguments(attributeProviderArgument);
                if (attributeArguments == null) attributeArguments = new AttributeProviderAttributeArgument[0];

                var attributeType = GetAttributeType(attributeProviderArgument);
                var attribute = GetAndCheckCustomAttribute(attributeType, attributeArguments, destinationModule);

                return new AttributeProviderInjectionInfo(true, attribute);
            }
            else
            {
                return new AttributeProviderInjectionInfo(false, null);
            }
        }
    }
}
