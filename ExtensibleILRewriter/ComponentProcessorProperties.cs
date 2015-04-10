using ExtensibleILRewriter.MethodProcessors.Contracts;
using Mono.Cecil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExtensibleILRewriter
{
    public class ComponentProcessorProperties
    {
        private readonly Dictionary<string, string> properties = new Dictionary<string, string>();

        public ComponentProcessorProperties(IEnumerable<Tuple<string, string>> properties)
        {
            this.properties = properties.ToDictionary(p => p.Item1, p => p.Item2);
        }

        public string GetProperty(string name)
        {
            return properties[name];
        }

        public bool ContainsProperty(string name)
        {
            return properties.ContainsKey(name);
        }
    }
}
