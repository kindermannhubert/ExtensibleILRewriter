using System;

namespace ExtensibleILRewriter.MsBuild.Configuration
{
    public class AssemblyAliasDefinition
    {
        public string Alias { get; set; }
        public string Path { get; set; }

        public void Check()
        {
            if (string.IsNullOrWhiteSpace(Alias))
            {
                throw new InvalidOperationException("Configuration of \{nameof(AssemblyAliasDefinition)} must contain \{nameof(Alias)} element.");
            }

            if (string.IsNullOrWhiteSpace(Path))
            {
                throw new InvalidOperationException("Configuration of \{nameof(AssemblyAliasDefinition)} must contain \{nameof(Path)} element.");
            }
        }
    }
}
