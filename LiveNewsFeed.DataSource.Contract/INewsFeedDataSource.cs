using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using LiveNewsFeed.Models;

namespace LiveNewsFeed.DataSource.Contract
{
    public interface INewsFeedDataSource : IDisposable
    {
        Task<IList<NewsArticlePost>> GetLatestPostsAsync(int count = 50);
        
        Task<IList<NewsArticlePost>> GetLatestPostsAsync(Category category, int count = 50);

        Task<IList<NewsArticlePost>> GetLatestPostsAsync(Tag tag, int count = 50);

        Task<IList<NewsArticlePost>> GetLatestImportantPostsAsync(int count = 50);

        Task<IList<NewsArticlePost>> GetNewPostsSinceLastUpdateAsync();
    }
}
