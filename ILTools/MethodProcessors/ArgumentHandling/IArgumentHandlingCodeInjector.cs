using Mono.Cecil;
using Mono.Cecil.Cil;

namespace ILTools.MethodProcessors.ArgumentHandling
{
    public interface IArgumentHandlingCodeInjector
    {
        void Inject(MethodDefinition method, ParameterDefinition parameter, ILogger logger);
    }
}
