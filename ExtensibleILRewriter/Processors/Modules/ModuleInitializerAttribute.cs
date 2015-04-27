﻿using System;

namespace ExtensibleILRewriter.Processors.Modules
{
    [AttributeUsage(AttributeTargets.Module, AllowMultiple = true, Inherited = false)]
    public class ModuleInitializerAttribute : Attribute
    {
        public string TypeName { get; }
        public string MethodName { get; }

        public ModuleInitializerAttribute(string typeName, string methodName)
        {
            TypeName = typeName;
            MethodName = methodName;
        }
    }
}
