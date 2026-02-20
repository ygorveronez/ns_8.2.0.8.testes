using System.Collections;
using System.Collections.Generic;

namespace System
{
    public static class ListExtensions
    {
        //#if NET8_0
        public static IEnumerable<TSource> DistinctBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
        {
            HashSet<TKey> seenKeys = new HashSet<TKey>();

            foreach (TSource element in source)
            {
                if (seenKeys.Add(keySelector(element)))
                    yield return element;
            }
        }
        //#endif

        public static bool IsNullOrEmpty(this IEnumerable source)
        {
            return source == null || ((ICollection)source).Count == 0;
        }
    }
}
