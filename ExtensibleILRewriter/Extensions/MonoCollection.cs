using ExtensibleILRewriter.Processors.Parameters;
using Mono.Collections.Generic;
using System.Collections.Generic;

namespace ExtensibleILRewriter.Extensions
{
    internal static class MonoCollectionExtensions
    {
        public static void AddRange<T>([NotNull]this Collection<T> collection, [NotNull]IEnumerable<T> values)
        {
            foreach (var value in values)
            {
                collection.Add(value);
            }
        }
    }
}
