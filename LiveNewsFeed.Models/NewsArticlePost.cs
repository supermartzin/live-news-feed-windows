using System;
using System.Collections.Generic;

namespace LiveNewsFeed.Models
{
    public class NewsArticlePost : IComparable<NewsArticlePost>
    {
        public string Id { get; }

        public string Title { get; }

        public string Content { get; }

        public string? ExtendedContent { get; }

        public DateTime PublishTime { get; }

        public DateTime UpdateTime { get; }

        public Image? Image { get; }

        public SocialPost? SocialPost { get; }

        public Uri FullArticleUrl { get; }

        public ISet<Tag> Tags { get; }

        public ISet<Category> Categories { get; }

        public bool IsImportant { get; }

        public string NewsFeedName { get; }

        public NewsArticlePost(string id,
                               string title,
                               string content,
                               DateTime publishTime,
                               DateTime updateTime,
                               Uri fullArticleUrl,
                               bool isImportant,
                               string newsFeedName,
                               string? extendedContent = default,
                               Image? image = default,
                               SocialPost? socialPost = default,
                               ISet<Category>? categories = default,
                               ISet<Tag>? tags = default)
        {
            Id = id ?? throw new ArgumentNullException(nameof(id));
            Title = title ?? throw new ArgumentNullException(nameof(title));
            Content = content ?? throw new ArgumentNullException(nameof(content));
            PublishTime = publishTime;
            UpdateTime = updateTime;
            FullArticleUrl = fullArticleUrl ?? throw new ArgumentNullException(nameof(fullArticleUrl));
            IsImportant = isImportant;
            NewsFeedName = newsFeedName;

            ExtendedContent = extendedContent;
            Image = image;
            SocialPost = socialPost;
            Categories = categories ?? new HashSet<Category>();
            Tags = tags ?? new HashSet<Tag>();
        }

        public int CompareTo(NewsArticlePost other)
        {
            return PublishTime.CompareTo(other.PublishTime);
        }

        public override bool Equals(object? obj)
        {
            if (ReferenceEquals(null, obj))
                return false;
            if (ReferenceEquals(this, obj))
                return true;
            if (obj.GetType() != GetType())
                return false;

            return Equals((NewsArticlePost) obj);
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
        
        protected bool Equals(NewsArticlePost other)
        {
            return Id == other.Id;
        }
    }
}
