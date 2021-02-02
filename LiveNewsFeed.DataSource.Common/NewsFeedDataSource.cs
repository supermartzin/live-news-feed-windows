using System;

namespace LiveNewsFeed.DataSource.Common
{
    public class NewsFeedDataSource
    {
        public string Name { get; }

        public INewsFeed NewsFeed { get; }

        public NewsFeedDataSource(string name, INewsFeed newsFeed)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            NewsFeed = newsFeed ?? throw new ArgumentNullException(nameof(newsFeed));
        }
    }
}