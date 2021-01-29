using System;

namespace LiveNewsFeed.Models
{
    public class Image
    {
        public string Title { get; }

        public Uri Url { get; }

        public Image(string title, Uri url)
        {
            Title = title;
            Url = url ?? throw new ArgumentNullException(nameof(url));
        }
    }
}