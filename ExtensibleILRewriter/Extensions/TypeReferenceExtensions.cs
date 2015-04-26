using Mono.Cecil;
using System;

namespace ExtensibleILRewriter.Extensions
{
    public static class TypeReferenceExtensions
    {
        private static readonly string NullableGenericTypeFullName = typeof(Nullable<>).FullName;

        public static bool IsNullableValueType(this TypeReference type)
        {
            return type.IsValueType && type.IsGenericInstance && type.GetElementType().FullName == NullableGenericTypeFullName;
        }
    }
}
