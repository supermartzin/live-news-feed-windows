using System;

using LiveNewsFeed.Models;

namespace LiveNewsFeed.UI.UWP.ViewModels
{
    public class TagViewModel : ViewModelBase
    {
        public Tag OriginalTag { get; }

        public string Name => OriginalTag.Name;

        public TagViewModel(Tag tag)
        {
            OriginalTag = tag ?? throw new ArgumentNullException(nameof(tag));
        }
    }
}