using System;
using System.Collections.Generic;

namespace Dissimilis.Core.Collections
{
    public static class CollectionExtensions
    {
        public static void AddRange<T>(this ICollection<T> collection, IEnumerable<T> values)
        {
            foreach (var value in values)
            {
                collection.Add(value);
            }
        }

        public static bool TryFindByIndex<T>(this IEnumerable<T> collection, int i, out T item)
        {
            using (var enumerator = collection.GetEnumerator())
            {
                var counter = 0;

                while (enumerator.MoveNext())
                {
                    if (counter == i)
                    {
                        item = enumerator.Current;
                        return true;
                    }

                    counter += 1;
                }
            }

            item = default;
            return false;
        }

        public static T FindByIndex<T>(this IEnumerable<T> collection, int i)
        {
            if (collection.TryFindByIndex(i, out T item))
            {
                return item;
            }

            throw new IndexOutOfRangeException();
        }
    }
}
