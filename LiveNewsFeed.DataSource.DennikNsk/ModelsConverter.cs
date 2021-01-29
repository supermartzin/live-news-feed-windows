using System;
using System.Collections.Generic;
using System.Linq;

using LiveNewsFeed.DataSource.DennikNsk.DTO;
using LiveNewsFeed.Models;

namespace LiveNewsFeed.DataSource.DennikNsk
{
    internal static class ModelsConverter
    {
        private const string DefaultTitle = "Denník N";

        private static readonly Dictionary<int, Category> DataSourceCodeToCategoryDictionary = new ()
        {
            { 430, Category.Local },
            { 431, Category.World },
            { 432, Category.Economy },
            { 433, Category.Arts },
            { 435, Category.Commentary },
            { 508, Category.Science },
            { 545, Category.Sport }
        };

        private static readonly Dictionary<Category, int> CategoryToDataSourceCodeDictionary = new ()
        {
            { Category.Local, 430 },
            { Category.World, 431 },
            { Category.Economy, 432 },
            { Category.Arts, 433 },
            { Category.Commentary, 435 },
            { Category.Science, 508 },
            { Category.Sport, 545 }
        };

        public static NewsArticlePost ToNewsArticlePost(ArticlePostDTO postDto) =>
            new(postDto.Id.ToString(),
                              DefaultTitle,
                              postDto.Content.MainText,
                              postDto.Created,
                              postDto.Updated,
                              new Uri(postDto.Url),
                              postDto.ImportantCode.HasValue,
                              ToImage(postDto.Image),
                              ToUri(postDto.SocialPost?.Url),
                              new HashSet<Category>(postDto.Categories.Select(ToCategory)),
                              new HashSet<Tag>(postDto.Tags.Select(ToTag)));

        public static Category ToCategory(CategoryDTO categoryDto) => ModelsConverter.ToCategory((int) categoryDto.Id);

        public static Tag ToTag(TagDTO tagDto) => new(tagDto.Name);

        public static Image? ToImage(ImageDTO? imageDto) => imageDto != null ? new Image(imageDto.Title, new Uri(imageDto.NormalSizeUrl)) : default;

        public static Uri? ToUri(string? url) => url != null ? new Uri(url) : default;

        public static Category ToCategory(int dataSourceCode)
        {
            return DataSourceCodeToCategoryDictionary.ContainsKey(dataSourceCode) 
                        ? DataSourceCodeToCategoryDictionary[dataSourceCode]
                        : Category.NotCategorized;
        }

        public static int ToDataSourceCode(Category category)
        {
            return CategoryToDataSourceCodeDictionary.ContainsKey(category)
                        ? CategoryToDataSourceCodeDictionary[category] 
                        : 0;
        }
    }
}