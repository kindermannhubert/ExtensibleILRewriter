using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ILTools.MethodProcessors.Helpers
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
