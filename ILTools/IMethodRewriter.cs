using Mono.Cecil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ILTools
{
    public interface IMethodRewriter
    {
        void Rewrite(MethodDefinition method);
    }
}
