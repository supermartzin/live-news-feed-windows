using System;
using System.Collections.Generic;
using Windows.UI;
using Windows.UI.Xaml.Media;

using LiveNewsFeed.Models;

using LiveNewsFeed.UI.UWP.ViewModels;

namespace LiveNewsFeed.UI.UWP.Common
{
    internal static class Helpers
    {
        private static readonly FontFamily SegoeUiSymbolFontFamily = new ("Segoe UI Symbol");
        private static readonly FontFamily SegoeMdl2AssetsFontFamily = new ("Segoe MDL2 Assets");

        private static readonly Dictionary<string, ImageBrush> NewsFeedLogos = new();
        private static readonly Dictionary<Category, CategoryViewModel> CategoriesMap = new()
        {
            { Category.Local, new CategoryViewModel(Category.Local, Color.FromArgb(255, 45, 157, 210), "\uE10F", SegoeMdl2AssetsFontFamily) },
            { Category.World, new CategoryViewModel(Category.World, Color.FromArgb(255, 196, 111, 13), "\uE128", SegoeMdl2AssetsFontFamily) },
            { Category.Economy, new CategoryViewModel(Category.Economy,  Color.FromArgb(255, 111, 77, 128), "\U0001F4B2", SegoeUiSymbolFontFamily) },
            { Category.Sport, new CategoryViewModel(Category.Sport, Color.FromArgb(255, 0, 138, 62), "\u26BD", SegoeUiSymbolFontFamily) },
            { Category.Arts, new CategoryViewModel(Category.Arts, Color.FromArgb(255, 7, 97, 141), "\U0001F3A8", SegoeUiSymbolFontFamily) },
            { Category.Science, new CategoryViewModel(Category.Science,  Color.FromArgb(255, 233, 29, 150), "\U0001F4A1", SegoeUiSymbolFontFamily) },
            { Category.Commentary, new CategoryViewModel(Category.Commentary, Color.FromArgb(255, 191, 33, 52), "\uE206", SegoeMdl2AssetsFontFamily) }
        };

        public static ImageBrush? GetLogoForNewsFeed(string newsFeedName)
        {
            if (newsFeedName == null)
                throw new ArgumentNullException(nameof(newsFeedName));

            return NewsFeedLogos.ContainsKey(newsFeedName) ? NewsFeedLogos[newsFeedName] : default;
        }

        public static void RegisterLogoForNewsFeed(string newsFeedName, ImageBrush logo)
        {
            if (newsFeedName == null)
                throw new ArgumentNullException(nameof(newsFeedName));

            NewsFeedLogos[newsFeedName] = logo ?? throw new ArgumentNullException(nameof(logo));
        }

        public static CategoryViewModel GetCategoryViewModel(Category category) => CategoriesMap[category];
    }
}