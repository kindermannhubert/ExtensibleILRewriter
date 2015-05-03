using System;
using System.Xml.Serialization;

namespace ExtensibleILRewriter.MsBuild.Configuration
{
    public class ProcessorPropertyDefinition
    {
        [XmlAttribute("key")]
        public string Name { get; set; }

        [XmlAttribute("value")]
        public string Value { get; set; }

        public void Check()
        {
            if (string.IsNullOrWhiteSpace(Name))
            {
                throw new InvalidOperationException($"Configuration of {nameof(ProcessorPropertyDefinition)} must contain {nameof(Name)} attribute.");
            }

            if (string.IsNullOrWhiteSpace(Value))
            {
                throw new InvalidOperationException($"Configuration of {nameof(ProcessorPropertyDefinition)} must contain {nameof(Value)} attribute.");
            }
        }
    }
}
