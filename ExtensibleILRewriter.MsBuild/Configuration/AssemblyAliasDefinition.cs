using System;
using System.Xml.Serialization;

namespace ExtensibleILRewriter.MsBuild.Configuration
{
    public class AssemblyAliasDefinition
    {
        [XmlAttribute("alias")]
        public string Alias { get; set; }

        [XmlAttribute("path")]
        public string Path { get; set; }

        public void Check()
        {
            if (string.IsNullOrWhiteSpace(Alias))
            {
                throw new InvalidOperationException($"Configuration of {nameof(AssemblyAliasDefinition)} must contain {nameof(Alias)} attribute.");
            }

            if (string.IsNullOrWhiteSpace(Path))
            {
                throw new InvalidOperationException($"Configuration of {nameof(AssemblyAliasDefinition)} must contain {nameof(Path)} attribute.");
            }
        }
    }
}
