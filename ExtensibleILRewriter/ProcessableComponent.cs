using System;
using Mono.Cecil;
using Mono.Collections.Generic;

namespace ExtensibleILRewriter
{
    public abstract class ProcessableComponent<UnderlyingComponentType, DeclaringComponentType> : IProcessableComponent
        where DeclaringComponentType : IProcessableComponent
    {
        public ProcessableComponentType Type { get; }

        public string Name { get; }

        public string FullName { get; }

        public Collection<CustomAttribute> CustomAttributes { get; }

        IProcessableComponent IProcessableComponent.DeclaringComponent { get { return DeclaringComponent; } }

        public DeclaringComponentType DeclaringComponent { get; }

        public ModuleDefinition DeclaringModule { get; }

        public UnderlyingComponentType UnderlyingComponent { get; }

        protected ProcessableComponent(
            ProcessableComponentType type,
            UnderlyingComponentType underlyingComponent,
            string name,
            string fullName,
            Collection<CustomAttribute> customAttributes,
            DeclaringComponentType declaringComponent,
            ModuleDefinition declaringModule)
        {
            Type = type;
            UnderlyingComponent = underlyingComponent;
            Name = name;
            FullName = fullName;
            CustomAttributes = customAttributes;
            DeclaringComponent = declaringComponent;
            DeclaringModule = declaringModule;
        }
    }

    public class AssemblyProcessableComponent : ProcessableComponent<AssemblyDefinition, NoDeclaringComponent>
    {
        public AssemblyProcessableComponent(AssemblyDefinition assembly)
            : base(ProcessableComponentType.Assembly, assembly, assembly.Name.Name, assembly.FullName, assembly.CustomAttributes, new NoDeclaringComponent(), assembly.MainModule)
        { }
    }

    public class ModuleProcessableComponent : ProcessableComponent<ModuleDefinition, AssemblyProcessableComponent>
    {
        public ModuleProcessableComponent(ModuleDefinition module)
            : base(ProcessableComponentType.Module, module, module.Name, module.FullyQualifiedName, module.CustomAttributes, module.Assembly.ToProcessableComponent(), module)
        { }
    }

    public class TypeProcessableComponent : ProcessableComponent<TypeDefinition, ModuleProcessableComponent>
    {
        public TypeProcessableComponent(TypeDefinition type)
            : base(ProcessableComponentType.Type, type, type.Name, type.FullName, type.CustomAttributes, type.Module.ToProcessableComponent(), type.Module)
        { }
    }

    public class MethodProcessableComponent : ProcessableComponent<MethodDefinition, TypeProcessableComponent>
    {
        public MethodProcessableComponent(MethodDefinition method)
            : base(ProcessableComponentType.Method, method, method.Name, method.FullName, method.CustomAttributes, method.DeclaringType.ToProcessableComponent(), method.Module)
        { }
    }

    public class MethodParameterProcessableComponent : ProcessableComponent<ParameterDefinition, MethodProcessableComponent>
    {
        public MethodParameterProcessableComponent(ParameterDefinition parameter, MethodDefinition declaringMethod)
            : base(ProcessableComponentType.MethodParameter, parameter, parameter.Name, parameter.Name, parameter.CustomAttributes, declaringMethod.ToProcessableComponent(), declaringMethod.Module)
        { }
    }

    public class NoDeclaringComponent : IProcessableComponent
    {
        public Collection<CustomAttribute> CustomAttributes { get { throw new NotSupportedException(); } }

        public IProcessableComponent DeclaringComponent { get { throw new NotSupportedException(); } }

        public ModuleDefinition DeclaringModule { get { throw new NotSupportedException(); } }

        public string FullName { get { throw new NotSupportedException(); } }

        public string Name { get { throw new NotSupportedException(); } }

        public ProcessableComponentType Type { get { throw new NotSupportedException(); } }
    }

    public static class ProcessableComponentExtensions
    {
        public static AssemblyProcessableComponent ToProcessableComponent(this AssemblyDefinition assembly)
        {
            return new AssemblyProcessableComponent(assembly);
        }

        public static ModuleProcessableComponent ToProcessableComponent(this ModuleDefinition module)
        {
            return new ModuleProcessableComponent(module);
        }

        public static TypeProcessableComponent ToProcessableComponent(this TypeDefinition type)
        {
            return new TypeProcessableComponent(type);
        }

        public static MethodProcessableComponent ToProcessableComponent(this MethodDefinition method)
        {
            return new MethodProcessableComponent(method);
        }

        public static MethodParameterProcessableComponent ToProcessableComponent(this ParameterDefinition parameter, MethodDefinition declaringMethod)
        {
            return new MethodParameterProcessableComponent(parameter, declaringMethod);
        }
    }
}