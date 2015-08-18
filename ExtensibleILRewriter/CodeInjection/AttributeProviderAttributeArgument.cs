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

        public Type ClrType
        {
            get
            {
                if (Type == AttributeProviderAttributeArgumentType.Enum && Value != null)
                {
                    if (IsArray)
                    {
                        var values = (object[])Value;
                        if (values.Length > 0)
                        {
                            return values[0].GetType().MakeArrayType();
                        }
                    }
                    else
                    {
                        return Value.GetType();
                    }
                }

                var type = GetElementClrType();
                if (IsArray)
                {
                    type = type.MakeArrayType();
                }

                return type;
            }
        }

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
            var elementType = GetElementClrType();

            if (IsArray)
            {
                return GenerateArrayCustomAttributeArgument(destinationModule, elementType, (object[])Value);
            }
            else
            {
                return GenerateElementCustomAttributeArgument(destinationModule, elementType, Value);
            }
        }

        private Type GetElementClrType()
        {
            switch (Type)
            {
                case AttributeProviderAttributeArgumentType.Byte:
                    return typeof(Byte);
                case AttributeProviderAttributeArgumentType.SByte:
                    return typeof(SByte);
                case AttributeProviderAttributeArgumentType.Int16:
                    return typeof(Int16);
                case AttributeProviderAttributeArgumentType.UInt16:
                    return typeof(UInt16);
                case AttributeProviderAttributeArgumentType.Char:
                    return typeof(Char);
                case AttributeProviderAttributeArgumentType.Int32:
                    return typeof(Int32);
                case AttributeProviderAttributeArgumentType.UInt32:
                    return typeof(UInt32);
                case AttributeProviderAttributeArgumentType.Single:
                    return typeof(Single);
                case AttributeProviderAttributeArgumentType.Int64:
                    return typeof(Int64);
                case AttributeProviderAttributeArgumentType.UInt64:
                    return typeof(UInt64);
                case AttributeProviderAttributeArgumentType.Double:
                    return typeof(Double);
                case AttributeProviderAttributeArgumentType.Type:
                    return typeof(Type);
                case AttributeProviderAttributeArgumentType.Enum:
                    return typeof(Enum);
                case AttributeProviderAttributeArgumentType.String:
                    return typeof(String);
                default:
                    throw new NotImplementedException($"Unknown {nameof(CodeProviderCallArgument)} type '{Type}'.");
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