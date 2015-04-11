using Mono.Cecil;
using System;

namespace ExtensibleILRewriter.MethodProcessors.ArgumentHandling
{
    public interface IArgumentHandlingCodeProvider
    {
        ArgumentHandlingType HandlingType { get; }
        void CheckPrerequisites();
    }
}
