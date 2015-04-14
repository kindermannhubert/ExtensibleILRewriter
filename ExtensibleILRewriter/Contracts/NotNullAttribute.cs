using System;

namespace ExtensibleILRewriter.Contracts
{
    [AttributeUsage(AttributeTargets.Parameter, Inherited = false, AllowMultiple = false)]
    public sealed class NotNullAttribute : Attribute
    {
    }
}
