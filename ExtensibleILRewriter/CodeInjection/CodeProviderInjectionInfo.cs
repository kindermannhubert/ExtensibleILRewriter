using Mono.Cecil;

namespace ExtensibleILRewriter.CodeInjection
{
    public struct CodeProviderInjectionInfo
    {
        public CodeProviderInjectionInfo(MethodReference methodReferenceToBeCalled, CodeProviderCallArgument[] callArguments)
        {
            MethodReferenceToBeCalled = methodReferenceToBeCalled;
            CallArguments = callArguments;
        }

        public MethodReference MethodReferenceToBeCalled { get; }

        public CodeProviderCallArgument[] CallArguments { get; }
    }
}
