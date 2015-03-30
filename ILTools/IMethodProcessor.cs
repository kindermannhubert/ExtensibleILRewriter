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
        void Rewrite([NotNull]MethodDefinition method, [NotNull]List<string> errors);
    }
}
