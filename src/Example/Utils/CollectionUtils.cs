using System;
using System.Collections.Generic;
using System.Linq;

namespace Example.Utils
{
    public static class CollectionUtils
    {
        public static int FindIndex<T>(this IEnumerable<T> collection, Func<T, bool> predicate)
        {
            return collection
                       .Select((value, index) => new { Value = value, Index = index })
                       .FirstOrDefault(pair => predicate(pair.Value))
                       ?.Index ?? -1;
        }

        public static bool CollectionEquals<T>(this IEnumerable<T> collection1, IEnumerable<T> collection2, IEqualityComparer<T> comparer)
        {
            return collection1 == null 
                ? collection2 == null 
                : collection2 != null && collection1.SequenceEqual(collection2, comparer);
        }
    }
}