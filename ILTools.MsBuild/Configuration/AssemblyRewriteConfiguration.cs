using ILTools.Extensions;
using ILTools.MethodProcessors;
using ILTools.MethodProcessors.Contracts;
using ILTools.MethodProcessors.Helpers;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace ILTools.MsBuild.Configuration
{
    public class AssemblyRewriteConfiguration
    {
        [XmlArray]
        [XmlArrayItem("AssemblyDefinition")]
        public AssemblyDefinition[] AssembliesWithProcessors { get; set; }

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

            if (MethodProcessors == null)
            {
                throw new InvalidOperationException("Configuration of \{nameof(AssemblyRewrite)} task must contain \{nameof(MethodProcessors)} element.");
            }
            foreach (var methodProcessor in MethodProcessors) methodProcessor.Check(definedAssemblyNames);
        }
    }
}
