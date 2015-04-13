using Mono.Cecil;
using System;
using System.Linq;
using System.Reflection;

namespace ExtensibleILRewriter.ParameterProcessors
{
    public abstract class ParameterValueHandlingCodeProvider<StateType> : IParameterValueHandlingCodeProvider
    {
        public abstract bool ShouldHandleParameter(ParameterDefinition parameterDefinition, MethodDefinition declaringMethod);

        public MethodInfo GetHandleParameterMethodInfo(TypeReference parameterType)
        {
            var methodName = GetHandleParameterMethodName(parameterType);

            var handlingMethods = GetType()
                .GetMethods(BindingFlags.Static | BindingFlags.Public)
                .Where(m => m.Name == methodName).ToArray();

            if (handlingMethods.Length == 0) throw new InvalidOperationException("Unable to find parameter handling method '\{methodName}' on type '\{GetType().FullName}'.");
            else if (handlingMethods.Length > 1) throw new InvalidOperationException("Found more than one parameter handling method with name '\{methodName}' on type '\{GetType().FullName}'.");

            var method = handlingMethods[0];

            if (method.ReturnParameter.ParameterType != typeof(void))
                throw new InvalidOperationException("Parameter handling method '\{methodName}' on type '\{GetType().FullName}' must have Void return type. Now it is '\{method.ReturnParameter.ParameterType.FullName}'.");

            var methodParams = method.GetParameters();

            if (methodParams.Length != 3)
                throw new InvalidOperationException("Parameter handling method '\{methodName}' on type '\{GetType().FullName}' must contains 3 parameters {StateType state, ParameterType parameter, string parameterName}.");

            if (methodParams[0].ParameterType != typeof(StateType))
                throw new InvalidOperationException("First parameter of parameter handling method '\{methodName}' on type '\{GetType().FullName}' must be of StateType.");

            if (methodParams[2].ParameterType != typeof(string))
                throw new InvalidOperationException("Third parameter of parameter handling method '\{methodName}' on type '\{GetType().FullName}' must be of string type (it is a parameter name).");

            if (method.IsGenericMethod && method.ContainsGenericParameters)
            {
                var methodGenericArguments = method.GetGenericArguments();
                if (methodGenericArguments.Length != 1) throw new InvalidOperationException("Parameter handling method '\{methodName}' on type '\{GetType().FullName}' must contain one generic argument which is ParameterType.");
                if(methodGenericArguments[0] != methodParams[1].ParameterType) throw new InvalidOperationException("Second parameter of parameter handling method '\{methodName}' on type '\{GetType().FullName}' must be of same type as generic argument of method.");
            }
            else throw new InvalidOperationException("Parameter handling method '\{methodName}' on type '\{GetType().FullName}' must be generic.");

            return method;
        }

        protected abstract string GetHandleParameterMethodName(TypeReference parameterType);

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
