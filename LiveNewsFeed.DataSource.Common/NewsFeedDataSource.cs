using System;

namespace LiveNewsFeed.DataSource.Common
{
    public class NewsFeedDataSource
    {
        public string Name { get; }

        public string Logo { get; }

        public INewsFeed NewsFeed { get; }

        public NewsFeedDataSource(INewsFeed newsFeed,
                                  string logo)
        {
            NewsFeed = newsFeed ?? throw new ArgumentNullException(nameof(newsFeed));
            Logo = logo ?? throw new ArgumentNullException(nameof(logo));

            Name = newsFeed.Name;
        }
    }
}