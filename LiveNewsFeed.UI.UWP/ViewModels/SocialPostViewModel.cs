using System;
using System.Web;
using Windows.UI.Xaml.Media;

using LiveNewsFeed.Models;

using LiveNewsFeed.UI.UWP.Common;

namespace LiveNewsFeed.UI.UWP.ViewModels
{
    public class SocialPostViewModel : ViewModelBase
    {
        public SocialPost OriginalPost { get; }

        public SocialPostType PostType => OriginalPost.PostType;

        public Uri Url => OriginalPost.Url;

        public ImageSource? SocialSiteLogo { get; }

        public string? Content { get; }

        public string ToolTip => GetLocalizedString($"SocialPostType_ToolTip_{PostType}");

        public SocialPostViewModel(SocialPost socialPost)
        {
            OriginalPost = socialPost ?? throw new ArgumentNullException(nameof(socialPost));

            Content = SanitizeSocialPostContent(OriginalPost.Content);
            SocialSiteLogo = Helpers.GetLogoForSocialSite(socialPost.PostType);
        }


        private static string? SanitizeSocialPostContent(string? socialPostContent) => socialPostContent != null
            ? $"<!DOCTYPE html>{HttpUtility.HtmlDecode(socialPostContent)}</html>"
            : default;
    }
}