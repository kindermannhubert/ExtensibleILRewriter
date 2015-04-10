using System;

namespace ExtensibleILRewriter.MethodProcessors.ArgumentHandling
{
    public abstract class ArgumentHandlingCodeProvider<ArgumentType>
    {
        private ArgumentHandlingCodeProvider<ArgumentType> handlingInstance;

        public ArgumentHandlingCodeProvider(ArgumentHandlingType type, string handlingInstanceName)
        {
            HandlingType = type;
            HandlingInstanceName = handlingInstanceName;

            switch (type)
            {
                case ArgumentHandlingType.CallStaticHandling:
                    if (handlingInstance != null) throw new InvalidOperationException("You cannot specify '\{handlingInstanceName}' when you are using '\{type}'.");
                    break;
                case ArgumentHandlingType.CallInstanceHandling:
                    if (handlingInstance == null) throw new InvalidOperationException("You must specify '\{handlingInstanceName}' when you are using '\{type}'.");
                    break;
                default:
                    throw new NotImplementedException("Unknown argument handling type: '\{type}'.");
            }

            //HandlingInstancesManager.RegisterInstance
        }

        public ArgumentHandlingType HandlingType { get; }

        public string HandlingInstanceName { get; }

        public abstract void CheckPrerequisites();

        public abstract void HandleArgument(ArgumentType argument, string argumentName);

        public void SetHandlingInstance(ArgumentHandlingCodeProvider<ArgumentType> instance)
        {
            handlingInstance = instance;
        }
    }
}
