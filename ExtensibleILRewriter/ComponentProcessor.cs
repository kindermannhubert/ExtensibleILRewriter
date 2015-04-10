﻿using ExtensibleILRewriter.MethodProcessors.Contracts;
using Mono.Cecil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExtensibleILRewriter
{
    public abstract class ComponentProcessor<ComponentType>
    {
        protected readonly ComponentProcessorProperties properties;
        protected readonly ILogger logger;

        public ComponentProcessor([NotNull]ComponentProcessorProperties properties, [NotNull]ILogger logger)
        {
            this.properties = properties;
            this.logger = logger;
        }

        public abstract void Process([NotNull]ComponentType component);

        protected void CheckIfContainsProperty([NotNull]ComponentProcessorProperties properties, string propertyName)
        {
            if (!properties.ContainsProperty(propertyName))
            {
                throw new InvalidOperationException("\{GetType().FullName} processor needs '\{propertyName}' element in configuration specified.");
            }
        }
    }
}