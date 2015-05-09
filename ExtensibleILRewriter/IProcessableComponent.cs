using Mono.Cecil;
using Mono.Collections.Generic;

namespace ExtensibleILRewriter
{
    public interface IProcessableComponent
    {
        ProcessableComponentType Type { get; }

        Collection<CustomAttribute> CustomAttributes { get; }

        string Name { get; }

        string FullName { get; }

        IProcessableComponent DeclaringComponent { get; }

        ModuleDefinition DeclaringModule { get; }

        object UnderlyingComponent { get; }
    }
}
