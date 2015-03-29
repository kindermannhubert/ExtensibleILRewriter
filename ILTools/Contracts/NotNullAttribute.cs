using System;

namespace ILTools
{
    [AttributeUsage(AttributeTargets.Parameter, Inherited = false, AllowMultiple = false)]
    public sealed class NotNullAttribute : Attribute
    {
    }
}
