using ExtensibleILRewriter.Processors.Parameters;
using Mono.Cecil;
using System;

namespace ExtensibleILRewriter.CodeInjection
{
    public struct AttributeProviderAttributeArgument
    {
        public string Name { get; }

        public AttributeProviderAttributeArgumentType Type { get; }

        public object Value { get; }

        private AttributeProviderAttributeArgument([NotNull]string name, AttributeProviderAttributeArgumentType type, object value)
        {
            Name = name;
            Type = type;
            Value = value;
        }

        public static AttributeProviderAttributeArgument CreateParameterArgument([NotNull]string name, Byte value)
        {
            return new AttributeProviderAttributeArgument(name, AttributeProviderAttributeArgumentType.Byte, value);
        }

        public static AttributeProviderAttributeArgument CreateParameterArgument([NotNull]string name, SByte value)
        {
            return new AttributeProviderAttributeArgument(name, AttributeProviderAttributeArgumentType.SByte, value);
        }

        public static AttributeProviderAttributeArgument CreateParameterArgument([NotNull]string name, Int16 value)
        {
            return new AttributeProviderAttributeArgument(name, AttributeProviderAttributeArgumentType.Int16, value);
        }

        public static AttributeProviderAttributeArgument CreateParameterArgument([NotNull]string name, UInt16 value)
        {
            return new AttributeProviderAttributeArgument(name, AttributeProviderAttributeArgumentType.UInt16, value);
        }

        public static AttributeProviderAttributeArgument CreateParameterArgument([NotNull]string name, Char value)
        {
            return new AttributeProviderAttributeArgument(name, AttributeProviderAttributeArgumentType.Char, value);
        }

        public static AttributeProviderAttributeArgument CreateParameterArgument([NotNull]string name, Int32 value)
        {
            return new AttributeProviderAttributeArgument(name, AttributeProviderAttributeArgumentType.Int32, value);
        }

        public static AttributeProviderAttributeArgument CreateParameterArgument([NotNull]string name, UInt32 value)
        {
            return new AttributeProviderAttributeArgument(name, AttributeProviderAttributeArgumentType.UInt32, value);
        }

        public static AttributeProviderAttributeArgument CreateParameterArgument([NotNull]string name, Single value)
        {
            return new AttributeProviderAttributeArgument(name, AttributeProviderAttributeArgumentType.Single, value);
        }

        public static AttributeProviderAttributeArgument CreateParameterArgument([NotNull]string name, Int64 value)
        {
            return new AttributeProviderAttributeArgument(name, AttributeProviderAttributeArgumentType.Int64, value);
        }

        public static AttributeProviderAttributeArgument CreateParameterArgument([NotNull]string name, UInt64 value)
        {
            return new AttributeProviderAttributeArgument(name, AttributeProviderAttributeArgumentType.UInt64, value);
        }

        public static AttributeProviderAttributeArgument CreateParameterArgument([NotNull]string name, Double value)
        {
            return new AttributeProviderAttributeArgument(name, AttributeProviderAttributeArgumentType.Double, value);
        }

        public static AttributeProviderAttributeArgument CreateParameterArgument([NotNull]string name, Type value)
        {
            return new AttributeProviderAttributeArgument(name, AttributeProviderAttributeArgumentType.Type, value);
        }

        public static AttributeProviderAttributeArgument CreateParameterArgument([NotNull]string name, Enum value)
        {
            return new AttributeProviderAttributeArgument(name, AttributeProviderAttributeArgumentType.Enum, value);
        }

        public static AttributeProviderAttributeArgument CreateParameterArgument([NotNull]string name, String value)
        {
            return new AttributeProviderAttributeArgument(name, AttributeProviderAttributeArgumentType.String, value);
        }

        public CustomAttributeArgument GenerateCustomAttributeArgument([NotNull]ModuleDefinition destinationModule)
        {
            switch (Type)
            {
                case AttributeProviderAttributeArgumentType.Byte:
                    return new CustomAttributeArgument(destinationModule.Import(typeof(Byte)), Value);
                case AttributeProviderAttributeArgumentType.SByte:
                    return new CustomAttributeArgument(destinationModule.Import(typeof(SByte)), Value);
                case AttributeProviderAttributeArgumentType.Int16:
                    return new CustomAttributeArgument(destinationModule.Import(typeof(Int16)), Value);
                case AttributeProviderAttributeArgumentType.UInt16:
                    return new CustomAttributeArgument(destinationModule.Import(typeof(UInt16)), Value);
                case AttributeProviderAttributeArgumentType.Char:
                    return new CustomAttributeArgument(destinationModule.Import(typeof(Char)), Value);
                case AttributeProviderAttributeArgumentType.Int32:
                    return new CustomAttributeArgument(destinationModule.Import(typeof(Int32)), Value);
                case AttributeProviderAttributeArgumentType.UInt32:
                    return new CustomAttributeArgument(destinationModule.Import(typeof(UInt32)), Value);
                case AttributeProviderAttributeArgumentType.Single:
                    return new CustomAttributeArgument(destinationModule.Import(typeof(Single)), Value);
                case AttributeProviderAttributeArgumentType.Int64:
                    return new CustomAttributeArgument(destinationModule.Import(typeof(Int64)), Value);
                case AttributeProviderAttributeArgumentType.UInt64:
                    return new CustomAttributeArgument(destinationModule.Import(typeof(UInt64)), Value);
                case AttributeProviderAttributeArgumentType.Double:
                    return new CustomAttributeArgument(destinationModule.Import(typeof(Double)), Value);
                case AttributeProviderAttributeArgumentType.Type:
                    return new CustomAttributeArgument(destinationModule.Import(typeof(Type)), Value);
                case AttributeProviderAttributeArgumentType.Enum:
                    return new CustomAttributeArgument(destinationModule.Import(typeof(Enum)), Value);
                case AttributeProviderAttributeArgumentType.String:
                    return new CustomAttributeArgument(destinationModule.Import(typeof(String)), Value);
                default:
                    throw new NotImplementedException("Unknown \{nameof(CodeProviderCallArgument)} type '\{Type}'.");
            }

            //TODO arrays
            //var byteArrayTypeReference = new ArrayType(byteTypeReference);
            //var customAttributeArgument = new CustomAttributeArgument(byteArrayTypeReference, new byte[] { 1, 2, 3, 4, 5, 7 }.Select(v => new CustomAttributeArgument(byteTypeReference, v)).ToArray());
        }
    }
}