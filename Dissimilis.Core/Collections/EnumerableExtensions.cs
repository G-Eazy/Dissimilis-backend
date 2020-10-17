using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Dissimilis.Core.Collections
{
    public static class EnumerableExtensions
    {
        [DebuggerStepThrough]
        public static void ForEach<T>(this IEnumerable<T> enumerable, Action<T> action)
        {
            foreach (var e in enumerable)
            {
                action(e);
            }
        }

        [DebuggerStepThrough]
        public static bool TryFind<T>(this IEnumerable<T> enumerable, Func<T, bool> predicate, out T result) where T : class
        {
            result = null;
            if (enumerable == null)
            {
                return false;
            }

            result = enumerable.FirstOrDefault(predicate);

            return result != null;
        }

        [DebuggerStepThrough]
        public static bool TrySingle<T>(this IEnumerable<T> enumerable, Func<T, bool> predicate, out T result) where T : class
        {
            result = null;
            if (enumerable == null)
            {
                return false;
            }

            result = enumerable.SingleOrDefault(predicate);

            return result != null;
        }

        [DebuggerStepThrough]
        public static T[][] Batch<T>(this IEnumerable<T> items, int maxItems) =>
            items.Select((item, inx) => new { item, inx })
                .GroupBy(x => x.inx / maxItems)
                .Select(g => g.Select(x => x.item).ToArray()).ToArray();


        [DebuggerStepThrough]
        public static IEnumerable<T> DistinctBy<T, TKey>(this IEnumerable<T> source, Func<T, TKey> selector)
        {
            return source.GroupBy(selector)
                .Select(g => g.First());
        }

        [DebuggerStepThrough]
        public static HashSet<T> ToHashSet<T>(this IEnumerable<T> values)
        {
            var hash = new HashSet<T>();
            foreach (var value in values)
            {
                hash.Add(value);
            }

            return hash;
        }

        [DebuggerStepThrough]
        public static bool AnyAndNotNull<T>(this IEnumerable<T> values, Func<T, bool> predicate = null) =>
            values != null && 
            (predicate == null
                ? values.Any()
                : values.Any(predicate));
    }
}