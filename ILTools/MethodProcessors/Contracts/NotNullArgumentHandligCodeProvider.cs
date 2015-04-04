using ILTools.MethodProcessors.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ILTools.MethodProcessors.Contracts
{
    class NotNullArgumentHandligCodeProvider<ArgumentType> : IArgumentHandlingCodeProvider<ArgumentType>
    {
        public void CheckPrerequisites()
        {
            if (!typeof(ArgumentType).IsClass && Nullable.GetUnderlyingType(typeof(ArgumentType)) == null)
            {
                throw new InvalidOperationException("ArgumentType must be reference type or nullable struct.");
            }
        }

        public void HandleArgument(ArgumentType argument, string argumentName)
        {
            if (argument == null) throw new ArgumentNullException(argumentName);
        }
    }
}
