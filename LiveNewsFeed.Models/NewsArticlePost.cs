using System;
using System.Collections.Generic;

namespace LiveNewsFeed.Models
{
    public class NewsArticlePost
    {
        public string Id { get; }

        public string Title { get; }

        public string Message { get; }

        public DateTime PublishTime { get; }

        public DateTime UpdateTime { get; }

        public Uri? ImageUrl { get; }

        public Uri? SocialPostUrl { get; }

        public Uri FullArticleUrl { get; }

        public ISet<Tag> Tags { get; }

        public ISet<Category> Categories { get; }

        public bool IsImportant { get; }

        public NewsArticlePost(string id, 
                               string title,
                               string message,
                               DateTime publishTime,
                               DateTime updateTime,
                               Uri fullArticleUrl,
                               bool isImportant,
                               Uri? imageUrl = null,
                               Uri? socialPostUrl = null)
        {
            Id = id ?? throw new ArgumentNullException(nameof(id));
            Title = title;
            Message = message;
            PublishTime = publishTime;
            UpdateTime = updateTime;
            FullArticleUrl = fullArticleUrl;
            IsImportant = isImportant;

            ImageUrl = imageUrl;
            SocialPostUrl = socialPostUrl;

            Tags = new HashSet<Tag>();
            Categories = new HashSet<Category>();
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
