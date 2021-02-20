using System;

namespace LiveNewsFeed.Models
{
    public class Image
    {
        public string Title { get; }

        public Uri Url { get; }

        public Uri? LargeSizeUrl { get; }

        public Image(string title, Uri url, Uri? largeSizeUrl)
        {
            Title = title;
            Url = url ?? throw new ArgumentNullException(nameof(url));
            LargeSizeUrl = largeSizeUrl;
        }
    }
}