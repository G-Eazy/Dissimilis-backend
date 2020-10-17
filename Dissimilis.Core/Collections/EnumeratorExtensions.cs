using System.Collections.Generic;
using System.Linq;

namespace Dissimilis.Core.Collections
{
    public static class EnumeratorExtensions
    {
        public static T[] ToArray<T>(this IEnumerator<T> enumerator) =>
            enumerator.Enumerate().ToArray();

        public static IEnumerable<T> Enumerate<T>(this IEnumerator<T> enumerator)
        {
            if (enumerator == null)
                yield break;

            while (enumerator.MoveNext())
            {
                yield return enumerator.Current;
            }
        }
    }
}
