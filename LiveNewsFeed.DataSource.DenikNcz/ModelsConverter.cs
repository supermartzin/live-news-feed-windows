using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using LiveNewsFeed.Models;

using LiveNewsFeed.DataSource.DenikNcz.DTO;

namespace LiveNewsFeed.DataSource.DenikNcz
{
    internal static class ModelsConverter
    {
        private static readonly Dictionary<int, Category> CodeToCategoryDictionary = new ()
        {
            { 8, Category.Local },
            { 9, Category.World },
            { 10, Category.Economy },
            { 12, Category.Arts },
            { 14, Category.Commentary },
            { 11, Category.Science },
            { 13, Category.Sport }
        };

        private static readonly Dictionary<Category, int> CategoryToCodeDictionary = new ()
        {
            { Category.Local, 8 },
            { Category.World, 9 },
            { Category.Economy, 10 },
            { Category.Arts, 12 },
            { Category.Commentary, 14 },
            { Category.Science, 11 },
            { Category.Sport, 13 }
        };

        private static readonly Dictionary<string, long> TagNameToCodeDictionary = new();

        public static NewsArticlePost ToNewsArticlePost(ArticlePostDTO postDto, string newsFeedName) =>
            new(postDto.Id.ToString(),
                string.Empty,
                postDto.Content.MainText,
                postDto.Created,
                postDto.Updated,
                new Uri(postDto.Url),
                postDto.ImportantCode.HasValue,
                newsFeedName,
                postDto.Content.ExtendedText,
                ToImage(postDto.Image),
                ToSocialPost(postDto.SocialPost),
                ParseCategories(postDto.Categories),
                ParseTags(postDto.Tags));
        
        public static Category ToCategory(CategoryDTO categoryDto) => ToCategory((int) categoryDto.Id);

        public static Tag ToTag(TagDTO tagDto)
        {
            // add to dictionary
            TagNameToCodeDictionary[tagDto.Name] = tagDto.Id;

            return new Tag(tagDto.Name);
        }

        public static Image? ToImage(ImageDTO? imageDto) => imageDto != null
            ? new Image(new Uri(imageDto.NormalSizeUrl),
                        HttpUtility.HtmlDecode(imageDto.Title),
                        imageDto.LargeSizeUrl != null ? new Uri(imageDto.LargeSizeUrl) : null)
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

        private static ISet<Category>? ParseCategories(IEnumerable<CategoryDTO>? categoryDtos)
        {
            if (categoryDtos == null)
                return default;

            var categories = new HashSet<Category>(categoryDtos.Select(ToCategory));

            categories.RemoveWhere(category => category == Category.NotCategorized);

            return categories;
        }
    }
}