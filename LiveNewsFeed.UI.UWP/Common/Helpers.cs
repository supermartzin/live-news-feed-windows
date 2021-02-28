using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Windows.Data.Html;
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

        public static NewsArticlePostViewModel ToViewModel(NewsArticlePost articlePost) =>
            new(articlePost.Title,
                                      HtmlUtilities.ConvertToText(articlePost.Content).Trim(),
                                      articlePost.PublishTime,
                                      articlePost.FullArticleUrl,
                                      new ImageBrush { ImageSource = GetLogoForNewsFeed(articlePost.NewsFeedName) },
                                      ToImageViewModel(articlePost.Image),
                                      SanitizeSocialPostContent(articlePost.SocialPost?.Content),
                                      articlePost.Categories
                                                 .Where(category => category != Category.NotCategorized)
                                                 .Select(GetCategoryViewModel),
                                      articlePost.Tags.Select(tag => new TagViewModel(tag.Name)));

        private static ImageViewModel? ToImageViewModel(Image? image) => image != null
            ? new ImageViewModel(image.Url,
                                 image.Title != null ? HtmlUtilities.ConvertToText(image.Title).Trim() : default,
                                 image.LargeSizeUrl)
            : default;

        private static string? SanitizeSocialPostContent(string? socialPostContent) => socialPostContent != null
            ? $"<!DOCTYPE html>{HttpUtility.HtmlDecode(socialPostContent)}</html>"
            : default;
    }
}