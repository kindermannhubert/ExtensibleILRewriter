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
        public const string NoInputItem = "no input";

        public override bool HasState { get { return true; } }

        protected override CodeProviderCallArgument[] GetCodeProvidingMethodArguments(MethodCodeInjectingCodeProviderArgument codeProviderArgument)
        {
            var parameters = codeProviderArgument.Method.UnderlyingComponent.Parameters;
            if (parameters.Count > 0 && parameters[0].ParameterType.FullName == typeof(int).FullName)
            {
                return new CodeProviderCallArgument[]
                   {
                        CodeProviderCallArgument.CreateStateArgument("state", GetStateType(), codeProviderArgument.StateField),
                        CodeProviderCallArgument.CreateParameterArgument("value", typeof(int), codeProviderArgument.Method.UnderlyingComponent.Parameters[0])
                   };
            }
            else
            {
                return new CodeProviderCallArgument[]
                   {
                        CodeProviderCallArgument.CreateStateArgument("state", GetStateType(), codeProviderArgument.StateField),
                   };
            }
        }

        protected override MethodInfo GetCodeProvidingMethod(MethodCodeInjectingCodeProviderArgument codeProviderArgument)
        {
            var parameters = codeProviderArgument.Method.UnderlyingComponent.Parameters;
            if (parameters.Count > 0 && parameters[0].ParameterType.FullName == typeof(int).FullName)
            {
                return GetType().GetMethod(nameof(InjectedMethod_Value));
            }
            else
            {
                return GetType().GetMethod(nameof(InjectedMethod_NoValue));
            }
        }

        protected override bool ShouldBeInjected(MethodCodeInjectingCodeProviderArgument codeProviderArgument)
        {
            return codeProviderArgument.Method.Name.StartsWith(InjectionPrefix);
        }

        public override Type GetStateType()
        {
            return typeof(State);
        }

        public static void InjectedMethod_NoValue(State state)
        {
            state.Items.Add(NoInputItem);
        }

        public static void InjectedMethod_Value(State state, int value)
        {
            state.Items.Add(value.ToString());
        }

        internal class State
        {
            public List<string> Items { get; } = new List<string>();
        }
    }
}
