using ILTools.MethodProcessors.Contracts;
using Mono.Cecil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ILTools
{
    public interface IComponentProcessor<ComponentType>
    {
        void Process([NotNull]ComponentType component, [NotNull]ILogger logger);
    }
}
