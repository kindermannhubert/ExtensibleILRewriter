using Mono.Cecil;
using System;
using System.Reflection;

namespace ExtensibleILRewriter.ParameterProcessors
{
    public abstract class ParameterValueHandlingCodeProvider<StateType> : IParameterValueHandlingCodeProvider
    {
        public abstract bool ShouldHandleParameter(ParameterDefinition parameterDefinition, MethodDefinition declaringMethod);

        public abstract MethodInfo GetHandleParameterMethodInfo(TypeReference parameterType);

        protected MethodInfo GetHandleParameterMethodInfo(string methodName)
        {
            var method = this.GetType().GetMethod(methodName);
            if (method == null) throw new InvalidOperationException("Unable to find parameter handling method '\{methodName}' on type '\{this.GetType().FullName}'.");
            return method;
        }

        //public abstract void HandleParameter<ParameterType>(StateType state, ParameterType parameter, string parameterName);

        public Type GetStateType()
        {
            return typeof(StateType);
        }
    }

    public interface IParameterValueHandlingCodeProvider
    {
        bool ShouldHandleParameter(ParameterDefinition parameterDefinition, MethodDefinition declaringMethod);

        MethodInfo GetHandleParameterMethodInfo(TypeReference parameterType);

        Type GetStateType();
    }

    public class EmptyCodeProviderState { }
}
