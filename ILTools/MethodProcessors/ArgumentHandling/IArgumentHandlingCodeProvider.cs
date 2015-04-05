using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ILTools.MethodProcessors.ArgumentHandling
{
    public interface IArgumentHandlingCodeProvider<ArgumentType>
    {
        void CheckPrerequisites();
        bool MakeArgumentHandligMethodStatic { get; }
        IArgumentHandlingCodeProvider<ArgumentType> HandlingObject { get; }
        void HandleArgument(ArgumentType argument, string argumentName);
    }
}
