using System;

namespace ExtensibleILRewriter.Tests.AddAttributeProcessor
{
    [AttributeUsage(AttributeTargets.All)]
    public class Injected1Attribute : Attribute
    {
        public Injected1Attribute(ProcessableComponentType component, Type type, int nameHash, string name)
        {
        }
    }
}
