using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using LiveNewsFeed.Models;

namespace LiveNewsFeed.DataSource.Common
{
    public interface INewsFeed
    {
        /// <summary>
        /// Name of the news feed source
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Get posts that are satisfying specified parameters.
        /// </summary>
        /// <param name="before">gets only posts before specified <see cref="DateTime"/></param>
        /// <param name="after">gets only posts after specified <see cref="DateTime"/></param>
        /// <param name="category">gets only posts of this <see cref="Category"/></param>
        /// <param name="tag">gets only posts with this <see cref="Tag"/></param>
        /// <param name="important">gets only posts that are important</param>
        /// <param name="count">gets selected number of posts</param>
        /// <returns>collection of <see cref="NewsArticlePost"/> objects</returns>
        Task<IList<NewsArticlePost>> GetPostsAsync(DateTime? before = default,
                                                   DateTime? after = default,
                                                   Category? category = default,
                                                   Tag? tag = default,
                                                   bool? important = default,
                                                   int? count = default);
    }
}
