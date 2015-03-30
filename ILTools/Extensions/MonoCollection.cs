using Mono.Collections.Generic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ILTools.Extensions
{
    public static class MonoCollectionExtensions
    {
        public static void AddRange<T>([NotNull]this Collection<T> collection, [NotNull]IEnumerable<T> values)
        {
            foreach (var value in values) collection.Add(value);
        }
    }
}
