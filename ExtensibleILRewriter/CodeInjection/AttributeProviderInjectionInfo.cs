using Mono.Cecil;

namespace ExtensibleILRewriter.CodeInjection
{
    public struct AttributeProviderInjectionInfo
    {
        public AttributeProviderInjectionInfo(bool shouldBeAttributeInjected, CustomAttribute customAttribute)
        {
            ShouldBeAttributeInjected = shouldBeAttributeInjected;
            CustomAttribute = customAttribute;
        }

        public bool ShouldBeAttributeInjected { get; }

        public CustomAttribute CustomAttribute { get; }
    }
}
