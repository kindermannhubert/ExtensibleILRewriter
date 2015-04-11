﻿using System;

namespace ExtensibleILRewriter.MethodProcessors.Helpers
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public sealed class MakeStaticVersionAttribute : Attribute
    {
        public string NewMethodName { get; }

        public MakeStaticVersionAttribute(string newName)
        {
            NewMethodName = newName;
        }
    }
}
