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
    public class ProcessorPropertyDefinition
    {
        public string Name { get; set; }
        public string Value { get; set; }

        public void Check([NotNull] ILogger logger)
        {
            if (string.IsNullOrWhiteSpace(Name))
            {
                var message = "Configuration of \{nameof(ProcessorPropertyDefinition)} must contain \{nameof(Name)} element.";
                logger.Error(message);
                throw new InvalidOperationException(message);
            }

            if (string.IsNullOrWhiteSpace(Value))
            {
                var message = "Configuration of \{nameof(ProcessorPropertyDefinition)} must contain \{nameof(Value)} element.";
                logger.Error(message);
                throw new InvalidOperationException(message);
            }
        }
    }
}
