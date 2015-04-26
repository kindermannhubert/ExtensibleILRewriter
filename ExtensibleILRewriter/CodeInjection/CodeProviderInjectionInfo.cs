using Mono.Cecil;

namespace ExtensibleILRewriter.CodeInjection
{
    public struct CodeProviderInjectionInfo
    {
        public bool ShouldBeCallInjected { get; }
        public MethodReference MethodReferenceToBeCalled { get; }
        public CodeProviderCallArgument[] CallArguments { get; }

        public CodeProviderInjectionInfo(bool shouldBeCallInjected, MethodReference methodReferenceToBeCalled, CodeProviderCallArgument[] callArguments)
        {
            ShouldBeCallInjected = shouldBeCallInjected;
            MethodReferenceToBeCalled = methodReferenceToBeCalled;
            CallArguments = callArguments;
        }
    }
}
