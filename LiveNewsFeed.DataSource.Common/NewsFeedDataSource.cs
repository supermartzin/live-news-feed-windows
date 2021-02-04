using System;

namespace LiveNewsFeed.DataSource.Common
{
    public class NewsFeedDataSource
    {
        public string Name { get; }

        public Uri LogoUrl { get; }

        public INewsFeed NewsFeed { get; }

        public NewsFeedDataSource(INewsFeed newsFeed,
                                  Uri logoUrl)
        {
            NewsFeed = newsFeed ?? throw new ArgumentNullException(nameof(newsFeed));
            LogoUrl = logoUrl ?? throw new ArgumentNullException(nameof(logoUrl));

            Name = newsFeed.Name;
        }
    }
}