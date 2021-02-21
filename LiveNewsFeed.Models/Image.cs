using System;

namespace LiveNewsFeed.Models
{
    public class Image
    {
        public string? Title { get; }

        public Uri Url { get; }

        public Uri? LargeSizeUrl { get; }

        public Image(Uri url, string? title, Uri? largeSizeUrl)
        {
            Url = url ?? throw new ArgumentNullException(nameof(url));
            Title = title;
            LargeSizeUrl = largeSizeUrl;
        }
    }
}