using System.Linq;
using ExtensibleILRewriter.Extensions;
using ExtensibleILRewriter.Processors.Parameters;
using ExtensibleILRewriter.Logging;
using System;

namespace ExtensibleILRewriter.ProcessorBaseTypes.Methods.Helpers
{
    public class MakeStaticVersionProcessor : ComponentProcessor<ComponentProcessorConfiguration.EmptyConfiguration>
    {
        private readonly static string MakeStaticVersionAttributeFullName = typeof(MakeStaticVersionAttribute).FullName;

        public MakeStaticVersionProcessor([NotNull]ComponentProcessorConfiguration.EmptyConfiguration configuration, [NotNull]ILogger logger)
            : base(configuration, logger)
        {
            AddSupportedComponent(ProcessableComponentType.Method);
        }

        public override void Process([NotNull]IProcessableComponent component)
        {
            if (component.Type != ProcessableComponentType.Method)
            {
                throw new InvalidOperationException("Component is expected to be method.");
            }

            var method = (MethodProcessableComponent)component;

            var attribute = method.CustomAttributes.FirstOrDefault(a => a.AttributeType.FullName == MakeStaticVersionAttributeFullName);
            if (attribute == null)
            {
                return;
            }

            var methodDefinition = method.UnderlyingComponent;
            if (!methodDefinition.HasBody)
            {
                Logger.LogErrorWithSource(methodDefinition, $"Cannot make static version of method '{method.FullName}' which does not have body.");
                return;
            }

            if (methodDefinition.IsStatic)
            {
                Logger.LogErrorWithSource(methodDefinition, $"Method '{method.FullName}' is already static.");
                return;
            }

            if (!methodDefinition.CouldBeStatic())
            {
                Logger.LogErrorWithSource(methodDefinition, $"Method '{method.FullName}' must be able to be static in order to make static version of it.");
                return;
            }

            var staticMethod = methodDefinition.CreateStaticVersion();
            staticMethod.Name = (string)attribute.ConstructorArguments[0].Value;

            staticMethod.DeclaringType = method.DeclaringComponent.UnderlyingComponent;
            if (staticMethod.DeclaringType.Methods.Any(m => m.FullName == staticMethod.FullName))
            {
                staticMethod.DeclaringType = null;
                Logger.LogErrorWithSource(methodDefinition, $"Type '{method.DeclaringComponent.FullName}' already contains method '{staticMethod.FullName}'.");
            }
            else
            {
                staticMethod.DeclaringType.Methods.Add(staticMethod);
            }
        }
    }
}
