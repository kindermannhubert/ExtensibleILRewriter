using ExtensibleILRewriter.Extensions;
using ExtensibleILRewriter.MethodProcessors;
using ExtensibleILRewriter.MethodProcessors.Contracts;
using ExtensibleILRewriter.MethodProcessors.Helpers;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace ExtensibleILRewriter.MsBuild.Configuration
{
    public class AssemblyRewriteConfiguration
    {
        [XmlArray]
        [XmlArrayItem("AssemblyDefinition")]
        public AssemblyNameDefinition[] AssembliesWithProcessors { get; set; }

        [XmlArray]
        [XmlArrayItem("Processor")]
        public ProcessorDefinition[] AssemblyProcessors { get; set; }

        [XmlArray]
        [XmlArrayItem("Processor")]
        public ProcessorDefinition[] ModuleProcessors { get; set; }

        [XmlArray]
        [XmlArrayItem("Processor")]
        public ProcessorDefinition[] TypeProcessors { get; set; }

        [XmlArray]
        [XmlArrayItem("Processor")]
        public ProcessorDefinition[] MethodProcessors { get; set; }

        public void Check()
        {
            if (AssembliesWithProcessors == null)
            {
                throw new InvalidOperationException("Configuration of \{nameof(AssemblyRewrite)} task must contain \{nameof(AssembliesWithProcessors)} element.");
            }
            foreach (var assembly in AssembliesWithProcessors) assembly.Check();

            if (AssembliesWithProcessors.Select(a => a.Name).Distinct().Count() != AssembliesWithProcessors.Length)
            {
                throw new InvalidOperationException("Configuration of \{nameof(AssemblyRewrite)} task must contain only distinct assembly definition names.");
            }

            var definedAssemblyNames = new HashSet<string>(AssembliesWithProcessors.Select(a => a.Name));

            CheckProcessorDefinitions(AssemblyProcessors, nameof(AssemblyProcessors), definedAssemblyNames);
            CheckProcessorDefinitions(ModuleProcessors, nameof(ModuleProcessors), definedAssemblyNames);
            CheckProcessorDefinitions(TypeProcessors, nameof(TypeProcessors), definedAssemblyNames);
            CheckProcessorDefinitions(MethodProcessors, nameof(MethodProcessors), definedAssemblyNames);
        }

        private void CheckProcessorDefinitions(ProcessorDefinition[] processors, string elementName, HashSet<string> definedAssemblyNames)
        {
            if (processors == null)
            {
                throw new InvalidOperationException("Configuration of \{nameof(AssemblyRewrite)} task must contain \{elementName} element.");
            }
            foreach (var processor in processors) processor.Check(definedAssemblyNames);
        }
    }
}
