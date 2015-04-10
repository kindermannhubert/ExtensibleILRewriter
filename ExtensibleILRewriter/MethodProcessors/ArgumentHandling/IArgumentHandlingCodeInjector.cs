using Mono.Cecil;
using Mono.Cecil.Cil;

namespace ExtensibleILRewriter.MethodProcessors.ArgumentHandling
{
    public interface IArgumentHandlingCodeInjector
    {
        void Inject(MethodDefinition method, ParameterDefinition parameter, ILogger logger);
    }
}
