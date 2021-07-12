using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Microsoft.Toolkit.Uwp.UI;
using Microsoft.Toolkit.Uwp;

using LiveNewsFeed.Models;

using LiveNewsFeed.UI.UWP.ViewModels;

namespace LiveNewsFeed.UI.UWP.Common
{
    internal static class Helpers
    {
        private static readonly FontFamily SegoeUiSymbolFontFamily = new ("Segoe UI Symbol");
        private static readonly FontFamily SegoeMdl2AssetsFontFamily = new ("Segoe MDL2 Assets");

        private static readonly Dictionary<ApplicationTheme, Dictionary<string, Uri>> NewsFeedLogos = new();
        private static readonly Dictionary<SocialPostType, ImageSource> SocialSiteLogos = CreateSocialSiteLogos();
        private static readonly Dictionary<Category, CategoryViewModel> CategoriesMap = new();

        public static void CreateCategoriesMap()
        {
            CategoriesMap[Category.Local] = new CategoryViewModel(Category.Local, "\uE10F", SegoeMdl2AssetsFontFamily);
            CategoriesMap[Category.World] = new CategoryViewModel(Category.World, "\uE128", SegoeMdl2AssetsFontFamily);
            CategoriesMap[Category.Economy] = new CategoryViewModel(Category.Economy, "\U0001F4B2", SegoeUiSymbolFontFamily);
            CategoriesMap[Category.Sport] = new CategoryViewModel(Category.Sport, "\u26BD", SegoeUiSymbolFontFamily);
            CategoriesMap[Category.Arts] = new CategoryViewModel(Category.Arts, "\U0001F3A8", SegoeUiSymbolFontFamily);
            CategoriesMap[Category.Science] = new CategoryViewModel(Category.Science, "\U0001F4A1", SegoeUiSymbolFontFamily);
            CategoriesMap[Category.Commentary] = new CategoryViewModel(Category.Commentary, "\uE206", SegoeMdl2AssetsFontFamily);
        }

        public static Uri? GetLogoForNewsFeed(string newsFeedName, ApplicationTheme theme)
        {
            if (newsFeedName == null)
                throw new ArgumentNullException(nameof(newsFeedName));

            if (NewsFeedLogos.ContainsKey(theme) && NewsFeedLogos[theme].ContainsKey(newsFeedName)) 
                return NewsFeedLogos[theme][newsFeedName];

            return default;
        }

        public static ImageSource? GetLogoForSocialSite(SocialPostType postType) =>
            SocialSiteLogos.ContainsKey(postType) ? SocialSiteLogos[postType] : default;

        public static void RegisterLogoForNewsFeed(string newsFeedName, ApplicationTheme theme, Uri logoPath)
        {
            if (newsFeedName == null)
                throw new ArgumentNullException(nameof(newsFeedName));
            if (logoPath == null)
                throw new ArgumentNullException(nameof(logoPath));

            if (!NewsFeedLogos.ContainsKey(theme))
                NewsFeedLogos.Add(theme, new Dictionary<string, Uri>());

            NewsFeedLogos[theme][newsFeedName] = logoPath;

            // cache logo in memory
            ImageCache.Instance.PreCacheAsync(logoPath, false, true);
        }

        public static CategoryViewModel GetCategoryViewModel(Category category) => CategoriesMap[category];
        
        public static async Task InvokeOnUiAsync(Action action)
        {
            if (action == null)
                throw new ArgumentNullException(nameof(action));

            await CoreApplication.MainView
                                 .DispatcherQueue
                                 .EnqueueAsync(action);
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