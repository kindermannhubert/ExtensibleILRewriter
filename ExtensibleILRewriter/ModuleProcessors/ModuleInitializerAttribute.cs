using System;

namespace ExtensibleILRewriter.ModuleProcessors
{
    [AttributeUsage(AttributeTargets.Module | AttributeTargets.Assembly, AllowMultiple = false, Inherited = false)]
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
