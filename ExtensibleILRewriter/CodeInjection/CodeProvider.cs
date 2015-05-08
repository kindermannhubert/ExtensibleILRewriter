using ExtensibleILRewriter.Extensions;
using Mono.Cecil;
using System;
using System.Linq;
using System.Reflection;

namespace ExtensibleILRewriter.CodeInjection
{
    // TODO - cacheing
    public abstract class CodeProvider<CodeProviderArgumentType>
    {
        public abstract bool HasState { get; }

        public virtual Type GetStateType()
        {
            if (!HasState)
            {
                throw new InvalidOperationException($"Cannot call method '{nameof(GetStateType)}' on code provider which does not have state. Code provider name: '{GetType().FullName}'.");
            }

            throw new NotImplementedException($"Method '{nameof(GetStateType)}' has to be overriden and implemented in order to support state for code provider '{GetType().FullName}'.");
        }

        protected abstract bool ShouldBeInjected(CodeProviderArgumentType codeProviderArgument);

        protected abstract string GetCodeProvidingMethodName(CodeProviderArgumentType codeProviderArgument);

        protected abstract CodeProviderCallArgument[] GetCodeProvidingMethodArguments(CodeProviderArgumentType codeProviderArgument);

        protected virtual TypeReference[] GetCodeProvidingMethodGenericArgumentTypes(CodeProviderArgumentType codeProviderArgument)
        {
            throw new NotImplementedException($"For usage of generic code providing method {nameof(GetCodeProvidingMethodGenericArgumentTypes)} must be properly implemented.");
        }

        protected MethodReference GetAndCheckCodeProvidingMethodReference(string codeProvidingMethodName, CodeProviderCallArgument[] codeProvidingMethodArguments, ModuleDefinition destinationModule)
        {
            var handlingMethods = GetType()
                .GetMethods(BindingFlags.Static | BindingFlags.Public)
                .Where(m => m.Name == codeProvidingMethodName).ToArray();

            if (handlingMethods.Length == 0)
            {
                throw new InvalidOperationException($"Unable to find public static method '{codeProvidingMethodName}' on type '{GetType().FullName}'.");
            }
            else if (handlingMethods.Length > 1)
            {
                throw new InvalidOperationException($"Found more than one public static method with name '{codeProvidingMethodName}' on type '{GetType().FullName}'.");
            }

            var method = handlingMethods[0];

            if (method.ReturnParameter.ParameterType != typeof(void))
            {
                throw new InvalidOperationException($"Method '{codeProvidingMethodName}' on type '{GetType().FullName}' must have Void return type. Now it is '{method.ReturnParameter.ParameterType.FullName}'.");
            }

            var methodParams = method.GetParameters();

            if (methodParams.Length != codeProvidingMethodArguments.Length)
            {
                throw new InvalidOperationException($"Method '{codeProvidingMethodName}' on type '{GetType().FullName}' should contain {codeProvidingMethodArguments.Length} parameters.");
            }

            for (int i = 0; i < methodParams.Length; i++)
            {
                if (methodParams[i].Name != codeProvidingMethodArguments[i].Name)
                {
                    throw new InvalidOperationException($"{i}. parameter of method '{codeProvidingMethodName}' on type '{GetType().FullName}' should be named '{codeProvidingMethodArguments[i].Name}'.");
                }

                if (methodParams[i].ParameterType != codeProvidingMethodArguments[i].ClrType)
                {
                    if (codeProvidingMethodArguments[i].ClrType != null || !methodParams[i].ParameterType.IsGenericParameter)
                    {
                        throw new InvalidOperationException($"Parameter '{codeProvidingMethodArguments[i].Name}' of method '{codeProvidingMethodName}' on type '{GetType().FullName}' should generic.");
                    }
                }
            }

            return destinationModule.Import(method);
        }

        public void CheckCodeProvidingMethodArguments(CodeProviderCallArgument[] requiredParameters)
        {
            var stateParametersCount = requiredParameters.Count(p => p.Type == CodeProviderCallArgumentType.FieldDefinition);

            if (HasState && stateParametersCount == 0)
            {
                throw new InvalidOperationException($"Code provider '{GetType().FullName}' declares it has state but contains zero FieldDefinition required parameters.");
            }

            if (stateParametersCount > 1)
            {
                throw new InvalidOperationException($"Code provider '{GetType().FullName}' contains more than one FieldDefinition required parameter.");
            }
        }

        public CodeProviderInjectionInfo GetCallInfo(CodeProviderArgumentType codeProviderArgument, ModuleDefinition destinationModule)
        {
            if (ShouldBeInjected(codeProviderArgument))
            {
                var methodName = GetCodeProvidingMethodName(codeProviderArgument);
                var methodArguments = GetCodeProvidingMethodArguments(codeProviderArgument);
                var methodReference = GetAndCheckCodeProvidingMethodReference(methodName, methodArguments, destinationModule);

                if (methodReference.ContainsGenericParameter)
                {
                    var genericArgumentTypes = GetCodeProvidingMethodGenericArgumentTypes(codeProviderArgument);
                    var genericMethod = new GenericInstanceMethod(methodReference);
                    genericMethod.GenericArguments.AddRange(genericArgumentTypes);
                    methodReference = genericMethod;
                }

                CheckCodeProvidingMethodArguments(methodArguments);
                return new CodeProviderInjectionInfo(true, methodReference, methodArguments);
            }
            else
            {
                return new CodeProviderInjectionInfo(false, null, null);
            }
        }
    }
}
