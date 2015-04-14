using Mono.Cecil;
using System.Linq;
using ExtensibleILRewriter.Extensions;
using ExtensibleILRewriter.Contracts;

namespace ExtensibleILRewriter.MethodProcessors.Helpers
{
    public class MakeStaticVersionProcessor : MethodProcessor<ComponentProcessorConfiguration.EmptyConfiguration>
    {
        private readonly static string makeStaticVersionAttributeFullName = typeof(MakeStaticVersionAttribute).FullName;

        public MakeStaticVersionProcessor([NotNull]ComponentProcessorConfiguration.EmptyConfiguration configuration, [NotNull]ILogger logger)
            : base(configuration, logger)
        {
        }

        public override void Process([NotNull]MethodDefinition method, TypeDefinition declaringType)
        {
            var attribute = method.CustomAttributes.FirstOrDefault(a => a.AttributeType.FullName == makeStaticVersionAttributeFullName);
            if (attribute == null) return;

            if (!method.HasBody)
            {
                logger.LogErrorWithSource(method, "Cannot make static version of method '\{method.FullName}' which does not have body.");
                return;
            }
            if (method.IsStatic)
            {
                logger.LogErrorWithSource(method, "Method '\{method.FullName}' is already static.");
                return;
            }
            if (!method.CouldBeStatic())
            {
                logger.LogErrorWithSource(method, "Method '\{method.FullName}' must be able to be static in order to make static version of it.");
                return;
            }

            var staticMethod = method.CreateStaticVersion();
            staticMethod.Name = (string)attribute.ConstructorArguments[0].Value;

            staticMethod.DeclaringType = declaringType;
            if (declaringType.Methods.Any(m => m.FullName == staticMethod.FullName))
            {
                staticMethod.DeclaringType = null;
                logger.LogErrorWithSource(method, "Type '\{declaringType.FullName}' already contains method '\{staticMethod.FullName}'.");
            }
            else
            {
                declaringType.Methods.Add(staticMethod);
            }
        }
    }
}
