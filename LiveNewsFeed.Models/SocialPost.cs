using System;

namespace LiveNewsFeed.Models
{
    public class SocialPost
    {
        public Uri Url { get; }

        public string? Content { get; }
        
        public SocialPost(Uri url, string? content)
        {
            Url = url ?? throw new ArgumentNullException(nameof(url));
            Content = content;
        }
    }
}