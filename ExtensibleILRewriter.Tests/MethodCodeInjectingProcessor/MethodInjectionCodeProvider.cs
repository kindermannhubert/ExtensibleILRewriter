using ExtensibleILRewriter.CodeInjection;
using ExtensibleILRewriter.Processors.Methods;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace ExtensibleILRewriter.Tests.MethodCodeInjectingProcessor
{
    internal class MethodInjectionCodeProvider : CodeProvider<MethodCodeInjectingCodeProviderArgument>
    {
        public const string InjectionPrefix = "Inject_";

        public override bool HasState { get { return true; } }

        protected override CodeProviderCallArgument[] GetCodeProvidingMethodArguments(MethodCodeInjectingCodeProviderArgument codeProviderArgument)
        {
            return new CodeProviderCallArgument[]
            {
                CodeProviderCallArgument.CreateStateArgument("state", GetStateType(), codeProviderArgument.StateField)
            };
        }

        protected override MethodInfo GetCodeProvidingMethod(MethodCodeInjectingCodeProviderArgument codeProviderArgument)
        {
            return GetType().GetMethod(nameof(InjectedMethod));
        }

        protected override bool ShouldBeInjected(MethodCodeInjectingCodeProviderArgument codeProviderArgument)
        {
            return codeProviderArgument.Method.Name.StartsWith(InjectionPrefix);
        }

        public override Type GetStateType()
        {
            return typeof(State);
        }

        public static void InjectedMethod(State state)
        {
            state.Items.Add("item");
        }

        internal class State
        {
            public List<string> Items { get; } = new List<string>();
        }
    }
}
