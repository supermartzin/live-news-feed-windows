using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using LiveNewsFeed.Models;

using LiveNewsFeed.DataSource.DennikNsk.DTO;

namespace LiveNewsFeed.DataSource.DennikNsk
{
    internal static class ModelsConverter
    {
        private static readonly Dictionary<int, Category> CodeToCategoryDictionary = new ()
        {
            { 430, Category.Local },
            { 431, Category.World },
            { 432, Category.Economy },
            { 433, Category.Arts },
            { 435, Category.Commentary },
            { 508, Category.Science },
            { 545, Category.Sport }
        };

        private static readonly Dictionary<Category, int> CategoryToCodeDictionary = new ()
        {
            { Category.Local, 430 },
            { Category.World, 431 },
            { Category.Economy, 432 },
            { Category.Arts, 433 },
            { Category.Commentary, 435 },
            { Category.Science, 508 },
            { Category.Sport, 545 }
        };

        private static readonly Dictionary<string, long> TagNameToCodeDictionary = new();

        public static NewsArticlePost ToNewsArticlePost(ArticlePostDTO postDto, string newsFeedName) =>
            new(postDto.Id.ToString(),
                              newsFeedName,
                              postDto.Content.MainText,
                              postDto.Created,
                              postDto.Updated,
                              new Uri(postDto.Url),
                              postDto.ImportantCode.HasValue,
                              newsFeedName,
                              ToImage(postDto.Image),
                              ToSocialPost(postDto.SocialPost),
                              ParseCategories(postDto.Categories),
                              ParseTags(postDto.Tags));
        
        public static Category ToCategory(CategoryDTO categoryDto) => ToCategory((int) categoryDto.Id);

        public static Tag ToTag(TagDTO tagDto)
        {
            // add to dictionary
            TagNameToCodeDictionary[tagDto.Name] = tagDto.Id;

            return new(tagDto.Name);
        }

        public static Image? ToImage(ImageDTO? imageDto) => imageDto != null
            ? new Image(new Uri(imageDto.NormalSizeUrl),
                        HttpUtility.HtmlDecode(imageDto.Title),
                        imageDto.LargeSizeUrl != null ? new Uri(imageDto.LargeSizeUrl) : default)
            : default;
        
        public static Category ToCategory(int dataSourceCode) =>
            CodeToCategoryDictionary.ContainsKey(dataSourceCode)
                ? CodeToCategoryDictionary[dataSourceCode]
                : Category.NotCategorized;

        public static int ToCode(Category category) =>
            CategoryToCodeDictionary.ContainsKey(category)
                ? CategoryToCodeDictionary[category]
                : 0;

        public static long ToCode(Tag tag) =>
            TagNameToCodeDictionary.ContainsKey(tag.Name)
                ? TagNameToCodeDictionary[tag.Name]
                : 0;


        private static SocialPost? ToSocialPost(SocialPostDTO? socialPostDto) => socialPostDto != null
            ? new SocialPost(new Uri(socialPostDto.Url), socialPostDto.EmbedCode)
            : default;

        private static ISet<Tag>? ParseTags(IEnumerable<TagDTO>? tagDtos) => tagDtos != null
            ? new HashSet<Tag>(tagDtos.Select(ToTag))
            : default;

        private static ISet<Category>? ParseCategories(IEnumerable<CategoryDTO>? categoryDtos) => categoryDtos != null
            ? new HashSet<Category>(categoryDtos.Select(ToCategory))
            : default;
    }
}