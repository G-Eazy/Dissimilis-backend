using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace FoodLabellingAPI.Collections
{
    public static class QueueExtensions
    {
        [DebuggerStepThrough]
        public static Queue<T> ToQueue<T>(this IEnumerable<T> list)
        {
            return list != null
                ? new Queue<T>(list)
                : new Queue<T>();
        }


        [DebuggerStepThrough]
        public static T[] DequeueRange<T>(this Queue<T> queue, int count)
        {
            lock (queue)
            {
                if (count > queue.Count)
                {
                    count = queue.Count;
                }

                IEnumerable<T> InnerDequeue()
                {
                    for (var i = 0; i < count; i++)
                    {
                        yield return queue.Dequeue();
                    }
                }
                return InnerDequeue().ToArray();
            }

        }

        [DebuggerStepThrough]
        public static Queue<T> EnqueRange<T>(this Queue<T> queue, IEnumerable<T> list)
        {
            lock (queue)
            {
                foreach (var item in list)
                {
                    queue.Enqueue(item);
                }
            }

            return queue;
        }

        [DebuggerStepThrough]
        public static bool TryDequeueThreadSafe<T>(this Queue<T> queue, out T item) where T : class
        {
            lock (queue)
            {
                if (!queue.Any())
                {
                    item = null;
                    return false;
                }

                item = queue.Dequeue();
            }

            return item != null;
        }

        [DebuggerStepThrough]
        public static bool TryDequeueRangeThreadSafe<T>(this Queue<T> queue, out T[] items, int numberOfItems) where T : class
        {
            lock (queue)
            {
                if (!queue.Any())
                {
                    items = null;
                    return false;
                }

                items = queue.DequeueRange(numberOfItems);
            }

            return items.Any();
        }

        [DebuggerStepThrough]
        public static bool TryDequeueThreadSafe(this Queue<int> queue, out int? item)
        {
            lock (queue)
            {
                if (!queue.Any())
                {
                    item = null;
                    return false;
                }

                item = queue.Dequeue();
            }

            return item != null;
        }
    }
}
