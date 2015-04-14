using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

namespace ExtensibleILRewriter.MsBuild.Configuration
{
    public class ProcessorDefinition
    {
        public string AssemblyAlias { get; set; }

        public string ProcessorName { get; set; }

        [XmlArray]
        [XmlArrayItem("Property")]
        public ProcessorPropertyDefinition[] Properties { get; set; }

        public void Check(HashSet<string> definedAssemblyAliases, HashSet<string> definedTypeAliases)
        {
            if (string.IsNullOrWhiteSpace(AssemblyAlias))
            {
                throw new InvalidOperationException("Configuration of \{nameof(ProcessorDefinition)} must contain \{nameof(AssemblyAlias)} element.");
            }

            if (string.IsNullOrWhiteSpace(ProcessorName))
            {
                throw new InvalidOperationException("Configuration of \{nameof(ProcessorDefinition)} must contain \{nameof(ProcessorName)} element.");
            }

            if (!definedAssemblyAliases.Contains(AssemblyAlias))
            {
                throw new InvalidOperationException("Configuration of \{nameof(AssemblyRewrite)} task does not contain assembly definition with name '\{AssemblyAlias}'.");
            }

            if (Properties == null) Properties = new ProcessorPropertyDefinition[0];

            if (Properties.Select(p => p.Name).Distinct().Count() != Properties.Length)
            {
                throw new InvalidOperationException("Configuration of \{nameof(ProcessorDefinition)} must contain only distinct property names.");
            }
        }
    }
}
