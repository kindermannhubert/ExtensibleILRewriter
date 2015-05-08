using System;

namespace ExtensibleILRewriter.Extensions
{
    public static class TypeExtensions
    {
        public static bool IsDerivedFrom(this Type type, Type baseType)
        {
            if (type.BaseType == null)
            {
                return false;
            }

            if (type.BaseType == baseType)
            {
                return true;
            }

            return type.BaseType.IsDerivedFrom(baseType);
        }
    }
}
