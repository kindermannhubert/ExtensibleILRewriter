using Mono.Cecil;
using System.Linq;
using ExtensibleILRewriter.Extensions;
using ExtensibleILRewriter.Processors.Parameters;

namespace ExtensibleILRewriter.ProcessorBaseTypes.Methods.Helpers
{
    public class MakeStaticVersionProcessor : MethodProcessor<ComponentProcessorConfiguration.EmptyConfiguration>
    {
        private readonly static string makeStaticVersionAttributeFullName = typeof(MakeStaticVersionAttribute).FullName;

        public MakeStaticVersionProcessor([NotNull]ComponentProcessorConfiguration.EmptyConfiguration configuration, [NotNull]ILogger logger)
            : base(configuration, logger)
        {
        }

        public override void Process([NotNull]MethodProcessableComponent method)
        {
            var attribute = method.CustomAttributes.FirstOrDefault(a => a.AttributeType.FullName == makeStaticVersionAttributeFullName);
            if (attribute == null) return;

            var methodDefinition = method.UnderlyingComponent;
            if (!methodDefinition.HasBody)
            {
                logger.LogErrorWithSource(methodDefinition, $"Cannot make static version of method '{method.FullName}' which does not have body.");
                return;
            }
            if (methodDefinition.IsStatic)
            {
                logger.LogErrorWithSource(methodDefinition, $"Method '{method.FullName}' is already static.");
                return;
            }
            if (!methodDefinition.CouldBeStatic())
            {
                logger.LogErrorWithSource(methodDefinition, $"Method '{method.FullName}' must be able to be static in order to make static version of it.");
                return;
            }

            var staticMethod = methodDefinition.CreateStaticVersion();
            staticMethod.Name = (string)attribute.ConstructorArguments[0].Value;

            staticMethod.DeclaringType = method.DeclaringComponent.UnderlyingComponent;
            if (staticMethod.DeclaringType.Methods.Any(m => m.FullName == staticMethod.FullName))
            {
                staticMethod.DeclaringType = null;
                logger.LogErrorWithSource(methodDefinition, $"Type '{method.DeclaringComponent.FullName}' already contains method '{staticMethod.FullName}'.");
            }
            else
            {
                staticMethod.DeclaringType.Methods.Add(staticMethod);
            }
        }
    }
}
