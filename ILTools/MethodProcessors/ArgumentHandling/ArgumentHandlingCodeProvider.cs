using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ILTools.MethodProcessors.ArgumentHandling
{
    public abstract class ArgumentHandlingCodeProvider<ArgumentType>
    {
        public ArgumentHandlingCodeProvider(ArgumentHandlingType type)
        {
            HandlingType = type;
        }

        public ArgumentHandlingType HandlingType { get; }

        //public abstract ArgumentHandlingCodeProvider<ArgumentType> HandlingObject { get; }

        public abstract void CheckPrerequisites();
        public abstract void HandleArgument(ArgumentType argument, string argumentName);
    }
}
