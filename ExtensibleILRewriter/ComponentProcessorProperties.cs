using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace ExtensibleILRewriter
{
    public class ComponentProcessorProperties : IEnumerable<KeyValuePair<string, string>>
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

        public bool TryGetProperty(string name, out string value)
        {
            return properties.TryGetValue(name, out value);
        }

        public IEnumerator<KeyValuePair<string, string>> GetEnumerator()
        {
            return properties.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
