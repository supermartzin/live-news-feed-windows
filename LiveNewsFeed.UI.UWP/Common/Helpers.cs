using System;
using System.Collections.Generic;
using Windows.UI.Xaml.Media;

using LiveNewsFeed.Models;

using LiveNewsFeed.UI.UWP.ViewModels;

namespace LiveNewsFeed.UI.UWP.Common
{
    internal static class Helpers
    {
        private static readonly FontFamily SegoeUiSymbolFontFamily = new ("Segoe UI Symbol");
        private static readonly FontFamily SegoeMdl2AssetsFontFamily = new ("Segoe MDL2 Assets");

        private static readonly Dictionary<string, ImageSource> NewsFeedLogos = new();
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

        public static void RegisterLogoForNewsFeed(string newsFeedName, ImageSource logoSource)
        {
            if (newsFeedName == null)
                throw new ArgumentNullException(nameof(newsFeedName));

            NewsFeedLogos[newsFeedName] = logoSource ?? throw new ArgumentNullException(nameof(logoSource));
        }

        public static CategoryViewModel GetCategoryViewModel(Category category) => CategoriesMap[category];
    }
}