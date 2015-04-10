using ExtensibleILRewriter.MethodProcessors.Contracts;
using Mono.Cecil;
using System;
using System.Linq;
using System.Collections.Generic;

namespace ExtensibleILRewriter
{
    public class TypeAliasResolver
    {
        private readonly Dictionary<string, Lazy<AssemblyDefinition>> assemblies;
        private readonly Dictionary<string, Lazy<TypeDefinition>> types;

        public TypeAliasResolver([NotNull]Dictionary<string, Lazy<AssemblyDefinition>> assemblies, [NotNull]Dictionary<string, TypeAliasDefinition> types)
        {
            this.assemblies = assemblies;
            this.types = types.ToDictionary(kv => kv.Key, kv => new Lazy<TypeDefinition>(() => LoadTypeDefinition(kv.Value)));
        }

        private TypeDefinition LoadTypeDefinition(TypeAliasDefinition typeAliasDefinition)
        {
            var assembly = assemblies[typeAliasDefinition.AssemblyAlias].Value;
            var typedef = assembly.MainModule.Types.FirstOrDefault(t => t.FullName == typeAliasDefinition.TypeName);
            if (typedef == null) throw new TypeLoadException("Unable to find type '\{typeAliasDefinition.TypeName}' with in the assembly '\{assembly.FullName}' with alias '\{typeAliasDefinition.AssemblyAlias}'.");
            return typedef;
        }

        public TypeDefinition Resolve(string typeAlias)
        {
            return types[typeAlias].Value;
        }

        public struct TypeAliasDefinition
        {
            public string AssemblyAlias { get; }
            public string TypeName { get; }

            public TypeAliasDefinition(string assemblyAlias, string typeName)
            {
                AssemblyAlias = assemblyAlias;
                TypeName = typeName;
            }
        }
    }
}
