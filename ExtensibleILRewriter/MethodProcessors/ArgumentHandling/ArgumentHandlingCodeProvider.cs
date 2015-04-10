using Mono.Cecil;
using System;

namespace ExtensibleILRewriter.MethodProcessors.ArgumentHandling
{
    public abstract class ArgumentHandlingCodeProvider<ArgumentType>
    {
        //private ArgumentHandlingCodeProvider<ArgumentType> handlingInstance;

        public ArgumentHandlingCodeProvider(ArgumentHandlingType type, TypeDefinition handlingInstanceType)
        {
            HandlingType = type;
            //HandlingInstanceName = handlingInstanceTypeAlias;

            switch (type)
            {
                case ArgumentHandlingType.CallStaticHandling:
                    if (handlingInstanceType != null) throw new InvalidOperationException("You cannot specify '\{nameof(handlingInstanceType)}' when you are using '\{type}'.");
                    break;
                case ArgumentHandlingType.CallInstanceHandling:
                    if (handlingInstanceType == null) throw new InvalidOperationException("You must specify '\{nameof(handlingInstanceType)}' when you are using '\{type}'.");
                    break;
                default:
                    throw new NotImplementedException("Unknown argument handling type: '\{type}'.");
            }

            //HandlingInstancesManager.RegisterInstance
        }

        public ArgumentHandlingType HandlingType { get; }

        //public string HandlingInstanceName { get; }

        public abstract void CheckPrerequisites();

        public abstract void HandleArgument(ArgumentType argument, string argumentName);

        //public void SetHandlingInstance(ArgumentHandlingCodeProvider<ArgumentType> instance)
        //{
        //    throw new NotImplementedException();
        //    //handlingInstance = instance;
        //}
    }
}
