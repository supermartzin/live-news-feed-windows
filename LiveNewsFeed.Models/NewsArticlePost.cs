using System;
using System.Collections.Generic;

namespace LiveNewsFeed.Models
{
    public class NewsArticlePost
    {
        public string Id { get; }

        public string Title { get; }

        public string Content { get; }

        public DateTime PublishTime { get; }

        public DateTime UpdateTime { get; }

        public Image? Image { get; }

        public Uri? SocialPostUrl { get; }

        public Uri FullArticleUrl { get; }

        public ISet<Tag> Tags { get; }

        public ISet<Category> Categories { get; }

        public bool IsImportant { get; }

        public NewsArticlePost(string id, 
                               string title,
                               string content,
                               DateTime publishTime,
                               DateTime updateTime,
                               Uri fullArticleUrl,
                               bool isImportant,
                               Image? image = null,
                               Uri? socialPostUrl = null,
                               ISet<Category>? categories = null,
                               ISet<Tag>? tags = null)
        {
            Id = id ?? throw new ArgumentNullException(nameof(id));
            Title = title ?? throw new ArgumentNullException(nameof(title));
            Content = content ?? throw new ArgumentNullException(nameof(content));
            PublishTime = publishTime;
            UpdateTime = updateTime;
            FullArticleUrl = fullArticleUrl ?? throw new ArgumentNullException(nameof(fullArticleUrl));
            IsImportant = isImportant;

            Image = image;
            SocialPostUrl = socialPostUrl;
            Categories = categories ?? new HashSet<Category>();
            Tags = tags ?? new HashSet<Tag>();
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
