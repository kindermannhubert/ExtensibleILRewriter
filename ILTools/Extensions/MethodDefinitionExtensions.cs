using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Cecil.Rocks;

namespace ILTools.Extensions
{
    public static class MethodDefinitionExtensions
    {
        public static bool CouldBeStatic(this MethodDefinition method)
        {
            if (!method.HasBody) return false;
            if (method.IsStatic) return true;
            return !AccessesThis(method.Body);
        }

        private static bool AccessesThis(MethodBody methodBody)
        {
            foreach (var ins in methodBody.Instructions)
            {
                if (ins.OpCode.Code == Code.Ldarg_0) return true;
                if (ins.Operand as ParameterDefinition == methodBody.ThisParameter) return true;
            }
            return false;
        }
    }
}