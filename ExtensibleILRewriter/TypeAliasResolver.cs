﻿using Mono.Cecil;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;
using ExtensibleILRewriter.Processors.Parameters;

namespace ExtensibleILRewriter
{
    public class TypeAliasResolver
    {
        private readonly Dictionary<string, Lazy<AssemblyDefinition>> assemblyDefinitions;
        private readonly Dictionary<string, Lazy<Assembly>> assemblies;
        private readonly Dictionary<string, Lazy<TypeDefinition>> typeDefinitions;
        private readonly Dictionary<string, Lazy<Type>> types;

        public TypeAliasResolver(
            [NotNull]Dictionary<string, Lazy<AssemblyDefinition>> assemblyDefinitions,
            [NotNull]Dictionary<string, Lazy<Assembly>> assemblies,
            [NotNull]Dictionary<string, TypeAliasDefinition> typeAliasDefinitions)
        {
            this.assemblyDefinitions = assemblyDefinitions;
            this.assemblies = assemblies;
            this.typeDefinitions = typeAliasDefinitions.ToDictionary(kv => kv.Key, kv => new Lazy<TypeDefinition>(() => LoadTypeDefinition(kv.Value)));
            this.types = typeAliasDefinitions.ToDictionary(kv => kv.Key, kv => new Lazy<Type>(() => LoadType(kv.Value)));
        }

        private TypeDefinition LoadTypeDefinition(TypeAliasDefinition typeAliasDefinition)
        {
            var assemblyDef = assemblyDefinitions[typeAliasDefinition.AssemblyAlias].Value;
            var typeDef = assemblyDef.MainModule.Types.FirstOrDefault(t => t.FullName == typeAliasDefinition.TypeName);
            if (typeDef == null) throw new TypeLoadException("Unable to find type '\{typeAliasDefinition.TypeName}' within the assembly '\{assemblyDef.FullName}' with alias '\{typeAliasDefinition.AssemblyAlias}'.");
            return typeDef;
        }

        private Type LoadType(TypeAliasDefinition typeAliasDefinition)
        {
            var assembly = assemblies[typeAliasDefinition.AssemblyAlias].Value;
            var type = assembly.GetTypes().FirstOrDefault(t => t.FullName == typeAliasDefinition.TypeName);
            if (type == null) throw new TypeLoadException("Unable to find type '\{typeAliasDefinition.TypeName}' within the assembly '\{assembly.FullName}' with alias '\{typeAliasDefinition.AssemblyAlias}'.");
            return type;
        }

        public TypeDefinition ResolveTypeDefinition(string typeAlias)
        {
            return typeDefinitions[typeAlias].Value;
        }

        public Type ResolveType(string typeAlias)
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
