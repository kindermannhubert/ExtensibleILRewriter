using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

namespace ExtensibleILRewriter.MsBuild.Configuration
{
    public class AssemblyRewriteConfiguration
    {
        [XmlArray]
        [XmlArrayItem("Assembly")]
        public AssemblyAliasDefinition[] Assemblies { get; set; }

        [XmlArray]
        [XmlArrayItem("Type")]
        public TypeAliasDefinition[] Types { get; set; }

        [XmlArray]
        [XmlArrayItem("Processor")]
        public ProcessorDefinition[] Processors { get; set; }

        public void Check()
        {
            CheckAssemblies();
            CheckTypes();

            var definedAssemblyAliases = new HashSet<string>(Assemblies.Select(a => a.Alias));
            var definedTypeAliases = new HashSet<string>(Types.Select(t => t.Alias));

            CheckProcessorDefinitions(Processors, nameof(Processors), definedAssemblyAliases, definedTypeAliases);
        }

        private void CheckAssemblies()
        {
            if (Assemblies == null)
            {
                throw new InvalidOperationException($"Configuration of {nameof(AssemblyRewrite)} task must contain {nameof(Assemblies)} element.");
            }

            foreach (var assembly in Assemblies)
            {
                assembly.Check();
            }

            if (Assemblies.Select(a => a.Alias).Distinct().Count() != Assemblies.Length)
            {
                throw new InvalidOperationException($"Configuration of {nameof(AssemblyRewrite)} task must contain only distinct assembly definition names.");
            }
        }

        private void CheckTypes()
        {
            if (Types == null)
            {
                throw new InvalidOperationException($"Configuration of {nameof(AssemblyRewrite)} task must contain {nameof(Types)} element.");
            }

            foreach (var type in Types)
            {
                type.Check();
            }

            if (Types.Select(t => t.Name).Distinct().Count() != Types.Length)
            {
                throw new InvalidOperationException($"Configuration of {nameof(AssemblyRewrite)} task must contain only distinct type definition names.");
            }
        }

        private void CheckProcessorDefinitions(ProcessorDefinition[] processors, string elementName, HashSet<string> definedAssemblyAliases, HashSet<string> definedTypeAliases)
        {
            if (processors == null)
            {
                throw new InvalidOperationException($"Configuration of {nameof(AssemblyRewrite)} task must contain {elementName} element.");
            }

            foreach (var processor in processors)
            {
                processor.Check(definedAssemblyAliases, definedTypeAliases);
            }
        }
    }
}
