using Mono.Cecil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ILTools
{
    public interface IMethodProcessor
    {
        void Process([NotNull]MethodDefinition method, [NotNull]ILogger logger);
    }
}
