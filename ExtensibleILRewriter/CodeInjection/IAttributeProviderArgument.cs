namespace ExtensibleILRewriter.CodeInjection
{
    public interface IAttributeProviderArgument<ComponentType, DeclaringComponentType>
    {
        ComponentType Component { get; }

        DeclaringComponentType DeclaringComponent { get; }
    }
}
