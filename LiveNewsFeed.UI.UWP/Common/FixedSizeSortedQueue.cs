using System;
using System.Collections.Concurrent;
using System.Linq;

namespace LiveNewsFeed.UI.UWP.Common
{
    public class FixedSizeSortedQueue<T> : ConcurrentQueue<T> where T : IComparable<T>
    {
        private readonly object _lock = new();

        public int Size { get; }

        public FixedSizeSortedQueue(int size)
        {
            if (size <= 0)
                throw new ArgumentOutOfRangeException(nameof(size));
            
            Size = size;
        }
        
        public new void Enqueue(T item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));

            base.Enqueue(item);
            lock (_lock)
            {
                while (Count > Size)
                {
                    TryDequeue(out _);
                }

                // sort queue
                var items = ToArray().ToList();
                items.Sort((firstItem, secondItem) => firstItem.CompareTo(secondItem));

                Clear();
                items.ForEach(base.Enqueue);
            }
        }

        public virtual bool IsFull() => Count == Size;
    }
}