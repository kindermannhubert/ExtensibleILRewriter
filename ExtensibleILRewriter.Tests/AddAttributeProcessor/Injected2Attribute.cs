using System;

namespace ExtensibleILRewriter.Tests.AddAttributeProcessor
{
    [AttributeUsage(AttributeTargets.All)]
    public class Injected2Attribute : Attribute
    {
        public Injected2Attribute(ProcessableComponentType[] component, Type[] type, int[] nameHash, string[] name)
        {
        }
    }
}
