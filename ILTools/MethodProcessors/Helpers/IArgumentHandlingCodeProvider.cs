using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ILTools.MethodProcessors.Helpers
{
    public interface IArgumentHandlingCodeProvider<ArgumentType>
    {
        void CheckPrerequisites();
        void HandleArgument(ArgumentType argument, string argumentName);
    }
}
