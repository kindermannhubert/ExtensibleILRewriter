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
    public class ProcessorDefinition
    {
        public string AssemblyName { get; set; }

        public string ProcessorName { get; set; }

        [XmlArray]
        [XmlArrayItem("Property")]
        public ProcessorPropertyDefinition[] Properties { get; set; }

        public void Check(HashSet<string> definedAssemblyNames)
        {
            if (string.IsNullOrWhiteSpace(AssemblyName))
            {
                throw new InvalidOperationException("Configuration of \{nameof(ProcessorDefinition)} must contain \{nameof(AssemblyName)} element.");
            }

            if (string.IsNullOrWhiteSpace(ProcessorName))
            {
                throw new InvalidOperationException("Configuration of \{nameof(ProcessorDefinition)} must contain \{nameof(ProcessorName)} element.");
            }

            if (!definedAssemblyNames.Contains(AssemblyName))
            {
                throw new InvalidOperationException("Configuration of \{nameof(AssemblyRewrite)} task does not contain assembly definition with name '\{AssemblyName}'.");
            }

            if (Properties == null) Properties = new ProcessorPropertyDefinition[0];

            if (Properties.Select(p => p.Name).Distinct().Count() != Properties.Length)
            {
                throw new InvalidOperationException("Configuration of \{nameof(ProcessorDefinition)} must contain only distinct property names.");
            }
        }
    }
}
