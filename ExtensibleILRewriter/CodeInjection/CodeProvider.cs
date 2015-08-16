using ExtensibleILRewriter.Extensions;
using ExtensibleILRewriter.Processors.Parameters;
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

        protected abstract MethodInfo GetCodeProvidingMethod(CodeProviderArgumentType codeProviderArgument);

        protected abstract CodeProviderCallArgument[] GetCodeProvidingMethodArguments(CodeProviderArgumentType codeProviderArgument);

        protected virtual TypeReference[] GetCodeProvidingMethodGenericArgumentTypes(CodeProviderArgumentType codeProviderArgument)
        {
            throw new NotImplementedException($"For usage of generic code providing method {nameof(GetCodeProvidingMethodGenericArgumentTypes)} must be properly implemented.");
        }

        protected MethodReference GetAndCheckCodeProvidingMethodReference([NotNull]MethodInfo method, [NotNull]CodeProviderCallArgument[] codeProvidingMethodArguments, [NotNull]ModuleDefinition destinationModule)
        {
            var methodDeclaringType = method.DeclaringType;

            if (!method.IsStatic)
            {
                throw new InvalidOperationException($"Method '{method.Name}' on type '{methodDeclaringType.FullName}' must be static to be injected.");
            }

            if (!method.IsPublic)
            {
                throw new InvalidOperationException($"Method '{method.Name}' on type '{methodDeclaringType.FullName}' must be public to be injected.");
            }

            if (method.ReturnParameter.ParameterType != typeof(void))
            {
                throw new InvalidOperationException($"Method '{method.Name}' on type '{methodDeclaringType.FullName}' must have Void return type to be injected. Now it is '{method.ReturnParameter.ParameterType.FullName}'.");
            }

            var methodParams = method.GetParameters();

            if (methodParams.Length != codeProvidingMethodArguments.Length)
            {
                throw new InvalidOperationException($"Method '{method.Name}' on type '{methodDeclaringType.FullName}' should contain {codeProvidingMethodArguments.Length} parameters to be injected.");
            }

            for (int i = 0; i < methodParams.Length; i++)
            {
                if (methodParams[i].Name != codeProvidingMethodArguments[i].Name)
                {
                    throw new InvalidOperationException($"{i}. parameter of method '{method.Name}' on type '{methodDeclaringType.FullName}' should be named '{codeProvidingMethodArguments[i].Name}'.");
                }

                if (methodParams[i].ParameterType != codeProvidingMethodArguments[i].ClrType)
                {
                    if (codeProvidingMethodArguments[i].ClrType != null)
                    {
                        throw new InvalidOperationException($"Type of {i}. parameter of method '{method.Name}' on type '{methodDeclaringType.FullName}' should be named '{codeProvidingMethodArguments[i].ClrType.FullName}'.");
                    }

                    if (!methodParams[i].ParameterType.IsGenericParameter)
                    {
                        throw new InvalidOperationException($"Parameter '{codeProvidingMethodArguments[i].Name}' of method '{method.Name}' on type '{methodDeclaringType.FullName}' should be generic.");
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
                var methodInfo = GetCodeProvidingMethod(codeProviderArgument);
                var methodArguments = GetCodeProvidingMethodArguments(codeProviderArgument) ?? CodeProviderCallArgument.EmptyCollection;
                var methodReference = GetAndCheckCodeProvidingMethodReference(methodInfo, methodArguments, destinationModule);

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
