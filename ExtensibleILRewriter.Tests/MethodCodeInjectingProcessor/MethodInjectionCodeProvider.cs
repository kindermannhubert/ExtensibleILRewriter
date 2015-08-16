using ExtensibleILRewriter.CodeInjection;
using ExtensibleILRewriter.Processors.Methods;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ExtensibleILRewriter.Tests.MethodCodeInjectingProcessor
{
    internal class MethodInjectionCodeProvider : CodeProvider<MethodCodeInjectingCodeProviderArgument>
    {
        public const string InjectionPrefix = "Inject_";

        public override bool HasState { get { return false; } }

        protected override CodeProviderCallArgument[] GetCodeProvidingMethodArguments(MethodCodeInjectingCodeProviderArgument codeProviderArgument)
        {
            return null;
        }

        protected override MethodInfo GetCodeProvidingMethod(MethodCodeInjectingCodeProviderArgument codeProviderArgument)
        {
            return GetType().GetMethod(nameof(InjectedMethod));
        }

        protected override bool ShouldBeInjected(MethodCodeInjectingCodeProviderArgument codeProviderArgument)
        {
            return codeProviderArgument.Method.Name.StartsWith(InjectionPrefix);
        }

        public static void InjectedMethod()
        {

        }
    }
}
