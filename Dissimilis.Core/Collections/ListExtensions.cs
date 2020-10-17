using System.Collections.Generic;

namespace Dissimilis.Core.Collections
{
    public static class ListExtensions
    {
        public static void AddIfNotExists<T>(this IList<T> list, T item)
        {
            if (!list.Contains(item))
            {
                list.Add(item);
            }
        }

        public static void AddRangeIfNotExists<T>(this IList<T> list, IEnumerable<T> items)
        {
            foreach (var item in items)
            {
                list.AddIfNotExists(item);
            }
        }
    }
}
