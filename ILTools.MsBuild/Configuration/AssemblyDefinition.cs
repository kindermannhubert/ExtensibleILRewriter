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
    public class AssemblyDefinition
    {
        public string Name { get; set; }
        public string Path { get; set; }

        public void Check()
        {
            if (string.IsNullOrWhiteSpace(Name))
            {
                throw new InvalidOperationException("Configuration of \{nameof(AssemblyDefinition)} must contain \{nameof(Name)} element.");
            }

            if (string.IsNullOrWhiteSpace(Path))
            {
                throw new InvalidOperationException("Configuration of \{nameof(AssemblyDefinition)} must contain \{nameof(Path)} element.");
            }
        }
    }
}
