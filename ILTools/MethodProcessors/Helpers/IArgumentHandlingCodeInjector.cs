using Mono.Cecil;
using Mono.Cecil.Cil;

namespace ILTools.MethodProcessors.Helpers
{
    public interface IArgumentHandlingCodeInjector
    {
        void Inject(MethodDefinition method, ParameterDefinition parameter, ILogger logger);
    }
}
