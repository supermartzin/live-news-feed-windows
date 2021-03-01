using System;

namespace LiveNewsFeed.Models
{
    public class SocialPost
    {
        public SocialPostType PostType { get; }

        public Uri Url { get; }

        public string? Content { get; }
        
        public SocialPost(Uri url, string? content)
        {
            Url = url ?? throw new ArgumentNullException(nameof(url));
            Content = content;
            PostType = DecidePostType(url);
        }

        private static SocialPostType DecidePostType(Uri socialPostUrl)
        {
            var url = socialPostUrl.AbsoluteUri.ToLowerInvariant();

            if (url.Contains("facebook.com") || url.Contains("fb.me"))
                return SocialPostType.Facebook;
            if (url.Contains("instagram.com"))
                return SocialPostType.Instagram;
            if (url.Contains("twitter.com"))
                return SocialPostType.Twitter;
            if (url.Contains("youtube.com") || url.Contains("youtu.be"))
                return SocialPostType.YouTube;
            if (url.Contains("spotify.com"))
                return SocialPostType.Spotify;
            if (url.Contains("deezer.com"))
                return SocialPostType.Deezer;
            if (url.Contains("music.apple.com"))
                return SocialPostType.AppleMusic;

            return SocialPostType.Other;
        }
    }
}