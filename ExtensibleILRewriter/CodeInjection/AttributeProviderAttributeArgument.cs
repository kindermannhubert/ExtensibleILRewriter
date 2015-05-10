using ExtensibleILRewriter.Processors.Parameters;
using Mono.Cecil;
using System;
using System.Linq;

namespace ExtensibleILRewriter.CodeInjection
{
    public struct AttributeProviderAttributeArgument
    {
        private AttributeProviderAttributeArgument([NotNull]string name, AttributeProviderAttributeArgumentType type, object value)
        {
            Name = name;
            Type = type;
            Value = value;
            IsArray = false;
        }

        private AttributeProviderAttributeArgument([NotNull]string name, AttributeProviderAttributeArgumentType type, object[] value)
        {
            Name = name;
            Type = type;
            Value = value;
            IsArray = true;
        }

        public string Name { get; }

        public AttributeProviderAttributeArgumentType Type { get; }

        public object Value { get; }

        public bool IsArray { get; }

        public static AttributeProviderAttributeArgument CreateParameterArgument([NotNull]string name, Byte value)
        {
            return new AttributeProviderAttributeArgument(name, AttributeProviderAttributeArgumentType.Byte, value);
        }

        public static AttributeProviderAttributeArgument CreateParameterArgument([NotNull]string name, Byte[] value)
        {
            return new AttributeProviderAttributeArgument(name, AttributeProviderAttributeArgumentType.Byte, ToObjectArray(value));
        }

        public static AttributeProviderAttributeArgument CreateParameterArgument([NotNull]string name, SByte value)
        {
            return new AttributeProviderAttributeArgument(name, AttributeProviderAttributeArgumentType.SByte, value);
        }

        public static AttributeProviderAttributeArgument CreateParameterArgument([NotNull]string name, SByte[] value)
        {
            return new AttributeProviderAttributeArgument(name, AttributeProviderAttributeArgumentType.SByte, ToObjectArray(value));
        }

        public static AttributeProviderAttributeArgument CreateParameterArgument([NotNull]string name, Int16 value)
        {
            return new AttributeProviderAttributeArgument(name, AttributeProviderAttributeArgumentType.Int16, value);
        }

        public static AttributeProviderAttributeArgument CreateParameterArgument([NotNull]string name, Int16[] value)
        {
            return new AttributeProviderAttributeArgument(name, AttributeProviderAttributeArgumentType.Int16, ToObjectArray(value));
        }

        public static AttributeProviderAttributeArgument CreateParameterArgument([NotNull]string name, UInt16 value)
        {
            return new AttributeProviderAttributeArgument(name, AttributeProviderAttributeArgumentType.UInt16, value);
        }

        public static AttributeProviderAttributeArgument CreateParameterArgument([NotNull]string name, UInt16[] value)
        {
            return new AttributeProviderAttributeArgument(name, AttributeProviderAttributeArgumentType.UInt16, ToObjectArray(value));
        }

        public static AttributeProviderAttributeArgument CreateParameterArgument([NotNull]string name, Char value)
        {
            return new AttributeProviderAttributeArgument(name, AttributeProviderAttributeArgumentType.Char, value);
        }

        public static AttributeProviderAttributeArgument CreateParameterArgument([NotNull]string name, Char[] value)
        {
            return new AttributeProviderAttributeArgument(name, AttributeProviderAttributeArgumentType.Char, ToObjectArray(value));
        }

        public static AttributeProviderAttributeArgument CreateParameterArgument([NotNull]string name, Int32 value)
        {
            return new AttributeProviderAttributeArgument(name, AttributeProviderAttributeArgumentType.Int32, value);
        }

        public static AttributeProviderAttributeArgument CreateParameterArgument([NotNull]string name, Int32[] value)
        {
            return new AttributeProviderAttributeArgument(name, AttributeProviderAttributeArgumentType.Int32, ToObjectArray(value));
        }

        public static AttributeProviderAttributeArgument CreateParameterArgument([NotNull]string name, UInt32 value)
        {
            return new AttributeProviderAttributeArgument(name, AttributeProviderAttributeArgumentType.UInt32, value);
        }

        public static AttributeProviderAttributeArgument CreateParameterArgument([NotNull]string name, UInt32[] value)
        {
            return new AttributeProviderAttributeArgument(name, AttributeProviderAttributeArgumentType.UInt32, ToObjectArray(value));
        }

        public static AttributeProviderAttributeArgument CreateParameterArgument([NotNull]string name, Single value)
        {
            return new AttributeProviderAttributeArgument(name, AttributeProviderAttributeArgumentType.Single, value);
        }

        public static AttributeProviderAttributeArgument CreateParameterArgument([NotNull]string name, Single[] value)
        {
            return new AttributeProviderAttributeArgument(name, AttributeProviderAttributeArgumentType.Single, ToObjectArray(value));
        }

        public static AttributeProviderAttributeArgument CreateParameterArgument([NotNull]string name, Int64 value)
        {
            return new AttributeProviderAttributeArgument(name, AttributeProviderAttributeArgumentType.Int64, value);
        }

        public static AttributeProviderAttributeArgument CreateParameterArgument([NotNull]string name, Int64[] value)
        {
            return new AttributeProviderAttributeArgument(name, AttributeProviderAttributeArgumentType.Int64, ToObjectArray(value));
        }

        public static AttributeProviderAttributeArgument CreateParameterArgument([NotNull]string name, UInt64 value)
        {
            return new AttributeProviderAttributeArgument(name, AttributeProviderAttributeArgumentType.UInt64, value);
        }

        public static AttributeProviderAttributeArgument CreateParameterArgument([NotNull]string name, UInt64[] value)
        {
            return new AttributeProviderAttributeArgument(name, AttributeProviderAttributeArgumentType.UInt64, ToObjectArray(value));
        }

        public static AttributeProviderAttributeArgument CreateParameterArgument([NotNull]string name, Double value)
        {
            return new AttributeProviderAttributeArgument(name, AttributeProviderAttributeArgumentType.Double, value);
        }

        public static AttributeProviderAttributeArgument CreateParameterArgument([NotNull]string name, Double[] value)
        {
            return new AttributeProviderAttributeArgument(name, AttributeProviderAttributeArgumentType.Double, ToObjectArray(value));
        }

        public static AttributeProviderAttributeArgument CreateParameterArgument([NotNull]string name, TypeReference value)
        {
            return new AttributeProviderAttributeArgument(name, AttributeProviderAttributeArgumentType.Type, value);
        }

        public static AttributeProviderAttributeArgument CreateParameterArgument([NotNull]string name, TypeReference[] value)
        {
            return new AttributeProviderAttributeArgument(name, AttributeProviderAttributeArgumentType.Type, ToObjectArray(value));
        }

        public static AttributeProviderAttributeArgument CreateParameterArgument([NotNull]string name, Enum value)
        {
            return new AttributeProviderAttributeArgument(name, AttributeProviderAttributeArgumentType.Enum, value);
        }

        public static AttributeProviderAttributeArgument CreateParameterArgument([NotNull]string name, Enum[] value)
        {
            return new AttributeProviderAttributeArgument(name, AttributeProviderAttributeArgumentType.Enum, ToObjectArray(value));
        }

        public static AttributeProviderAttributeArgument CreateParameterArgument([NotNull]string name, String value)
        {
            return new AttributeProviderAttributeArgument(name, AttributeProviderAttributeArgumentType.String, value);
        }

        public static AttributeProviderAttributeArgument CreateParameterArgument([NotNull]string name, String[] value)
        {
            return new AttributeProviderAttributeArgument(name, AttributeProviderAttributeArgumentType.String, ToObjectArray(value));
        }

        public CustomAttributeArgument GenerateCustomAttributeArgument([NotNull]ModuleDefinition destinationModule)
        {
            switch (Type)
            {
                case AttributeProviderAttributeArgumentType.Byte:
                    return GenerateCustomAttributeArgument(destinationModule, typeof(Byte));
                case AttributeProviderAttributeArgumentType.SByte:
                    return GenerateCustomAttributeArgument(destinationModule, typeof(SByte));
                case AttributeProviderAttributeArgumentType.Int16:
                    return GenerateCustomAttributeArgument(destinationModule, typeof(Int16));
                case AttributeProviderAttributeArgumentType.UInt16:
                    return GenerateCustomAttributeArgument(destinationModule, typeof(UInt16));
                case AttributeProviderAttributeArgumentType.Char:
                    return GenerateCustomAttributeArgument(destinationModule, typeof(Char));
                case AttributeProviderAttributeArgumentType.Int32:
                    return GenerateCustomAttributeArgument(destinationModule, typeof(Int32));
                case AttributeProviderAttributeArgumentType.UInt32:
                    return GenerateCustomAttributeArgument(destinationModule, typeof(UInt32));
                case AttributeProviderAttributeArgumentType.Single:
                    return GenerateCustomAttributeArgument(destinationModule, typeof(Single));
                case AttributeProviderAttributeArgumentType.Int64:
                    return GenerateCustomAttributeArgument(destinationModule, typeof(Int64));
                case AttributeProviderAttributeArgumentType.UInt64:
                    return GenerateCustomAttributeArgument(destinationModule, typeof(UInt64));
                case AttributeProviderAttributeArgumentType.Double:
                    return GenerateCustomAttributeArgument(destinationModule, typeof(Double));
                case AttributeProviderAttributeArgumentType.Type:
                    return GenerateCustomAttributeArgument(destinationModule, typeof(Type));
                case AttributeProviderAttributeArgumentType.Enum:
                    return GenerateCustomAttributeArgument(destinationModule, typeof(Enum));
                case AttributeProviderAttributeArgumentType.String:
                    return GenerateCustomAttributeArgument(destinationModule, typeof(String));
                default:
                    throw new NotImplementedException($"Unknown {nameof(CodeProviderCallArgument)} type '{Type}'.");
            }
        }

        private CustomAttributeArgument GenerateCustomAttributeArgument([NotNull]ModuleDefinition destinationModule, [NotNull]Type valueType)
        {
            if (IsArray)
            {
                return GenerateArrayCustomAttributeArgument(destinationModule, valueType, (object[])Value);
            }
            else
            {
                return GenerateElementCustomAttributeArgument(destinationModule, valueType, Value);
            }
        }

        private static CustomAttributeArgument GenerateElementCustomAttributeArgument([NotNull]ModuleDefinition destinationModule, [NotNull]Type valueType, object value)
        {
            return new CustomAttributeArgument(destinationModule.Import(valueType), value);
        }

        private static CustomAttributeArgument GenerateArrayCustomAttributeArgument([NotNull]ModuleDefinition destinationModule, [NotNull]Type valueType, object[] values)
        {
            var typeReference = destinationModule.Import(valueType);
            var typeArrayReference = new ArrayType(typeReference);
            var elementArguments = values.Select(v => new CustomAttributeArgument(typeReference, v)).ToArray();
            return new CustomAttributeArgument(typeArrayReference, elementArguments);
        }

        private static object[] ToObjectArray<T>(T[] array)
        {
            return array.Select(v => (object)v).ToArray();
        }
    }
}