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

        public void Check([NotNull] ILogger logger, HashSet<string> definedAssemblyNames)
        {
            if (string.IsNullOrWhiteSpace(AssemblyName))
            {
                var message = "Configuration of \{nameof(ProcessorDefinition)} must contain \{nameof(AssemblyName)} element.";
                logger.Error(message);
                throw new InvalidOperationException(message);
            }

            if (string.IsNullOrWhiteSpace(ProcessorName))
            {
                var message = "Configuration of \{nameof(ProcessorDefinition)} must contain \{nameof(ProcessorName)} element.";
                logger.Error(message);
                throw new InvalidOperationException(message);
            }

            if (!definedAssemblyNames.Contains(AssemblyName))
            {
                var message = "Configuration of \{nameof(AssemblyRewrite)} task does not contain assembly definition with name '\{AssemblyName}'.";
                logger.Error(message);
                throw new InvalidOperationException(message);
            }
        }
    }
}
