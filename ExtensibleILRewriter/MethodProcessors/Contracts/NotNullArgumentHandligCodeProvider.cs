using ExtensibleILRewriter.MethodProcessors.ArgumentHandling;
using ExtensibleILRewriter.MethodProcessors.Helpers;
using System;

namespace ExtensibleILRewriter.MethodProcessors.Contracts
{
    public class NotNullArgumentHandligCodeProvider<ArgumentType> : ArgumentHandlingCodeProvider<ArgumentType>
    {
        public NotNullArgumentHandligCodeProvider(ArgumentHandlingType type, string handlingInstanceName)
            : base(type, handlingInstanceName)
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
