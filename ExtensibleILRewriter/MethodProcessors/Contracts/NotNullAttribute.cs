using System;

namespace ExtensibleILRewriter.MethodProcessors.Contracts
{
    [AttributeUsage(AttributeTargets.Parameter, Inherited = false, AllowMultiple = false)]
    public sealed class NotNullAttribute : Attribute
    {
    }
}
