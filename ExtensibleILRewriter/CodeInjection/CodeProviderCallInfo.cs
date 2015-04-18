using Mono.Cecil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExtensibleILRewriter.CodeInjection
{
    public struct CodeProviderCallInfo
    {
        public bool ShouldBeCallInjected { get; }
        public MethodReference MethodReferenceToBeCalled { get; }
        public CodeProviderCallArgument[] CallArguments { get; }

        public CodeProviderCallInfo(bool shouldBeCallInjected, MethodReference methodReferenceToBeCalled, CodeProviderCallArgument[] callArguments)
        {
            ShouldBeCallInjected = shouldBeCallInjected;
            MethodReferenceToBeCalled = methodReferenceToBeCalled;
            CallArguments = callArguments;
        }
    }
}
