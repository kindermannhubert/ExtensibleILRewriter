using System;

namespace ExtensibleILRewriter.Tests.AddAttributeProcessor
{
    [AttributeUsage(AttributeTargets.All)]
    public class InjectedAttribute : Attribute
    {
        public InjectedAttribute(ProcessableComponentType component, Type type, int nameHash, string name)
        {
        }
    }
}
