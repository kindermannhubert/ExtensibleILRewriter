using Mono.Cecil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ILTools.MethodProcessors.Contracts;
using ILTools.Extensions;

namespace ILTools.MethodProcessors.Helpers
{
    public class MakeStaticVersionProcessor : IComponentProcessor<MethodDefinition>
    {
        private readonly static string makeStaticVersionAttributeFullName = typeof(MakeStaticVersionAttribute).FullName;

        public void Process([NotNull]MethodDefinition method, [NotNull]ILogger logger)
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

            staticMethod.DeclaringType = method.DeclaringType;
            if (method.DeclaringType.Methods.Any(m => m.FullName == staticMethod.FullName))
            {
                staticMethod.DeclaringType = null;
                logger.LogErrorWithSource(method, "Type '\{method.DeclaringType.FullName}' already contains method '\{staticMethod.FullName}'.");
                return;
            }
            method.DeclaringType.Methods.Add(staticMethod);
        }
    }
}
