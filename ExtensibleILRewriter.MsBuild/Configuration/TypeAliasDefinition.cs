using System;
using System.Xml.Serialization;

namespace ExtensibleILRewriter.MsBuild.Configuration
{
    public class TypeAliasDefinition
    {
        [XmlAttribute("assemblyAlias")]
        public string AssemblyAlias { get; set; }

        [XmlAttribute("alias")]
        public string Alias { get; set; }

        [XmlAttribute("name")]
        public string Name { get; set; }

        public void Check()
        {
            if (string.IsNullOrWhiteSpace(AssemblyAlias))
            {
                throw new InvalidOperationException($"Configuration of {nameof(TypeAliasDefinition)} must contain {nameof(AssemblyAlias)} attribute.");
            }

            if (string.IsNullOrWhiteSpace(Alias))
            {
                throw new InvalidOperationException($"Configuration of {nameof(TypeAliasDefinition)} must contain {nameof(Alias)} attribute.");
            }

            if (string.IsNullOrWhiteSpace(Name))
            {
                throw new InvalidOperationException($"Configuration of {nameof(TypeAliasDefinition)} must contain {nameof(Name)} attribute.");
            }
        }
    }
}
