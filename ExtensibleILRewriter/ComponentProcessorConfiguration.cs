﻿using ExtensibleILRewriter.MethodProcessors.Contracts;
using System;
using System.Collections.Generic;

namespace ExtensibleILRewriter
{
    public abstract class ComponentProcessorConfiguration
    {
        public abstract void LoadFromProperties(ComponentProcessorProperties properties);

        protected void CheckIfContainsProperty([NotNull]ComponentProcessorProperties properties, string property)
        {
            if (!properties.ContainsProperty(property))
            {
                throw new InvalidOperationException("\{GetType().FullName} processor configuration needs '\{property}' element in configuration specified.");
            }
        }

        public class EmptyConfiguration : ComponentProcessorConfiguration
        {
            public override void LoadFromProperties(ComponentProcessorProperties properties)
            {
            }
        }
    }
}
