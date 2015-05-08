using System;

namespace ExtensibleILRewriter.Processors.Modules
{
    [AttributeUsage(AttributeTargets.Module, AllowMultiple = true, Inherited = false)]
    public class ModuleInitializerAttribute : Attribute
    {
        public ModuleInitializerAttribute(string typeName, string methodName)
        {
            TypeName = typeName;
            MethodName = methodName;
        }

        public string TypeName { get; }

        public string MethodName { get; }
    }
}
