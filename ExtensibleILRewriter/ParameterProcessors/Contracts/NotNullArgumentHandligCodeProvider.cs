using ExtensibleILRewriter.MethodProcessors.ArgumentHandling;
using ExtensibleILRewriter.MethodProcessors.Helpers;
using Mono.Cecil;
using System;

namespace ExtensibleILRewriter.ParameterProcessors.Contracts
{
    public class NotNullArgumentHandligCodeProvider<ArgumentType> : ArgumentHandlingCodeProvider<ArgumentType>
    {
        public NotNullArgumentHandligCodeProvider(ArgumentHandlingType type, TypeDefinition handlingInstanceType)
            : base(type, handlingInstanceType)
        {
        }

        public override void CheckPrerequisites()
        {
            if (!typeof(ArgumentType).IsClass && Nullable.GetUnderlyingType(typeof(ArgumentType)) == null)
            {
                throw new InvalidOperationException("ArgumentType must be reference type or nullable struct.");
            }
        }

        [MakeStaticVersion(ArgumentHandligCodeInjector<ArgumentType>.StaticHandlingMethodPrefix + nameof(HandleArgument))]
        public override void HandleArgument(ArgumentType argument, string argumentName)
        {
            if (argument == null) throw new ArgumentNullException(argumentName);
        }
    }
}
