using ILTools.MethodProcessors.ArgumentHandling;
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
        public IArgumentHandlingCodeProvider<ArgumentType> HandlingObject
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public bool MakeArgumentHandligMethodStatic
        {
            get
            {
                return true;
            }
        }

        public void CheckPrerequisites()
        {
            if (!typeof(ArgumentType).IsClass && Nullable.GetUnderlyingType(typeof(ArgumentType)) == null)
            {
                throw new InvalidOperationException("ArgumentType must be reference type or nullable struct.");
            }
        }

        [MakeStaticVersion("__static_" + nameof(HandleArgument))]
        public void HandleArgument(ArgumentType argument, string argumentName)
        {
            if (argument == null) throw new ArgumentNullException(argumentName);
        }
    }
}
