using System;

namespace ExtensibleILRewriter.MsBuild.Configuration
{
    public class TypeAliasDefinition
    {
        public string AssemblyAlias { get; set; }
        public string Alias { get; set; }
        public string Name { get; set; }

        public void Check()
        {
            if (string.IsNullOrWhiteSpace(AssemblyAlias))
            {
                throw new InvalidOperationException("Configuration of \{nameof(TypeAliasDefinition)} must contain \{nameof(AssemblyAlias)} element.");
            }

            if (string.IsNullOrWhiteSpace(Alias))
            {
                throw new InvalidOperationException("Configuration of \{nameof(TypeAliasDefinition)} must contain \{nameof(Alias)} element.");
            }

            if (string.IsNullOrWhiteSpace(Name))
            {
                throw new InvalidOperationException("Configuration of \{nameof(TypeAliasDefinition)} must contain \{nameof(Name)} element.");
            }
        }
    }
}
