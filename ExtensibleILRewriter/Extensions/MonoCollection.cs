using ExtensibleILRewriter.MethodProcessors.Contracts;
using Mono.Collections.Generic;
using System.Collections.Generic;

namespace ExtensibleILRewriter.Extensions
{
    static class MonoCollectionExtensions
    {
        public static void AddRange<T>([NotNull]this Collection<T> collection, [NotNull]IEnumerable<T> values)
        {
            foreach (var value in values) collection.Add(value);
        }
    }
}
