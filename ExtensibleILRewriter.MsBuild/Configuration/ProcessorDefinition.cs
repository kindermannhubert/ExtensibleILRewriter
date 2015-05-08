using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

namespace ExtensibleILRewriter.MsBuild.Configuration
{
    public class ProcessorDefinition
    {
        [XmlAttribute("assemblyAlias")]
        public string AssemblyAlias { get; set; }

        [XmlAttribute("name")]
        public string ProcessorName { get; set; }

        [XmlArray]
        [XmlArrayItem("Property")]
        public ProcessorPropertyDefinition[] Properties { get; set; }

        [XmlArray]
        [XmlArrayItem("TypeAlias")]
        public string[] GenericArguments { get; set; }

        public void Check(HashSet<string> definedAssemblyAliases, HashSet<string> definedTypeAliases)
        {
            if (string.IsNullOrWhiteSpace(AssemblyAlias))
            {
                throw new InvalidOperationException($"Configuration of {nameof(ProcessorDefinition)} must contain {nameof(AssemblyAlias)} attribute.");
            }

            if (string.IsNullOrWhiteSpace(ProcessorName))
            {
                throw new InvalidOperationException($"Configuration of {nameof(ProcessorDefinition)} must contain {nameof(ProcessorName)} attribute.");
            }

            if (!definedAssemblyAliases.Contains(AssemblyAlias))
            {
                throw new InvalidOperationException($"Configuration of {nameof(AssemblyRewrite)} task does not contain assembly definition with name '{AssemblyAlias}'.");
            }

            if (Properties == null)
            {
                Properties = new ProcessorPropertyDefinition[0];
            }

            foreach (var property in Properties)
            {
                property.Check();
            }

            if (Properties.Select(p => p.Name).Distinct().Count() != Properties.Length)
            {
                throw new InvalidOperationException($"Configuration of {nameof(ProcessorDefinition)} must contain only distinct property names.");
            }

            if (GenericArguments == null)
            {
                GenericArguments = new string[0];
            }

            foreach (var argument in GenericArguments)
            {
                if (string.IsNullOrWhiteSpace(argument))
                {
                    throw new InvalidOperationException($"Configuration of {nameof(GenericArguments)} element must contain type alias.");
                }
            }
        }
    }
}
