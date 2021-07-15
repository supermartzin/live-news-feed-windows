using System;

namespace LiveNewsFeed.DataSource.Common
{
    public class NewsFeedDataSource
    {
        public string Name { get; }

        public Logo Logo { get; }

        public bool IsEnabled { get; set; }

        public INewsFeed NewsFeed { get; }

        public NewsFeedDataSource(INewsFeed newsFeed, Logo logo, bool isEnabled)
        {
            NewsFeed = newsFeed ?? throw new ArgumentNullException(nameof(newsFeed));
            Logo = logo ?? throw new ArgumentNullException(nameof(logo));

            Name = newsFeed.Name;
            IsEnabled = isEnabled;
        }
    }
}