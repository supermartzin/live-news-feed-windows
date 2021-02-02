using System.Collections.Generic;
using LiveNewsFeed.DataSource.Common;

namespace LiveNewsFeed.UI.UWP.Managers
{
    public interface IDataSourcesManager
    {
        void RegisterDataSource(NewsFeedDataSource newsFeedDataSource);

        IList<NewsFeedDataSource> GetRegisteredDataSources();
    }
}