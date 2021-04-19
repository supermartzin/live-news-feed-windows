using System;
using System.Collections.Generic;
using Windows.Data.Html;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Microsoft.Toolkit.Uwp.UI;

using LiveNewsFeed.Models;

using LiveNewsFeed.UI.UWP.ViewModels;

namespace LiveNewsFeed.UI.UWP.Common
{
    internal static class Helpers
    {
        private static readonly FontFamily SegoeUiSymbolFontFamily = new ("Segoe UI Symbol");
        private static readonly FontFamily SegoeMdl2AssetsFontFamily = new ("Segoe MDL2 Assets");

        private static readonly Dictionary<string, ImageSource> NewsFeedLogos = new();
        private static readonly Dictionary<SocialPostType, ImageSource> SocialSiteLogos = CreateSocialSiteLogos();
        private static readonly Dictionary<Category, CategoryViewModel> CategoriesMap = new();
        
        public static void CreateCategoriesMap()
        {
            CategoriesMap[Category.Local] = new CategoryViewModel(Category.Local, "CategoryLocalColor", "\uE10F", SegoeMdl2AssetsFontFamily);
            CategoriesMap[Category.World] = new CategoryViewModel(Category.World, "CategoryWorldColor", "\uE128", SegoeMdl2AssetsFontFamily);
            CategoriesMap[Category.Economy] = new CategoryViewModel(Category.Economy, "CategoryEconomyColor", "\U0001F4B2", SegoeUiSymbolFontFamily);
            CategoriesMap[Category.Sport] = new CategoryViewModel(Category.Sport, "CategorySportColor", "\u26BD", SegoeUiSymbolFontFamily);
            CategoriesMap[Category.Arts] = new CategoryViewModel(Category.Arts, "CategoryArtsColor", "\U0001F3A8", SegoeUiSymbolFontFamily);
            CategoriesMap[Category.Science] = new CategoryViewModel(Category.Science, "CategoryScienceColor", "\U0001F4A1", SegoeUiSymbolFontFamily);
            CategoriesMap[Category.Commentary] = new CategoryViewModel(Category.Commentary, "CategoryCommentaryColor", "\uE206", SegoeMdl2AssetsFontFamily);
        }

        public static ImageSource? GetLogoForNewsFeed(string newsFeedName)
        {
            if (newsFeedName == null)
                throw new ArgumentNullException(nameof(newsFeedName));

            return NewsFeedLogos.ContainsKey(newsFeedName) ? NewsFeedLogos[newsFeedName] : default;
        }

        public static ImageSource? GetLogoForSocialSite(SocialPostType postType) =>
            SocialSiteLogos.ContainsKey(postType) ? SocialSiteLogos[postType] : default;

        public static void RegisterLogoForNewsFeed(string newsFeedName, Uri logoPath)
        {
            if (newsFeedName == null)
                throw new ArgumentNullException(nameof(newsFeedName));
            if (logoPath == null)
                throw new ArgumentNullException(nameof(logoPath));

            NewsFeedLogos[newsFeedName] = new BitmapImage(logoPath);

            // cache logo in memory
            ImageCache.Instance.PreCacheAsync(logoPath, false, true);
        }

        public static CategoryViewModel GetCategoryViewModel(Category category) => CategoriesMap[category];

        public static string SanitizeHtmlContent(string htmlContent)
        {
            if (htmlContent == null)
                throw new ArgumentNullException(nameof(htmlContent));
            
            // replace bullet list occurrences
            var content = htmlContent.Replace("<li>", "<li> • ");

            // replace newlines
            content = content.Replace("\n", Environment.NewLine);

            // remove HTML tags
            content = HtmlUtilities.ConvertToText(content);

            return content.Trim();
        }


        private static Dictionary<SocialPostType, ImageSource> CreateSocialSiteLogos()
        {
            var logos = new Dictionary<SocialPostType, ImageSource>
            {
                { SocialPostType.Facebook, new BitmapImage(new Uri("ms-appx:///Assets/Logos/facebook-logo.png")) },
                { SocialPostType.Twitter, new BitmapImage(new Uri("ms-appx:///Assets/Logos/twitter-logo.png")) },
                { SocialPostType.YouTube, new BitmapImage(new Uri("ms-appx:///Assets/Logos/youtube-logo.png")) },
                { SocialPostType.Instagram, new BitmapImage(new Uri("ms-appx:///Assets/Logos/instagram-logo.png")) },
                { SocialPostType.Spotify, new BitmapImage(new Uri("ms-appx:///Assets/Logos/spotify-logo.png")) },
                { SocialPostType.Deezer, new BitmapImage(new Uri("ms-appx:///Assets/Logos/deezer-logo.png")) },
                { SocialPostType.AppleMusic, new BitmapImage(new Uri("ms-appx:///Assets/Logos/apple-music-logo.png")) }
            };

            // cache logos
            foreach (var logo in logos.Values)
            {
                ImageCache.Instance.PreCacheAsync(((BitmapImage) logo).UriSource, false, true);
            }

            return logos;
        }
    }
}