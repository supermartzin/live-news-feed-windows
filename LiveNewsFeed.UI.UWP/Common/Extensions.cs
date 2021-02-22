using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace LiveNewsFeed.UI.UWP.Common
{
    public static class Extensions
    {
        public static void SortDescending<TSource, TKey>(this ObservableCollection<TSource> source, Func<TSource, TKey> keySelector)
        {
            var sortedSource = source.OrderByDescending(keySelector).ToList();

            for (var index = 0; index < sortedSource.Count; index++)
            {
                var itemToSort = sortedSource[index];

                var oldIndex = source.IndexOf(itemToSort);
                
                // If the item is already at the right position, leave it and continue.
                if (oldIndex == index)
                    continue;

                // move item to correct index
                source.Move(oldIndex, index);
            }
        }
    }
}