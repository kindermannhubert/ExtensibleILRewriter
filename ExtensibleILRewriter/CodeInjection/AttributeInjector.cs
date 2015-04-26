using Mono.Cecil;
using Mono.Collections.Generic;
using ExtensibleILRewriter.Extensions;
using System;

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
        {
            var assembly = component as AssemblyDefinition;
            if (assembly != null)
            {
                AddAttributeToAssembly(assembly, attributeProviderArgument, logger);
                return;
            }

            var module = component as ModuleDefinition;
            if (module != null)
            {
                AddAttributeToModule(module, attributeProviderArgument, logger);
                return;
            }

            var type = component as TypeDefinition;
            if (type != null)
            {
                AddAttributeToType(type, attributeProviderArgument, logger);
                return;
            }

            var method = component as MethodDefinition;
            if (method != null)
            {
                AddAttributeToMethod(method, attributeProviderArgument, logger);
                return;
            }

            throw new NotSupportedException("Not supported ComponentType '\{typeof(ComponentType).FullName}' used.");
        }

        public void AddAttributeToAssembly(AssemblyDefinition assembly, AttributeProviderArgumentType attributeProviderArgument, ILogger logger)
        {
            AddAttributeToComponent(attributeProviderArgument, assembly.CustomAttributes, assembly.MainModule, logger, "assembly '\{assembly.FullName}'");
        }

        public void AddAttributeToModule(ModuleDefinition module, AttributeProviderArgumentType attributeProviderArgument, ILogger logger)
        {
            AddAttributeToComponent(attributeProviderArgument, module.CustomAttributes, module, logger, "module '\{module.Name}'");
        }

        public void AddAttributeToType(TypeDefinition type, AttributeProviderArgumentType attributeProviderArgument, ILogger logger)
        {
            AddAttributeToComponent(attributeProviderArgument, type.CustomAttributes, type.Module, logger, "type'\{type.FullName}'");
        }

        public void AddAttributeToMethod(MethodDefinition method, AttributeProviderArgumentType attributeProviderArgument, ILogger logger)
        {
            AddAttributeToComponent(attributeProviderArgument, method.CustomAttributes, method.Module, logger, "method '\{method.FullName}'");
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