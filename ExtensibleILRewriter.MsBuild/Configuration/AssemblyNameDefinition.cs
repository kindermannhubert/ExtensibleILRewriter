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
    public class AssemblyNameDefinition
    {
        public string Name { get; set; }
        public string Path { get; set; }

        public void Check()
        {
            if (string.IsNullOrWhiteSpace(Name))
            {
                throw new InvalidOperationException("Configuration of \{nameof(AssemblyNameDefinition)} must contain \{nameof(Name)} element.");
            }

            if (string.IsNullOrWhiteSpace(Path))
            {
                throw new InvalidOperationException("Configuration of \{nameof(AssemblyNameDefinition)} must contain \{nameof(Path)} element.");
            }
        }
    }
}
