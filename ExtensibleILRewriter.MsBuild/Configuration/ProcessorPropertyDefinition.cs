﻿using ExtensibleILRewriter.Extensions;
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
    public class ProcessorPropertyDefinition
    {
        public string Name { get; set; }
        public string Value { get; set; }

        public void Check()
        {
            if (string.IsNullOrWhiteSpace(Name))
            {
                throw new InvalidOperationException("Configuration of \{nameof(ProcessorPropertyDefinition)} must contain \{nameof(Name)} element.");
            }

            if (string.IsNullOrWhiteSpace(Value))
            {
                throw new InvalidOperationException("Configuration of \{nameof(ProcessorPropertyDefinition)} must contain \{nameof(Value)} element.");
            }
        }
    }
}