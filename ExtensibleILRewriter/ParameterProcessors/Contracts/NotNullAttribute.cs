using System;

namespace ExtensibleILRewriter.ParameterProcessors.Contracts
{
    [AttributeUsage(AttributeTargets.Parameter, Inherited = false, AllowMultiple = false)]
    public sealed class NotNullAttribute : Attribute
    {
    }
}
