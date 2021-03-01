using System;
using System.Web;

using LiveNewsFeed.Models;

namespace LiveNewsFeed.UI.UWP.ViewModels
{
    public class SocialPostViewModel : ViewModelBase
    {
        public SocialPost OriginalPost { get; }

        public SocialPostType PostType => OriginalPost.PostType;

        public Uri Url => OriginalPost.Url;

        public string? Content { get; }

        public SocialPostViewModel(SocialPost socialPost)
        {
            OriginalPost = socialPost ?? throw new ArgumentNullException(nameof(socialPost));

            Content = SanitizeSocialPostContent(OriginalPost.Content);
        }


        private static string? SanitizeSocialPostContent(string? socialPostContent) => socialPostContent != null
            ? $"<!DOCTYPE html>{HttpUtility.HtmlDecode(socialPostContent)}</html>"
            : default;
    }
}